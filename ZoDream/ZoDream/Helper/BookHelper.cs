using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Model;
using ZoDream.Services;

namespace ZoDream.Helper
{
    class BookHelper
    {

        const string Pattern = @"^\s{0,6}(正文)?第?\s*[0-9一二三四五六七八九十百千]{1,10}[章回|节|卷|集|幕|计]?[\s\S]{0,20}$";

        public static async Task<List<BookChapter>> GetBookChapterAsync(string url, BookRule rule)
        {
            var html = await Http.Get(url);
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
                var chapter = await GetChapterAsync(Http.GetAbsolute(baseUrl, match.Groups["href"].Value), rule);
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
                var chapter = await GetChapterAsync(Http.GetAbsolute(book.Url, match.Groups["href"].Value), rule);
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
            var html = await Http.Get(url);
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
            var content = await StorageHelper.GetFileTextAsync(file);
            GetBookChapter(GetLines(content), book);
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
            content.Append(lines[start]);
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
                if (Regex.IsMatch(line, Pattern))
                {
                    return index;
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
        public static async Task<Book> OpenAsync()
        {
            var file = await StorageHelper.OpenFile(new List<string>() { ".txt" });
            if (file == null)
            {
                return null;
            }
            var book = GetBook(file);
            await SqlHelper.Conn.OpenAsync();
            book.Save();
            await GetBookChapterAsync(file, book);
            SqlHelper.Conn.Close();
            return book;
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
            var html = await Http.Get(url);
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
    }
}
