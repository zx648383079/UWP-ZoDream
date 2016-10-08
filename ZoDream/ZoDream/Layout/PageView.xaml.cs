using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ZoDream.Layout
{
    public sealed partial class PageView : UserControl
    {
        public PageView()
        {
            this.InitializeComponent();
            this.SizeChanged += PageView_SizeChanged;
        }

        private List<RichTextBlockOverflow> _blocks = new List<RichTextBlockOverflow>();
        

        private void _loading(bool arg = true)
        {
            if (arg)
            {
                LoadingRing.Visibility = Visibility.Visible;
                LoadingRing.IsActive = true;
            } else
            {
                LoadingRing.IsActive = false;
                LoadingRing.Visibility = Visibility.Collapsed;
                
            }
        }

        private double _width;

        private double _height;

        private void PageView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _width = e.NewSize.Width / PageCount;
            _height = e.NewSize.Height - 20;
            Refresh();
        }

        private void _loadBlock()
        {
            if (_removeEmpty())
            {
                return;
            }
            if (!MainBlock.HasOverflowContent)
            {
                return;
            }
            
            if (_blocks.Count < 1)
            {
                RichTextBlockOverflow block = new RichTextBlockOverflow();
                block.Width = _width;
                block.Height = _height;
                MainBlock.OverflowContentTarget = block;
                _addBlock(block);
            }
            _addLast();
        }

        private bool _removeEmpty()
        {
            var index = _getNoOver();
            if (index == -1)
            {
                return false;
            }
            var count = _blocks.Count;
            for (int i = count; i > index; i--)
            {
                _removeBlock(i);
            }
            return count > index;
        }

        private int _getNoOver()
        {
            MainBlock.Width = _width;
            MainBlock.Height = _height;
            MainBlock.UpdateLayout();
            if (!MainBlock.HasOverflowContent)
            {
                return 0;
            }
            RichTextBlockOverflow block;
            for (int i = 0,length = _blocks.Count; i < length; i++)
            {
                block = _blocks[i];
                block.Width = _width;
                block.Height = _height;
                block.UpdateLayout();
                if (!block.HasOverflowContent)
                {
                    return i;
                }
            }
            return -1;
        }

        private RichTextBlockOverflow _createBlock(RichTextBlockOverflow target)
        {
            var block = new RichTextBlockOverflow();
            block.Width = _width;
            block.Height = _height;
            _addBlock(block);
            target.OverflowContentTarget = block;
            block.UpdateLayout();
            return block;
        }

        private void _addBlock(RichTextBlockOverflow block)
        {
            _blocks.Add(block);
            Paneler.Children.Add(block);
        }

        private void _addLast()
        {
            if (_blocks.Count < 1)
            {
                return;
            }
            var block = _blocks[_blocks.Count - 1];
            block.UpdateLayout();
            var hasOver = block.HasOverflowContent;
            while (hasOver)
            {
                block = _createBlock(block);
                hasOver = block.HasOverflowContent;
            }
        }

        private void _removeBlock(int index)
        {
            if (index >= Count)
            {
                return;
            }
            _blocks.RemoveAt(index - 1);
            Paneler.Children.RemoveAt(index);
        }



        public PageAnimation PageAnimation
        {
            get { return (PageAnimation)GetValue(PageAnimationProperty); }
            set { SetValue(PageAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageAnimationProperty =
            DependencyProperty.Register("PageAnimation", typeof(PageAnimation), typeof(PageView), new PropertyMetadata(PageAnimation.Default));




        /// <summary>
        /// 一屏的页数
        /// </summary>
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set {
                SetValue(PageCountProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PageView), new PropertyMetadata(1));



        /// <summary>
        /// 通过文本插入内容
        /// </summary>
        public string PageText
        {
            get { return (string)GetValue(PageTextProperty); }
            set {
                SetValue(PageTextProperty, value);
                AddContent(value);
            }
        }

        // Using a DependencyProperty as the backing store for PageText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageTextProperty =
            DependencyProperty.Register("PageText", typeof(string), typeof(PageView), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for PageContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageContentProperty =
            DependencyProperty.Register("PageContent", typeof(Paragraph), typeof(PageView), new PropertyMetadata(null));

        /// <summary>
        /// 通过 Paragraph 插入内容
        /// </summary>
        public Paragraph PageContent
        {
            get { return (Paragraph)GetValue(PageContentProperty); }
            set {
                SetValue(PageContentProperty, value);
                AddContent(value);
            }
        }



        public string PageHtml
        {
            get { return (string)GetValue(PageHtmlProperty); }
            set {
                SetValue(PageHtmlProperty, value);
                AddHtml(value);
            }
        }

        // Using a DependencyProperty as the backing store for PageHtml.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageHtmlProperty =
            DependencyProperty.Register("PageHtml", typeof(string), typeof(PageView), new PropertyMetadata(null));


        public void AddHtml(string html)
        {
            html = Regex.Replace(html, @"\<p[^\<\>]+\>([^\<\>]+)\</p\>|\<br/?\>", "$1zodream", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\<(stype|script)[^\<\>]+\>[^\<\>]+\</\1>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"\<[^\<\>]+?\>|\r|\n", "");
            AddContent(html.Replace("zodream", "\r\n"));
        }


        /// <summary>
        /// 添加文本内容
        /// </summary>
        /// <param name="content"></param>
        public void AddContent(string content)
        {
            string[] lines = content.Split(Environment.NewLine.ToCharArray());
            Paragraph ph = new Paragraph();
            Run textRun;
            foreach (string item in lines)
            {
                textRun = new Run();
                textRun.Text = item;
                ph.Inlines.Add(textRun);
                ph.Inlines.Add(new LineBreak());
            }
            AddContent(ph);
        }

        /// <summary>
        /// 添加 Paragraph 内容
        /// </summary>
        /// <param name="content"></param>
        public void AddContent(Paragraph content)
        {
            MainBlock.Blocks.Clear();
            MainBlock.Blocks.Add(content);
            Index = 0;
            Refresh();
        }


        /// <summary>
        /// 当前页数 0开始
        /// </summary>
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set {
                SetValue(IndexProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(PageView), new PropertyMetadata(0));




        public double LineHeight
        {
            get { return (double)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(double), typeof(PageView), new PropertyMetadata(0));


        /// <summary>
        /// 获取总数
        /// </summary>
        public int Count
        {
            get { return _blocks.Count + 1; }
        }


        /// <summary>
        /// 是否能返回
        /// </summary>
        public bool CanBack
        {
            get { return (bool)GetValue(CanBackProperty); }
            set { SetValue(CanBackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanBack.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanBackProperty =
            DependencyProperty.Register("CanBack", typeof(bool), typeof(PageView), new PropertyMetadata(false));


        /// <summary>
        /// 是否能下一页
        /// </summary>
        public bool CanForword
        {
            get { return (bool)GetValue(CanForwordProperty); }
            set { SetValue(CanForwordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanForword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanForwordProperty =
            DependencyProperty.Register("CanForword", typeof(bool), typeof(PageView), new PropertyMetadata(false));

        /// <summary>
        /// 到了最后一页并请求下一页
        /// </summary>
        public event EventHandler OnNextPage;

        /// <summary>
        /// 到了第一页请求上一页
        /// </summary>
        public event EventHandler OnPreviousPage;

        /// <summary>
        /// 页数改变时发生
        /// </summary>
        public event EventHandler<IndexEventArgs> OnIndexChanged;

        /// <summary>
        /// 上一页
        /// </summary>
        public void GoBack()
        {
            Go(Index - 1);
        }

        /// <summary>
        /// 上一屏
        /// </summary>
        public void GoBackPage()
        {
            Go(Index - PageCount);
        }
        /// <summary>
        /// 下一页
        /// </summary>
        public void GoForword()
        {
            Go(Index + 1);
        }
        /// <summary>
        /// 下一屏
        /// </summary>
        public void GoForwordPage()
        {
            Go(Index + PageCount);
        }

        /// <summary>
        /// 滚动到第几页
        /// </summary>
        /// <param name="index"></param>
        public void Go(int index)
        {
            Debug.WriteLine(index);
            Debug.WriteLine(Index);
            if (index == Index)
            {
                return;
            }
            if (index < 0)
            {
                OnPreviousPage?.Invoke(this, new EventArgs());
                return;
            }
            GoIndex(index);
            if (index >= Count)
            {
                OnNextPage?.Invoke(this, new EventArgs());
                return;
            }
        }

        public void GoIndex(int index)
        {
            Index = index;
            Scroller.ChangeView(index * _width, null, null, PageAnimation == PageAnimation.None);
            OnIndexChanged?.Invoke(this, new IndexEventArgs(index, Count));
            if (Index < Count - 1)
            {
                return;
            }
            _addLast();
        }

        /// <summary>
        /// 刷新页面
        /// </summary>
        public void Refresh()
        {
            _loading(true);
            foreach (var item in MainBlock.Blocks)
            {
                item.FontFamily = FontFamily;
                item.FontSize = FontSize;
                if (LineHeight > 0)
                {
                    item.LineHeight = LineHeight;
                }
                item.CharacterSpacing = CharacterSpacing;
                item.FontWeight = this.FontWeight;
                item.Foreground = this.Foreground;
            }
            var progress = Convert.ToDouble(Index) / Convert.ToDouble(Count);
            _loadBlock();
            GoIndex(Convert.ToInt32(Math.Floor(progress * Count)));
            _loading(false);
        }

        /// <summary>
        /// 清除页面
        /// </summary>
        public void Clear()
        {
            MainBlock.Blocks.Clear();
            for (int i = _blocks.Count; i > 0; i--)
            {
                Paneler.Children.RemoveAt(i);
            }
            _blocks.Clear();
            GoIndex(0);
        }
        

        private int getInt(double arg)
        {
            var i = Convert.ToInt32(arg);
            var j = arg - i;
            if (i > 0.3)
            {
                return i + 1;
            }
            if (i < -0.3)
            {
                return i - 1;
            }
            return i;
        }
    }

    public enum PageAnimation
    {
        None,
        Default
    }

    public class IndexEventArgs : EventArgs
    {
        public IndexEventArgs(int index, int count)
        {
            Index = index;
            Count = count;
        }
        public int Index { get; private set; }

        public int Count { get; private set; }
    }
}
