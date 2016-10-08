using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace ZoDream.Layout
{
    public class BasePage: Page
    {
        public BasePage()
        {
            this.ManipulationMode = ManipulationModes.TranslateX;

            this.ManipulationCompleted += BasePage_ManipulationCompleted;

            this.ManipulationDelta += BasePage_ManipulationDelta;

            _tt = this.RenderTransform as TranslateTransform;

            if (_tt == null)
                this.RenderTransform = _tt = new TranslateTransform();
        }

        private TranslateTransform _tt;

        private int action;
       

        private void BasePage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_tt.X + e.Delta.Translation.X < 0)
            {
                _tt.X = 0;
                return;
            }
            _tt.X += e.Delta.Translation.X;
        }

        private void BasePage_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            double abs_delta = Math.Abs(e.Cumulative.Translation.X);
            double speed = Math.Abs(e.Velocities.Linear.X);
            double delta = e.Cumulative.Translation.X;
            double to = 0;

            if (abs_delta < this.ActualWidth / 3 && speed < 0.5)
            {
                _tt.X = 0;
                return;
            }

            action = 0;
            if (delta > 0)
                to = this.ActualWidth;
            else if (delta < 0)
                return;

            var s = new Storyboard();
            var doubleanimation = new DoubleAnimation() { Duration = new Duration(TimeSpan.FromMilliseconds(120)), From = _tt.X, To = to };
            doubleanimation.Completed += Doubleanimation_Completed;
            Storyboard.SetTarget(doubleanimation, _tt);
            Storyboard.SetTargetProperty(doubleanimation, "X");
            s.Children.Add(doubleanimation);
            s.Begin();
        }

        private void Doubleanimation_Completed(object sender, object e)
        {
            if (action == 0)
            {

            }
                //MasterPage.BackRequest(sender);
            else
            {
                //MasterPage.MainEntryTapped((PageID + action + 3) % 3);
            }

            _tt = this.RenderTransform as TranslateTransform;
            if (_tt == null) this.RenderTransform = _tt = new TranslateTransform();
            _tt.X = 0;
        }
    }
}
