using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Main.MWM.View
{
    /// <summary>
    /// Interaction logic for StockView.xaml
    /// </summary>
    public partial class StockView : UserControl
    {
        public StockView()
        {
            InitializeComponent();
        }

        private void AddPart(object sender, RoutedEventArgs e)
        {
            addPartPopup.IsOpen = true;
        }
        private void CancelPart(object sender, RoutedEventArgs e)
        {
            addPartPopup.IsOpen = false;
        }
    }
}
