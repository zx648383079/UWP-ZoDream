using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using ZoDream.Model;
using ZoDream.Services;
using ZoDreamToolkit.Common;

namespace ZoDream.Helper
{
    class BookHelper
    {

        public static readonly Regex Pattern = new Regex(@"^\s{0,6}(正文|楔子)?(第?)\s*[0-9０１２３４５６７８９一二三四五六七八九十百千]{1,10}([章回节卷集幕计])?[\s\S]{0,30}$");

        public static async Task<List<BookChapter>> GetBookChapterAsync(string url, BookRule rule)
        {
            var html = await HttpHelper.GetAsync(url);
            return await GetBookChapterAsync(html, url, rule);
        }
        /// <summary>
        /// 根据目录代码获取章节
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static async Task<List<BookChapter>> GetBookChapterAsync(string html, string baseUrl, BookRule rule)
        {
            return await GetBookChapterAsync(html, new Uri(baseUrl), rule);
        }

        public static async Task<List<BookChapter>> GetBookChapterAsync(string html, Uri baseUrl, BookRule rule)
        {
            var chapters = new List<BookChapter>();
            html = NarrowText(html, rule.ListStart, rule.ListEnd);
            var matches = Regex.Matches(html, @"<a[^<>]+?href=""?(?<href>[^""<>\s]+)[^<>]*>(?<title>[\s\S]+?)</a>");
            foreach (Match match in matches)
            {
                var chapter = await GetChapterAsync(HttpHelper.GetAbsolute(baseUrl, match.Groups["href"].Value), rule);
                if (string.IsNullOrWhiteSpace(chapter.Name))
                {
                    chapter.Name = match.Groups["title"].Value;
                }
                chapters.Add(chapter);
            }
            return chapters;
        }

        /// <summary>
        /// 下载直接保存不返回
        /// </summary>
        /// <param name="html"></param>
        /// <param name="book"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static async Task GetBookChapterAsync(string html, Book book, BookRule rule)
        {
            var i = 1;
            html = NarrowText(html, rule.ListStart, rule.ListEnd);
            var matches = Regex.Matches(html, @"<a[^<>]+?href=""?(?<href>[^""<>\s]+)[^<>]*>(?<title>[\s\S]+?)</a>");
            foreach (Match match in matches)
            {
                var chapter = await GetChapterAsync(HttpHelper.GetAbsolute(book.Url, match.Groups["href"].Value), rule);
                if (string.IsNullOrWhiteSpace(chapter.Name))
                {
                    chapter.Name = match.Groups["title"].Value;
                }
                chapter.BookId = book.Id;
                chapter.Position = i;
                chapter.Save();
                i++;
            }
            book.Count = i - 1;
            book.Save();
        }

        /// <summary>
        /// 根据目录网址获取章节
        /// </summary>
        /// <param name="url"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static async Task<BookChapter> GetChapterAsync(string url, BookRule rule)
        {
            var html = await HttpHelper.GetAsync(url);
            return new BookChapter()
            {
                Name = NarrowText(html, rule.TitleStart, rule.TitleEnd),
                Content = HtmlToText(NarrowText(html, rule.ContentStart, rule.ContentEnd)),
                Url = url
            };
        }
        /// <summary>
        /// 从文件切分章节
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<List<BookChapter>> GetBookChapterAsync(StorageFile file)
        {
            var content = await StorageHelper.GetFileTextAsync(file);
            return GetBookChapter(GetLines(content));
        }

        /// <summary>
        /// 根据行切割章节
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<BookChapter> GetBookChapter(IList<string> lines)
        {
            var chapters = new List<BookChapter>();
            var index = _getChapterTitle(lines);
            int start;
            var position = 1;
            while (index >= 0)
            {
                start = index;
                index = _getChapterTitle(index + 1, lines);
                chapters.Add(new BookChapter()
                {
                    Name = lines[start],
                    Position = position,
                    Content = _getChapterContent(lines, start, index - 1)
                });
                position++;
            }

            return chapters;
        }

