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
        string[] Colors = { "Blue", "Red", "Black" };
        int[] Sizes = { 26, 32 };
        List<TextBox> CommandInputs = new List<TextBox>();
        Dictionary<string, System.Windows.Media.SolidColorBrush> ColorsDictionary=new Dictionary<string, SolidColorBrush>();
        Dictionary<string, List<TextBox>> Inputs=new Dictionary<string, List<TextBox>>();
        public MainWindow()
        {

            InitializeComponent();

            Console.WriteLine("heeeho");
            
            ColorsDictionary.Add("Blue",System.Windows.Media.Brushes.Blue);
            ColorsDictionary.Add("Red", System.Windows.Media.Brushes.Red);
            ColorsDictionary.Add("Black", System.Windows.Media.Brushes.Black);

            //foreach(Model model in Models)
            //{
            //    foreach(int size in Sizes)
            //    {
            //        foreach(string color in Colors)
            //        {
            //            string[] temp_textbox_info = { model.Name, size.ToString(), color };
            //            Inputs.Add(temp_textbox_info, new List<TextBox>());
            //        }
            //    }
            //}

            foreach (Model model in Models)
            {
                Inputs.Add(model.Name, new List<TextBox>());
            }
                // Create Columns


                GridOrder.VerticalAlignment = VerticalAlignment.Top;
            GridOrder.Background = Brushes.White;
            GridOrder.HorizontalAlignment = HorizontalAlignment.Left;

            //GridOrder.ShowGridLines = true;

            GridOrder.ColumnDefinitions.Add(new ColumnDefinition());
            foreach (String s in Colors)
            {
                ColumnDefinition cl = new ColumnDefinition();
                cl.Width = new GridLength(200);
                GridOrder.ColumnDefinitions.Add(cl);
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
                    gridRow1.Height = new GridLength(60);
                    GridOrder.RowDefinitions.Add(gridRow1);
                }
            }

            

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
                    txt.Name = $"{Colors[i]}z{Sizes[j % 2]}";
                    Grid.SetColumn(txt, i+1);
                    Grid.SetRow(txt, j+1);
                    GridOrder.Children.Add(txt);
                    Inputs[Models[j/Sizes.Length].Name].Add(txt);
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
                txtb.TextAlignment = TextAlignment.Center;
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
            int nb_velo = 0;
            string text = "";
            foreach(Model model in Models)
            {
                foreach(TextBox txt in Inputs[model.Name])
                {
                    if(txt.Text.Length != 0)
                    {
                        Total += int.Parse(txt.Text)*model.Price;
                        nb_velo+= int.Parse(txt.Text);
                        text+=model.Name+" "+txt.Name.Split("z")[0]+" "+txt.Name.Split("z")[1] + ": "+model.Price+" * "+txt.Text+" = "+ (int.Parse(txt.Text) * model.Price).ToString()+"€"+Environment.NewLine;
                    }
                    txt.Text = "";
                }
            }
            //Summary.TextWrapping = TextWrapping.Wrap;
            text+="Total: "+Total.ToString()+"€"+Environment.NewLine;
            Summary.Text = text;

            EstimationDate.Content = "Livraison dans "+(nb_velo*2+3).ToString()+" jours ouvrables";
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
