using System;
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
        public RelayCommand SalesViewCommand { get; set; }
        public RelayCommand ProductionViewCommand { get; set; }
        public RelayCommand StockViewCommand { get; set; }

        //Vues
        public SalesViewModel SalesVM { get; set; }
        public ProductionViewModel ProductionVM { get; set; }
        public StockViewModel StockVM { get; set; }

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

            ProductionVM = new ProductionViewModel();

            StockVM = new StockViewModel();

            CurrentView = SalesVM;

            SalesViewCommand = new RelayCommand(o =>
            {
                CurrentView = SalesVM;
            });

            ProductionViewCommand = new RelayCommand(o =>
            {
                CurrentView = ProductionVM;
            });

            StockViewCommand = new RelayCommand(o =>
            {
                CurrentView = StockVM;
            });
        }
    }
}
