using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ZoDream.Layout
{
    public sealed partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            this.InitializeComponent();
            this.GotFocus += new RoutedEventHandler(SearchBox_GotFocus);
            this.LostFocus += new RoutedEventHandler(SearchBox_LostFocus);
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBlock.Visibility = Visibility.Visible;
            SearchTextBox.Opacity = 0;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBlock.Visibility = Visibility.Collapsed;
            SearchTextBox.Opacity = 1;
            //SearchTextBox.Focus(FocusState.Programmatic);
        }

        public string Title
        {
            get { return (string)GetValue(TiltleProperty); }
            set {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = string.IsNullOrWhiteSpace(Source) ? "搜索或浏览" : Source;
                }
                SetValue(TiltleProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Tiltle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TiltleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SearchBox), new PropertyMetadata(null));



        public InputScope InputScope
        {
            get { return (InputScope)GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputScope.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputScopeProperty =
            DependencyProperty.Register("InputScope", typeof(InputScope), typeof(SearchBox), new PropertyMetadata(InputScopeNameValue.Default));



        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(SearchBox), new PropertyMetadata(null));

        
        public event EventHandler<EnterEventArgs> OnEnter;

        private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && OnEnter != null)
            {
                OnEnter(this, new EnterEventArgs(SearchTextBox.Text));
            }
        }
    }

    public class EnterEventArgs: EventArgs
    {
        public EnterEventArgs(string source)
        {
            Source = source;
        }

        public string Source { get; private set; }
    }
}
