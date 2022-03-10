﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Main.Core;
using System.Diagnostics;

namespace Main.MWM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        //Commandes menu
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand ProductionViewCommand { get; set; }

        //Vues
        public SalesViewModel SalesVM { get; set; }
        public ProductionViewModel ProductionVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            SalesVM = new SalesViewModel();

            CurrentView = SalesVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = SalesVM;
            });
        }
    }
}