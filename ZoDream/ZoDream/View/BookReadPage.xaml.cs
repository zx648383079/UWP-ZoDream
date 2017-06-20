using System;
using System.Collections.Generic;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace ZoDream.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookReadPage : Page
    {
        public BookReadPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            TitleTb.Text = "";
            Pager.PageHtml = "    大漠孤烟直，长河落日圆。天  籁小说 一望无垠的大漠，空旷而高远，壮阔而雄浑，当红日西坠，地平线尽" +
                "头一片殷红，磅礴中亦有种苍凉感。上古的烽烟早已在楚风一个人在旅行，很疲惫，他躺在黄沙上，看着血色的夕阳，不知道还要多久才能离开这片大漠。数日前他毕业了，同" +
                "时也跟校园中的女神说再见，或许见不到了吧，毕离开学院后，他便出来旅行。落日很红，挂在大漠的尽头，在空旷中有一种宁静的美。楚风坐起来喝了一些水，感觉,精力13,123,13复了不少，" +
                "他的身体一路西进，他在大漠中留下一串很长、很远的脚印。无声无息，竟起雾了，这在沙漠中非常罕见。当时他并未在意。依旧宁静，沙漠中除却多了一层朦胧的蓝雾，并" +
                "没有其他变故生，楚风加快脚步，他想尽快离开这里。大漠的尽头，落日蓝的妖异，染蓝了西部的天空，不过它终究快要消失在地平线上了。天边，蓝日下沉，即将消失，雾气弥漫，" +
                "浩瀚的大漠如同披上了一层诡异的蓝色薄纱。“啵！”大量的蓝花，晶莹点点，犹若梦幻，有些醉人，遍开在沙漠中，非常不真实。沙漠干燥、缺水，只有极其稀少的耐旱植物偶尔可见，" +
                "零星散落着。而彼岸花喜欢阴森、潮湿的环境，无论如何也不该在这里出现，还如此的妖艳。一缕淡淡的芬芳飘漾，让人沉迷。天色渐暗，最后的落日余晖也已不见了。天色渐黑，他终于走" +
                "出来了，清晰的看到了山地，也隐约间看到了山脚下牧民的帐篷。再回头时，身后大漠浩瀚，很寂静，跟平日没什么两样。山地前方，灯火摇曳，离山脚下还较远时就听到了一些嘈杂声，" +
                "那里不平静，像是有什么事情正在生。此外，还有牛羊等牲畜惶恐的叫声，以及藏獒沉闷的低吼声。有异常之事吗？楚风加快脚步，赶到山脚下，临近牧民的栖居地。";
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Pager.GoBack();
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            Pager.GoForword();
        }

        private void Pager_OnNextPage(object sender, EventArgs e)
        {
            
        }

        private void Pager_OnPreviousPage(object sender, EventArgs e)
        {
            
        }

        private void Pager_OnIndexChanged(object sender, Layout.IndexEventArgs e)
        {
            PageProgress.Value = e.Index * 100 / e.Count;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SettingGrid.Visibility == Visibility.Collapsed)
            {
                SettingGrid.Visibility = Visibility.Visible;
                return;
            }
            SettingGrid.Visibility = Visibility.Visible;
        }
    }
}