        /// <summary>
        /// 直接保存不返回
        /// </summary>
        /// <param name="file"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public static async Task GetBookChapterAsync(StorageFile file, Book book)
        {
            var lines = new List<string>(); // 已获取的行
            string line; // 当前行
            var maxScore = 0; // 最高分数
            var maxLine = -1; //最高分数对应的行号
            var maxCount = 0;
            var count = 0;    // 当前字数
            var lineLength = 0;   // 当前行的长度
            var index = -1;        // 当前行在数组中的序号
            var score = 0;         // 当前行分数
            var lastMaxLine = -1;
            var position = 1;  // 章节总数
            BookChapter chapter;   // 章节
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var inputStream = await file.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream, StorageHelper.GetEncoding(classicStream, Encoding.GetEncoding("gbk"))))
            {
                while (streamReader.Peek() >= 0)
                {
                    line = streamReader.ReadLine();
                    lines.Add(line);
                    index ++;
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        score += 10;
                        continue;
                    }
                    if (lastMaxLine < 0)
                    {
                        lastMaxLine = index;
                        score = 0;
                        continue;
                    }
                    lineLength = line.Length;
                    count += lineLength;
                    if (lineLength > 50)
                    {
                        score = 0;
                        continue;
                    }
                    if (Pattern.IsMatch(line))
                    {
                        // 正则匹配
                        var match = Pattern.Match(line);
                        var a = string.IsNullOrEmpty(match.Groups[2].Value);
                        var b = string.IsNullOrEmpty(match.Groups[3].Value);
                        if (!a && !b)
                        {
                            // 保存当前已搜索的章节
                            chapter = new BookChapter()
                            {
                                Name = lines[lastMaxLine],
                                Position = position,
                                Content = _getChapterContent(lines, 0, index - 1),
                                BookId = book.Id
                            };
                            chapter.Save();
                            position++;
                            lines.RemoveRange(0, index);
                            // 初始化
                            lastMaxLine = 0;
                            index = 0;
                            maxScore = 0;
                            count = 0;
                            score = 0;
                            continue;
                        }
                        if (count < 500)
                        {
                            score = 0;
                            continue;
                        }
                        if (!b)
                        {
                            maxScore = score + 110;
                            maxLine = index;
                            maxCount = count;
                        }
                        else
                        {
                            maxScore = score + 105;
                            maxLine = index;
                            maxCount = count;
                        }
                        score = 0;
                        continue;
                    }
                    if (count < 500)
                    {
                        score = 0;
                        continue;
                    }
                    score += 10 - Math.Abs(count - 10000) / 1000 + 5 * Math.Abs(20 - lineLength);
                    if (line.IndexOf("“") >= 0 || line.IndexOf("‘") >= 0 || line.IndexOf("：") >= 0)
                    {
                        score -= 5;
                    }
                    if (score > maxScore)
                    {
                        maxScore = score;
                        maxLine = index;
                        score = 0;
                        maxCount = count;
                    }
                    if (count >= 20000)
                    {
                        // 保存当前已搜索的章节
                        chapter = new BookChapter()
                        {
                            Name = lines[lastMaxLine],
                            Position = position,
                            Content = _getChapterContent(lines, 0, maxLine - 1),
                            BookId = book.Id
                        };
                        chapter.Save();
                        position++;
                        lines.RemoveRange(0, maxLine);
                        // 初始化
                        lastMaxLine = 0;
                        index -= maxLine;
                        maxScore = 0;
                        count -= maxCount;
                        continue;
                    }
                    score = 0;
                }
            }
            // 最后保存
            chapter = new BookChapter()
            {
                Name = lines[lastMaxLine],
                Position = position,
                Content = _getChapterContent(lines, 0, lines.Count - 1),
                BookId = book.Id
            };
            chapter.Save();

            book.Count = position;
            book.Save();
            // 原始方法
            //var content = await StorageHelper.GetFileTextAsync(file);
            //GetBookChapter(GetLines(content), book);
        }

        /// <summary>
        /// 直接保存不返回
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="book"></param>
        public static void GetBookChapter(IList<string> lines, Book book)
        {
            var index = _getChapterTitle(lines);
            int start;
            var position = 1;
            BookChapter chapter;
            while (index >= 0)
            {
                start = index;
                index = _getChapterTitle(index + 1, lines);
                chapter = new BookChapter()
                {
                    Name = lines[start],
                    Position = position,
                    Content = _getChapterContent(lines, start, index - 1),
                    BookId = book.Id
                };
                chapter.Save();
                position++;
            }
            book.Count = position - 1;
            book.Save();
        }

        private static string _getChapterContent(IList<string> lines, int start, int end)
        {
            var length = lines.Count;
            if (start >= length)
            {
                return "";
            }
            var content = new StringBuilder();
            content.AppendLine(lines[start]);
            start++;
            for (; start < length; start++)
            {
                var line = lines[start];
                //if (string.IsNullOrWhiteSpace(line))
                //{
                //    continue;
                //}
                content.AppendLine(line);
                if (end == start)
                {
                    break;
                }
            }
            return content.ToString();
        }

        private static int _getChapterTitle(int index, IList<string> lines) {
            var length = lines.Count;
            var count = 0;
            var lineScore = 0;
            var lineIndex = -1;
            int lineLength;
            int score;
            for (; index < length; index++)
            {
                var line = lines[index];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                lineLength = line.Length;
                count += lineLength;
                if (count < 500 || lineLength > 40)
                {
                    continue;
                }
                if (Pattern.IsMatch(line))
                {
                    // 正则匹配
                    var match = Pattern.Match(line);
                    var a = string.IsNullOrEmpty(match.Groups[2].Value);
                    var b = string.IsNullOrEmpty(match.Groups[3].Value);
                    if (!a && !b)
                    {
                        return index;
                    }
                    if (!b)
                    {
                        lineScore = 110;
                        lineIndex = index;
                    } else
                    {
                        lineScore = 105;
                        lineIndex = index;
                    }
                    continue;
                }
                score = 10 - Math.Abs(count - 10000) / 1000 + 5 * Math.Abs(20 - lineLength);
                if (line.IndexOf("“") >= 0 || line.IndexOf("‘") >= 0 || line.IndexOf("：") >= 0) {
                    score -= 5;
                }
                if (score > lineScore) {
                    lineScore = score;
                    lineIndex = index;
                }
                if (count >= 20000)
                {
                    break;
                }
            }
            //如果读取完了就到最后
            if (index >= length - 1)
            {
                return -1;
            }
            return lineIndex;
        }

        private static int _getChapterTitle(IList<string> lines)
        {
            var length = lines.Count;
            for (int i = 0; i < length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从本地文件获取 BOOK
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Book GetBook(StorageFile file)
        {
            return new Book()
            {
                Name = file.DisplayName,
                Count = 1
            };
        }

        /// <summary>
        /// 从源码里获取 book
        /// </summary>
        /// <param name="html"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static Book GetBook(string html, BookRule rule)
        {
            return new Book()
            {
                Name = NarrowText(html, rule.TitleStart, rule.TitleEnd),
                Author = NarrowText(html, rule.AuthorStart, rule.AuthorEnd),
                Description = NarrowText(html, rule.DescriptionStart, rule.DescriptionEnd)
            };
        }

        /// <summary>
        /// 缩小范围 使用index方法截取 不包括
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string NarrowText(string content, string begin, string end)
        {
            if (string.IsNullOrWhiteSpace(begin)) {
                var match = Regex.Match(content, end, RegexOptions.IgnoreCase);
                return match.Groups[match.Groups.Count - 1].Value;
            }
            if (string.IsNullOrWhiteSpace(end)) {
                var match = Regex.Match(content, begin, RegexOptions.IgnoreCase);
                return match.Groups[1].Value;
            }
            var index = content.IndexOf(begin, StringComparison.Ordinal);
            if (index < 0)
            {
                index = 0;
            }
            else
            {
                index += begin.Length;
            }
            var next = Math.Min(content.IndexOf(end, index, StringComparison.Ordinal), content.Length - index);
            return content.Substring(index, next);
        }

        /// <summary>
        /// 选择文件并获取生成书及章节
        /// </summary>
        /// <returns></returns>
        public static async Task<List<Book>> OpenAsync()
        {
            var files = await StorageHelper.OpenFilesAsync();
            if (files == null)
            {
                return null;
            }
            var books = new List<Book>();
            
            await SqlHelper.Conn.OpenAsync();
            foreach (var file in files)
            {
                var book = GetBook(file);
                book.Save();
                await GetBookChapterAsync(file, book);
                books.Add(book);
            }
            SqlHelper.Conn.Close();
            return books;
        }

        /// <summary>
        /// 下载并保存
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<Book> OpenAsync(Uri url)
        {
            SqlHelper.Conn.Open();
            var rule = GetBookRule(url.Host);
            if (rule == null)
            {
                SqlHelper.Conn.Close();
                return null;
            }
            var html = await HttpHelper.GetAsync(url);
            var book = GetBook(html, rule);
            book.IsLocal = false;
            book.Url = url.AbsoluteUri;
            book.Save();
            await GetBookChapterAsync(html, book, rule);
            SqlHelper.Conn.Close();
            return book;
        }

        public static async Task<Book> OpenAsync(string url)
        {
            return await OpenAsync(new Uri(url));
        }

        public static BookRule GetBookRule(string host)
        {
            using (var reader = SqlHelper.Select<BookRule>("WHERE Host = @host LIMIT 1", new SqliteParameter("@host", host)))
            {
                reader.Read();
                if (reader.HasRows)
                {
                    return new BookRule(reader);
                }
            }
            return null;
        }

        /// <summary>
        /// 字符串切分成行
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] GetLines(string content)
        {
            var r = content.IndexOf("\r");
            var n = content.IndexOf("\n");
            if (r > 0 && n > 0)
            {
                content = content.Replace("\r", "");
            }
            else if (r > 0)
            {
                content = content.Replace("\r", "\n");
            }
            return content.Split('\n');
        }

        public static string HtmlToText(string html)
        {
            html = Regex.Replace(html, @"\s+", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!--[\s\S]*-->", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(script|style)[^>]*?>.*?</\1>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(br|p)[^>]*>", "\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<[^>]*>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(quot|#34);", "/", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&#\d+;", "", RegexOptions.IgnoreCase);
            return html.Replace("\\n", "\n")
                .Replace("\\xa1", "\xa1")
                .Replace("\\xa2", "\xa2")
                .Replace("\\xa3", "\xa3")
                .Replace("\\xa9", "\xa9");
        }

        public static async Task<bool> SaveAsync(Book book)
        {
            var file = await StorageHelper.GetSaveFileAsync(book.Name);
            if (file == null)
            {
                return false;
            }
            await SqlHelper.Conn.OpenAsync();
            var result = await SaveAsync(book, file);
            SqlHelper.Conn.Close();
            return result;
        }

        public static async Task<bool> SaveAsync(Book book, StorageFile file)
        {
            CachedFileManager.DeferUpdates(file);
            using (var reader = SqlHelper.Select<BookChapter>("Name, Content", "WHERE BookId = @id ORDER BY Position ASC, Id ASC", new SqliteParameter("@id", book.Id)))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        await FileIO.AppendTextAsync(file, reader.GetString(0) + "\r\n\r\n", Windows.Storage.Streams.UnicodeEncoding.Utf8);
                        if (!reader.IsDBNull(1))
                        {
                            await FileIO.AppendTextAsync(file, reader.GetString(1) + "\r\n\r\n", Windows.Storage.Streams.UnicodeEncoding.Utf8);
                        }
                    }
                }
            }
            var status = await CachedFileManager.CompleteUpdatesAsync(file);
            return status == FileUpdateStatus.Complete;
        }

        public static FontFamily GetFont(string font) 
        {
            if (font == "方正启体简体") 
            {
                return new FontFamily("ms-appx:///Assets/Fonts/方正启体简体.TTF#方正启体简体");
            }
            if (font == "华康少女") 
            {
                return new FontFamily("ms-appx:/Assets/Fonts/Myuppy.ttf#MYuppy DDC");
            }
            return new FontFamily(font);
        }
    }
}
