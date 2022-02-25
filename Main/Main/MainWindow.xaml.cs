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
        
        Model[] Models = { new Model("Explorer", 1000), new Model("Adventure", 500), new Model("City", 300) };
        //String[] Models = { "Explorer", "Adventure", "City"};
        String[] Colors = { "Blue", "Red", "Black" };
        int[] Sizes = { 26, 32 };
        List<TextBox> CommandInputs = new List<TextBox>();
        Dictionary<string, System.Windows.Media.SolidColorBrush> ColorsDictionary=new Dictionary<string, SolidColorBrush>();
        Dictionary<string, List<TextBox>> = new Dictionary<string, List<TextBox>>();
        public MainWindow()
        {

            InitializeComponent();

            Console.WriteLine("heeeho");
            
            ColorsDictionary.Add("Blue",System.Windows.Media.Brushes.Blue);
            ColorsDictionary.Add("Red", System.Windows.Media.Brushes.Red);
            ColorsDictionary.Add("Black", System.Windows.Media.Brushes.Black);

            // Create Columns


            GridOrder.VerticalAlignment = VerticalAlignment.Top;
            GridOrder.Background = Brushes.White;

            //GridOrder.ShowGridLines = true;

            GridOrder.ColumnDefinitions.Add(new ColumnDefinition());
            foreach (String s in Colors)
            {
                GridOrder.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create Rows

            RowDefinition NameRow = new RowDefinition();
            NameRow.Height = new GridLength(45);
            GridOrder.RowDefinitions.Add(NameRow);

            foreach (Model s in Models)
            {
                foreach (int size in Sizes)
                {
                    RowDefinition gridRow1 = new RowDefinition();
                    gridRow1.Height = new GridLength(45);
                    GridOrder.RowDefinitions.Add(gridRow1);
                }
            }

            TextBlock txt1 = new TextBlock();
            txt1.Text = "2005 Products Shipped";
            txt1.FontSize = 20;
            txt1.FontWeight = FontWeights.Bold;

            for (int i = 0; i< Colors.Length; i++)
            {
                TextBlock txtb = new TextBlock();
                txtb.Text = Colors[i];
                txtb.TextAlignment = TextAlignment.Center;
                txtb.Foreground = Brushes.White;
                txtb.Background = ColorsDictionary[Colors[i]];
                txtb.FontSize = 20;
                txtb.FontWeight = FontWeights.Bold;
                Grid.SetColumn(txtb, i+1);
                Grid.SetRow(txtb, 0);
                GridOrder.Children.Add(txtb);

                for (int j = 0; j <= Sizes.Length+Models.Length; j++)
                {

                    TextBox txt = new TextBox();
                    
                    txt.TextAlignment = TextAlignment.Center;
                    txt.FontSize = 20;
                    txt.Name = Models[j / Sizes.Length].ToString();
                    Grid.SetColumn(txt, i+1);
                    Grid.SetRow(txt, j+1);
                    GridOrder.Children.Add(txt);

                    CommandInputs.Append(txt);
                }
            }

            int it = 0;

            for (int j = 0; j < Models.Length; j++)
            {
                for (int k = 0; k< Sizes.Length; k++)
                {
                TextBlock txtb = new TextBlock();
                txtb.Text = Models[j].ToString()+" "+Sizes[k];
                txtb.FontSize = 20;
                txtb.FontWeight = FontWeights.Bold;
                Grid.SetColumn(txtb, 0);
                Grid.SetRow(txtb, 1+it);
                GridOrder.Children.Add(txtb);
                it++;
                }
            }

        }

        private void Comfirm_Click(object sender, RoutedEventArgs e)
        {
            int Total = 0;
            foreach (TextBox txt in CommandInputs)
            {
                //Console.WriteLine(txt.Name);
                //Console.WriteLine(txt.Text);
                //Total+= 
            }
        }


        class Model
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public Model(string Name,int Price)
            {
                this.Price = Price;
                this.Name = Name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
