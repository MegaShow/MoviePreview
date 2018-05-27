﻿using System;

using MoviePreview.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using MoviePreview.Models;
using MoviePreview.Services;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;

namespace MoviePreview.Views
{
    public sealed partial class MovieDetailPage : Page
    {
        private MovieDetailViewModel ViewModel {
            get { return DataContext as MovieDetailViewModel; }
        }

        public MovieDetailPage()
        {
            InitializeComponent();
            ViewModel.Initialize(gridView);
        }

        private void AddData(MovieItemDetail m)
        {
            TimeAPIService.CurrentDetail = m;
            ViewModel.AddData();
            ChangeBG();
        }

        private void ChangeBG()
        {
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = ViewModel.MovieDetail.Images[0].ImageUri,
                Stretch = Stretch.UniformToFill,
                AlignmentY = AlignmentY.Top,
            };
            TopPanel.Background = imageBrush;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                // 滚动到之前的地方
                ThreadPoolTimer DelayTimer = ThreadPoolTimer.CreateTimer(async (source) =>
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                                              {
                                                  var transform = gridView.TransformToVisual(ScrollPage);
                                                  var point = transform.TransformPoint(new Point(0, 0));
                                                  if (point.Y != 0)
                                                  {
                                                      var y = point.Y + ScrollPage.VerticalOffset;
                                                      if (y < 200) y += 200;
                                                      ScrollPage.ChangeView(null, y - 200, null, true);
                                                  }
                                              });

                    }, TimeSpan.FromSeconds(0.1));
                ChangeBG();
                MovieImage.Source = TimeAPIService.CurrentDetail.ImageUri;
                await ViewModel.LoadAnimationAsync();
            }
            else
            {
                AddData(e.Parameter as MovieItemDetail);
                ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Image");
                if (imageAnimation != null)
                {
                    MovieImage.Opacity = 0;
                    MovieImage.Source = new BitmapImage(new Uri((e.Parameter as MovieItemDetail).Image));
                    // Wait for image opened. In future Insider Preview releases, this won't be necessary.
                    MovieImage.ImageOpened += (sender_, e_) =>
                    {
                        MovieImage.Opacity = 1;
                        imageAnimation.TryStart(MovieImage);
                    };
                }
            }
        }

        private void BGRectangle_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            EnterStoryboard.Begin();
        }

        private void BGRectangle_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ExitStoryboard.Begin();
        }
    }
}
