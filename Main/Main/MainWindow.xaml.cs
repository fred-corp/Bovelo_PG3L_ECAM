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

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> Models;
        int ModelIndex;

        public MainWindow()
        {
            InitializeComponent();
            Models = new List<string>{"Adventure","City","Explorer"};
            ModelList.ItemsSource = Models;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelIndex = ModelList.SelectedIndex;
            CurrentModel.Content = Models[ModelIndex];
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    class Order
    {
        public Order(string product, int nb)
        {

        }
    }
    class Product
    {
        string name;
        int price;
        public Product(string name, int price)
        {
            this.name = name;
            this.price = price;
        }

        public int getPrice(int nb)
        {
            return nb * price;
        }
    }
}
