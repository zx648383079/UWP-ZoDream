using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace ZoDream.Controls
{
    public sealed partial class PageControl : UserControl
    {
        CanvasRenderTarget renderTarget;
        Pager pager;

        public PageControl()
        {
            this.InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set {
                SetValue(TextProperty, value);
                pager = new Pager(value);
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PageControl), new PropertyMetadata(""));
        
        /// <summary>
        /// 行间距
        /// </summary>
        public double LineSpacing
        {
            get { return (double)GetValue(LineSpacingProperty); }
            set { SetValue(LineSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineSpacingProperty =
            DependencyProperty.Register("LineSpacing", typeof(double), typeof(PageControl), new PropertyMetadata(5.0));

        /// <summary>
        /// 字间距
        /// </summary>
        public double LetterSpacing
        {
            get { return (double)GetValue(LetterSpacingProperty); }
            set { SetValue(LetterSpacingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LetterSpacing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LetterSpacingProperty =
            DependencyProperty.Register("LetterSpacing", typeof(double), typeof(PageControl), new PropertyMetadata(5.0));



        public int Page
        {
            get { return (int)GetValue(PageProperty); }
            set {
                if (value < 1)
                {
                    value = 1;
                }
                SetValue(PageProperty, value);
                applyPage();
            }
        }

        // Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register("Page", typeof(int), typeof(PageControl), new PropertyMetadata(1));




        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            CanvasControl.RemoveFromVisualTree();
            CanvasControl = null;
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (renderTarget == null)
            {
                return;
            }
            args.DrawingSession.DrawImage(renderTarget);
        }

        private void CanvasControl_CreateResources(CanvasControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(PreparePage(sender).AsAsyncAction());
        }

        private async Task PreparePage(CanvasControl sender)
        {
            
        }

        public void Go(int page = 1)
        {
            Page = page;
        }

        private void applyPage()
        {
            if (pager == null)
            {
                return;
            }
            var fontWidth = FontSize + LetterSpacing;
            var fontHeight = FontSize + LineSpacing;
            var line = Math.Floor(CanvasControl.ActualWidth / fontWidth);
            var count = Math.Floor(CanvasControl.ActualHeight / fontHeight);
            var lines = pager.GetPage(Page, (int)line, (int)count);
            renderTarget = new CanvasRenderTarget(CanvasControl, (float)CanvasControl.ActualWidth,
                (float)CanvasControl.ActualHeight, 96);
            var font = new CanvasTextFormat();
            font.FontFamily = FontFamily.ToString();
            font.FontSize = (float)FontSize;
            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(ColorHelper.FromArgb(255, 255, 255, 255));
                var lineIndex = -1;
                foreach (var item in lines)
                {
                    lineIndex++;
                    for (int i = 0; i < item.Lnegth; i++)
                    {
                        ds.DrawText(Text[item.Start + i].ToString(), new Vector2((float)((i + (item.IsNew ? 2 : 0)) * fontWidth),
                            (float)(lineIndex * fontHeight)), ColorHelper.FromArgb(255, 0, 0, 0), font);
                    }
                }
            }
            CanvasControl.Invalidate();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            applyPage();
        }
    }
}
