using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;
using ZoDream.Reader.Model;

namespace ZoDream.Reader.View
{
    public sealed partial class ReadPage : Page
    {
        public ReadPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BookName.Text = (e.Parameter as BookItem).Name;
        }

        private void _addView(string content)
        {
            //Regex regex = new Regex(@"(\n\r|\r|\n)+?");
            string[] lines = content.Split(Environment.NewLine.ToCharArray());
            ReadView.Items.Clear();
            RichTextBlock tbContent = new RichTextBlock();
            tbContent.TextWrapping = TextWrapping.Wrap;
            Paragraph ph = new Paragraph();
            Run textRun;
            foreach (string item in lines)
            {
                textRun = new Run();
                textRun.Text = item;
                ph.Inlines.Add(textRun);
                ph.Inlines.Add(new LineBreak());
            }
            tbContent.Blocks.Add(ph);
            ReadView.Items.Add(tbContent);
            // 更新一下状态，方便获取是否有溢出的文本  
            tbContent.UpdateLayout();
            bool isflow = tbContent.HasOverflowContent;
            // 因为除了第一个文本块是RichTextBlock，  
            // 后面的都是RichTextBlockOverflow一个一个接起来的  
            // 所以我们需要两个变量  
            RichTextBlockOverflow oldFlow = null, newFlow = null;
            if (isflow)
            {
                oldFlow = new RichTextBlockOverflow();
                tbContent.OverflowContentTarget = oldFlow;
                ReadView.Items.Add(oldFlow);
                oldFlow.UpdateLayout();
                // 继续判断是否还有溢出  
                isflow = oldFlow.HasOverflowContent;
            }
            while (isflow)
            {
                newFlow = new RichTextBlockOverflow();
                oldFlow.OverflowContentTarget = newFlow;
                ReadView.Items.Add(newFlow);
                newFlow.UpdateLayout();
                // 继续判断是否还有溢出的文本  
                isflow = newFlow.HasOverflowContent;
                // 当枪一个变量填充了文本后，  
                // 把第一个变量的引用指向当前RichTextBlockOverflow  
                // 确保OverflowContentTarget属性可以前后相接  
                oldFlow = newFlow;
            }
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            _getFile();
        }

        private async void _getFile()
        {
            FileOpenPicker openFile = new FileOpenPicker();
            openFile.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openFile.ViewMode = PickerViewMode.List;
            openFile.FileTypeFilter.Add(".txt");
            StorageFile file = await openFile.PickSingleFileAsync();
            if (file != null)
            {
                string text = await FileIO.ReadTextAsync(file);
                _addView(text);

            }
        }
    }
}
