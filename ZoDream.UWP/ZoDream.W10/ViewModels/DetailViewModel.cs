using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;

using AppStudio.Uwp;
using AppStudio.DataProviders;
using AppStudio.Uwp.Actions;
using AppStudio.Uwp.Cache;
using AppStudio.Uwp.Commands;
using AppStudio.Uwp.DataSync;
using AppStudio.Uwp.Navigation;

using ZoDream.Sections;
using ZoDream.Services;
using ZoDream.Navigation;
using ZoDream.Pages;


namespace ZoDream.ViewModels
{
    public abstract class DetailViewModel : PageViewModelBase
    {
        private bool _showInfoLastValue;

        public abstract Task LoadStateAsync(NavDetailParameter detailParameter);

        public DetailViewModel(string title, string sectionName)
        {
            Title = title;
            SectionName = sectionName;

            ShowInfo = true;
            FontSize = GetFontSize();

            ZoomMode = ZoomMode.Disabled;
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                ShellPage.Current.ShellControl.OnExitFullScreen += OnExitFullScreen;
                ShellPage.Current.ShellControl.OnEnterFullScreen += OnEnterFullScreen;
            }
        }

		public ObservableCollection<ComposedItemViewModel> Items { get; protected set; } = new ObservableCollection<ComposedItemViewModel>();  

        #region SelectedItem
        private ComposedItemViewModel _selectedItem;

        public ComposedItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
				UpdateInformationPanel();
            }
        }
        #endregion

        #region ZoomMode
        private ZoomMode _zoomMode;

        public ZoomMode ZoomMode
        {
            get { return _zoomMode; }
            set { SetProperty(ref _zoomMode, value); }
        }
        #endregion
		
        #region FontSize
        private double _fontSize;

        public double FontSize
        {
            get { return _fontSize; }
            set { SetProperty(ref _fontSize, value); }
        }
        #endregion

        #region ShowInfo
        private bool _showInfo;

        public bool ShowInfo
        {
            get { return _showInfo; }
            set { SetProperty(ref _showInfo, value); }
        }
        #endregion

                

        #region SlideShowTimer
        private DispatcherTimer _slideShowTimer;

        public DispatcherTimer SlideShowTimer
        {
            get
            {
                if (_slideShowTimer == null)
                {
                    _slideShowTimer = new DispatcherTimer()
                    {
                        Interval = TimeSpan.FromMilliseconds(1500)
                    };
                    _slideShowTimer.Tick += PresentationTimeEvent;
                }
                return _slideShowTimer;
            }
        }
        #endregion

		#region Commands
		public ICommand FullScreenCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    await ShellPage.Current.ShellControl.TryEnterFullScreenAsync();
                });
            }
        }

		public ICommand ShowPresentationCommand
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    if (await ShellPage.Current.ShellControl.TryEnterFullScreenAsync())
                    {
                        ZoomMode = ZoomMode.Disabled;
                        SlideShowTimer.Start();
                    }
                });
            }
        }

		public ICommand ShowInfoCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ShowInfo = !ShowInfo;
                });
            }
        }

	
        public ICommand ApplyFontSizeCommand
        {
            get
            {
                return new RelayCommand<string>((fontSizeResourceName) =>
                {
                    FontSize = (double)fontSizeResourceName.Resource();
                    SetFontSize(FontSize);
                });
            }
        }
		#endregion

		public void ShareContent(DataRequest dataRequest, bool supportsHtml = true)
        {
            ShareContent(dataRequest, SelectedItem, supportsHtml);
        }

		public static double GetFontSize()
        {
            if (!ApplicationData.Current.LocalSettings.Values.ContainsKey("DescriptionFontSize"))
            {
                SetFontSize((double)"DescriptionTextSizeNormal".Resource());
            }
            return (double)ApplicationData.Current.LocalSettings.Values["DescriptionFontSize"];
        }

        public static void SetFontSize(double fontsize)
        {
            ApplicationData.Current.LocalSettings.Values["DescriptionFontSize"] = fontsize;
        }
        		                                

		private void UpdateInformationPanel()
        {
            if (SelectedItem != null)
            {
                if (string.IsNullOrEmpty(SelectedItem.Title) && string.IsNullOrEmpty(SelectedItem.Description))
                {
                    ShowInfo = false;
                }
            }
        }

		#region Events
		private void OnExitFullScreen(object sender, EventArgs e)
        {
            this.ShowInfo = _showInfoLastValue;
            SlideShowTimer.Stop();
            ZoomMode = ZoomMode.Disabled;
        }

        private void OnEnterFullScreen(object sender, EventArgs e)
        {
            _showInfoLastValue = this.ShowInfo;
            ShowInfo = false;
            ZoomMode = ZoomMode.Enabled;
        }

        private void PresentationTimeEvent(object sender, object e)
        {
            if (Items != null && Items.Count > 1 && SelectedItem != null)
            {
                var index = Items.IndexOf(SelectedItem);
                if (index < Items.Count - 1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
                SelectedItem = Items[index];
            }
        }
		#endregion
    }

	public class DetailViewModel<TSchema> : DetailViewModel where TSchema : SchemaBase
    {
        private Section<TSchema> _section;

        public DetailViewModel(Section<TSchema> section) : base(section.DetailPage.Title, section.Name)
        {
            _section = section;
        }

        public override async Task LoadStateAsync(NavDetailParameter detailParameter)
        {
            try
            {
                HasLoadDataErrors = false;
                IsBusy = true;

                if (detailParameter != null)
                {
                    //avoid warning
                    await Task.Run(() => { });

                    ParseItems(detailParameter.Items.OfType<TSchema>(), detailParameter.SelectedId);
                }
            }
            catch (Exception ex)
            {
                HasLoadDataErrors = true;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ParseItems(IEnumerable<TSchema> items, string selectedId)
        {
            foreach (var item in items)
            {
                var composedItem = new ComposedItemViewModel
                {
                    Id = item._id
                };

                foreach (var binding in _section.DetailPage.LayoutBindings)
                {
                    var parsedItem = new ItemViewModel
                    {
                        Id = item._id
                    };
                    binding(parsedItem, item);

                    composedItem.Add(parsedItem);
                }

                composedItem.Actions = _section.DetailPage.Actions
                                                            .Select(a => new ActionInfo
                                                            {
                                                                Command = a.Command,
                                                                CommandParameter = a.CommandParameter(item),
                                                                Style = a.Style,
                                                                Text = a.Text,
                                                                ActionType = ActionType.Primary
                                                            })
                                                            .ToList();

                Items.Add(composedItem);
            }
            if (!string.IsNullOrEmpty(selectedId))
            {
                SelectedItem = Items.FirstOrDefault(i => i.Id == selectedId);
            }
        }
    }
}
