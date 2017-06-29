using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using ZoDream.Helper;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ZoDream.Layout
{
    public sealed partial class PageView : UserControl
    {
        public PageView()
        {
            InitializeComponent();
            SizeChanged += PageView_SizeChanged;
        }

        private List<PageItem> _blocks = new List<PageItem>();
        

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
            SetSize(e.NewSize.Width / PageCount, e.NewSize.Height - 20);
        }
        /// <summary>
        /// 更新页面尺寸
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetSize(double width, double height)
        {
            _width = width;
            _height = height;
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
                var block = new PageItem(MainBlock);
                block.SetSize(_width, _height);
                block.SetX(_width).Index = 1;
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
            MainBg.Background = Background;
            MainBlock.UpdateLayout();
            if (!MainBlock.HasOverflowContent)
            {
                return 0;
            }
            PageItem block;
            for (int i = 0,length = _blocks.Count; i < length; i++)
            {
                block = _blocks[i];
                block.SetSize(_width, _height);
                block.SetX((i + 1) + _width);
                block.Update();
                if (!block.HasOver())
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// 创建元素
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private PageItem _createBlock(PageItem target, int index)
        {
            var block = new PageItem(target);
            block.SetSize(_width, _height);
            block.SetX((index + 1) + _width).Index = index + 2;
            _addBlock(block);
            block.Update();
            return block;
        }

        private void _addBlock(PageItem block)
        {
            _blocks.Add(block.SetBackground(Background));
            PagerBox.Children.Add(block.Bg);
        }

        /// <summary>
        /// 从最后开始添加
        /// </summary>
        private void _addLast()
        {
            if (_blocks.Count < 1)
            {
                return;
            }
            var index = _blocks.Count - 1;
            var block = _blocks[index];
            block.Update();
            var hasOver = block.HasOver();
            while (hasOver)
            {
                block = _createBlock(block, index);
                hasOver = block.HasOver();
            }
        }

        private void _removeBlock(int index)
        {
            if (index >= Count)
            {
                return;
            }
            _blocks.RemoveAt(index - 1);
            PagerBox.Children.RemoveAt(index);
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
            AddContent(BookHelper.HtmlToText(html));
        }


        /// <summary>
        /// 添加文本内容
        /// </summary>
        /// <param name="content"></param>
        public void AddContent(string content)
        {

            string[] lines = BookHelper.GetLines(content);
            Paragraph ph = new Paragraph();
            Run textRun;
            foreach (string item in lines)
            {
                textRun = new Run()
                {
                    Text = item
                };
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
            get { return Convert.ToDouble(GetValue(LineHeightProperty)); }
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
            if (index == Index)
            {
                return;
            }
            if (index < 0)
            {
                OnPreviousPage?.Invoke(this, new EventArgs());
                return;
            }
            
            if (index >= Count)
            {
                OnNextPage?.Invoke(this, new EventArgs());
                return;
            }
            GoIndex(index);
        }

        public void GoIndex(int index)
        {
            Index = index;
            for (int i = _blocks.Count - 1; i >= 0 ; i--)
            {
                _blocks[i].SetX(i < index ? 0 : (i + 1 - index) * _width);
            }
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
            SetProperty();
            var progress = Convert.ToDouble(Index) / Convert.ToDouble(Count);
            _loadBlock();
            GoIndex(Convert.ToInt32(Math.Floor(progress * Count)));
            _loading(false);
        }

        /// <summary>
        /// 设置属性
        /// </summary>
        public void SetProperty()
        {
            SetBlockProperty();
            SetBackground();
        }

        public void SetBlockProperty()
        {
            foreach (var item in MainBlock.Blocks)
            {
                item.FontFamily = FontFamily;
                item.FontSize = FontSize;
                if (LineHeight > 0)
                {
                    item.LineHeight = LineHeight;
                }
                item.CharacterSpacing = CharacterSpacing;
                item.FontWeight = FontWeight;
                item.Foreground = Foreground;
            }
        }

        public void SetBackground()
        {
            MainBg.Background = Background;
            foreach (var item in _blocks)
            {
                item.SetBackground(Background);
            }
        }

        /// <summary>
        /// 清除页面
        /// </summary>
        public void Clear()
        {
            MainBlock.Blocks.Clear();
            for (int i = _blocks.Count; i > 0; i--)
            {
                PagerBox.Children.RemoveAt(i);
            }
            _blocks.Clear();
            GoIndex(0);
        }
        

        private int GetInt(double arg)
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

    public class PageItem
    {
        private int _index;

        public int Index
        {
            get { return _index; }
            set {
                _index = value;
                Bg.SetValue(Canvas.ZIndexProperty, _index + 1);
            }
        }



        public Border Bg { get; set; }

        public RichTextBlockOverflow Block { get; set; }
        /// <summary>
        /// 设置宽和高
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public PageItem SetSize(double width, double height)
        {
            Block.Width = width;
            Block.Height = height;
            return this;
        }
        /// <summary>
        /// 设置宽带
        /// </summary>
        /// <param name="width"></param>
        public PageItem SetSize(double width)
        {
            Block.Width = width;
            return this;
        }
        /// <summary>
        /// 设置left
        /// </summary>
        /// <param name="x"></param>
        public PageItem SetX(double x)
        {
            Bg.SetValue(Canvas.LeftProperty, x);
            return this;
        }
        /// <summary>
        /// 更新
        /// </summary>
        public PageItem Update()
        {
            Block.UpdateLayout();
            return this;
        }

        public PageItem SetBackground(Brush color)
        {
            Bg.Background = color;
            return this;
        }

        public bool HasOver()
        {
            return Block.HasOverflowContent;
        }
        /// <summary>
        /// 把超出部分给别人
        /// </summary>
        /// <param name="target"></param>
        public PageItem GetOverTarget(RichTextBlockOverflow target)
        {
            Block.OverflowContentTarget = target;
            return this;
        }
        /// <summary>
        /// 把超出部分给别人
        /// </summary>
        /// <param name="target"></param>
        public PageItem GetOverTarget(PageItem target)
        {
            target.SetOverTarget(Block);
            return this;
        }
        /// <summary>
        /// 从其他获取超出内容
        /// </summary>
        /// <param name="target"></param>
        public PageItem SetOverTarget(RichTextBlockOverflow target)
        {
            target.OverflowContentTarget = Block;
            return this;
        }

        /// <summary>
        /// 从其他获取超出内容
        /// </summary>
        /// <param name="target"></param>
        public PageItem SetOverTarget(RichTextBlock target)
        {
            target.OverflowContentTarget = Block;
            return this;
        }

        /// <summary>
        /// 从其他获取超出内容
        /// </summary>
        /// <param name="target"></param>
        public PageItem SetOverTarget(PageItem target)
        {
            target.GetOverTarget(Block);
            return this;
        }

        public PageItem()
        {

        }

        public PageItem(Border bg, RichTextBlockOverflow block)
        {
            Bg = bg;
            Block = block;
        }

        public PageItem(RichTextBlock block)
        {
            Bg = new Border();
            Block = new RichTextBlockOverflow();
            Bg.Child = Block;
            SetOverTarget(block);
        }

        public PageItem(RichTextBlockOverflow block)
        {
            Bg = new Border();
            Block = new RichTextBlockOverflow();
            Bg.Child = Block;
            SetOverTarget(block);
        }

        public PageItem(PageItem block)
        {
            Bg = new Border();
            Block = new RichTextBlockOverflow();
            Bg.Child = Block;
            SetOverTarget(block);
        }
    }
}
