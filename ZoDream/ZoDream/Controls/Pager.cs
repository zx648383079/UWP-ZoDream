using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Helpers;

namespace ZoDream.Controls
{
    public class Pager
    {
        public IList<PageLine> Lines { get; set; }

        public Pager(string content)
        {
            Refresh(content);
        }

        public void Refresh(string content)
        {
            Lines = new List<PageLine>();
            int start = -1,
                isWhiteEnd = -1,
                i = 0;
            char lineChar = '\n', 
                lineChar2 = '\r', 
                whiteChar = ' ', 
                whiteChar2 = '　', 
                code;
            for (; i < content.Length; i++)
            {
                code = content[i];
                if (start < 0 && (
                    code.Equals(lineChar) || code.Equals(lineChar2) || code.Equals(whiteChar) || code.Equals(whiteChar2)
                    ))
                {
                    // 判断新一行前几个字符是否为空白或换行字符，是则去掉
                    continue;
                }
                if (start < 0)
                {
                    start = i;
                    continue;
                }
                
                if (code.Equals(lineChar) || code.Equals(lineChar2))
                {
                    // 根据换行符判断是否进入新的一行，结束则保存
                    Lines.Add(new PageLine() { Start = start, Lnegth = (isWhiteEnd > 0 ? isWhiteEnd : i) - start });
                    isWhiteEnd = start = -1;
                    continue;
                }
                if (code.Equals(whiteChar) || code.Equals(whiteChar2))
                {
                    // 判断行尾是否有空白字符，有则去掉
                    if (isWhiteEnd < 0)
                    {
                        isWhiteEnd = i;
                    }
                    continue;
                } else
                {
                    isWhiteEnd = -1;
                }
            }
            // 最后做收尾
            if (start > 0)
            {
                Lines.Add(new PageLine() { Start = start, Lnegth = (isWhiteEnd > 0 ? isWhiteEnd : (i + 1)) - start });
            }
        }

        public IList<PageLine> GetPage(int page, int line, int count)
        {
            var start = page > 1 ? (page - 1) * count : 0;
            var end = start + count;
            var lines = new List<PageLine>();
            var index = 0;
            foreach (var item in Lines)
            {
                var lineCount = item.GetLineCount(line);
                if (index < end && index + lineCount > start)
                {
                    var i = Math.Max(start - index, 0);
                    var length = Math.Min(lineCount, end - index);
                    for (; i < length; i++)
                    {
                        var lineStart = i * line - (i < 1 ? 0 : 2);
                        lines.Add(new PageLine()
                        {
                            Start = item.Start + lineStart,
                            Lnegth = Math.Min(line - (i < 1 ? 2 : 0), item.Lnegth - i * line + 2),
                            IsNew = i < 1
                        });
                    }
                }
                index += lineCount;
                if (index > end)
                {
                    continue;
                }
            }
            return lines;
        }
    }
}
