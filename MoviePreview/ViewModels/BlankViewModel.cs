﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using MoviePreview.Models;
using MoviePreview.Services;

namespace MoviePreview.ViewModels
{
    public class BlankViewModel : ViewModelBase
    {
        public BlankViewModel()
        {
        }
        public ObservableCollection<MovieItemComing> MovieItems { get; private set; } = new ObservableCollection<MovieItemComing>();
        public Boolean EmptyItem {
            get {
                return MovieItems.Count == 0;
            }
        }
        public async Task LoadData()
        {
            if (MovieItems.Count == 0)
            {
                MovieItems.Clear();
                var data = await TimeAPIService.GetComingMovies();
                foreach (var movie in data)
                {
                    if (movie.Image != "")
                    {
                        MovieItems.Add(movie);
                    }
                }
                RaisePropertyChanged("EmptyItem");
                // TODO 加入磁贴 即将上映
                // Singleton<LiveTileService>.Instance.AddTileToQueue("最新上映", MovieItems[0].TitleCn, MovieItems[0].TitleEn);
            }
        }
    }
}
