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
using System.Text.RegularExpressions;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> Models;
        int ModelIndex;
        List<Order> Orders=new List<Order>();
        Dictionary<string, Product> Products = new Dictionary<string, Product>();
        List<System.Windows.Controls.TextBox> TextBoxes = new List<TextBox>(); 

        public MainWindow()
        {
            InitializeComponent();
            DBConn db = new DBConn();
            Console.WriteLine("got to init!");
            db.testDB();
            Models = new List<string>{"Adventure","City","Explorer"};
            ModelList.ItemsSource = Models;
            Products.Add("Explorer", new Product("Explrorer", 100));
            Products.Add("City", new Product("City", 1000));
            Products.Add("Adventure", new Product("Adventure", 500));
            TextBoxes.Add(In_Size1Color1);
            TextBoxes.Add(In_Size1Color2);
            TextBoxes.Add(In_Size1Color3);
            TextBoxes.Add(In_Size2Color1);
            TextBoxes.Add(In_Size2Color2);
            TextBoxes.Add(In_Size2Color3);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelIndex = ModelList.SelectedIndex;
            CurrentModel.Content = Models[ModelIndex];
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int total = 0;
            string test;
            int nb = 0;
            foreach(TextBox textbox in TextBoxes)
            {
                test = textbox.Text;
                nb =Int32.Parse(test);
                total += Products[Models[ModelIndex]].getPrice(nb);
            }

            Display_Total.Content = total;
        }

        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
    class Order
    {
        Product product;
        int nb;
        public Order(Product product, int nb)
        {
            this.product = product;
            this.nb = nb;
        }

        public override string ToString()
        {
            return product.name.ToString() + " " + nb.ToString();
        }
    }
    class Product
    {
        //private string name;
        private int price;
        public Product(string name, int price)
        {
            this.name = name;
            this.price = price;
        }

        public int getPrice(int nb)
        {
            return nb * price;
        }
        public string name { get; set; }
    }
}
