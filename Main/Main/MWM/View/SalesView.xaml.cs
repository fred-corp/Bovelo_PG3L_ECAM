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

namespace Main.MWM.View
{
    /// <summary>
    /// Interaction logic for SalesView.xaml
    /// </summary>
    public partial class SalesView : UserControl
    {
        List<int> _Sizes = new List<int>();
        List<string> _Models = new List<string>();
        List<string> _Colors = new List<string>();
        List<TabItem> _ModelsTab = new List<TabItem>();
        public SalesView()
        {
            InitializeComponent();
            _Sizes.Add(26);
            _Sizes.Add(28);
            _Colors.Add("Red");
            _Colors.Add("Blue");
            _Colors.Add("Black");
            _Models.Add("City");
            _Models.Add("Adventure");

            foreach (var model in _Models)
            {
                TabItem tab = new TabItem();
                tab.Name = model;
                tab.Header = model;
                tab.Height = 75;
                tab.Content = GetGrid(model);
                _ModelsTab.Add(tab);
                MainTabControl.Items.Add(tab);
            }
        }

        private Grid GetGrid(string model)
        {
            Grid grid = new Grid();
            grid.Name = "Grid" + model;
            ColumnDefinition Column1 = new ColumnDefinition();
            ColumnDefinition Column2 = new ColumnDefinition();
            RowDefinition Row1 = new RowDefinition();
            RowDefinition Row2 = new RowDefinition();

            grid.ColumnDefinitions.Add(Column1);
            grid.ColumnDefinitions.Add(Column2);

            grid.RowDefinitions.Add(Row1);
            grid.RowDefinitions.Add(Row2);

            Image image = new Image();
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);

            TextBox textBox = new TextBox();
            textBox.Text = model;
            Grid.SetRow(textBox, 1);
            Grid.SetColumn(textBox, 0);
            grid.Children.Add(textBox);


            Grid CommandGrid = new Grid();
            CommandGrid.Name = "CommandGrid";
            RowDefinition CommandRow1 = new RowDefinition();
            RowDefinition CommandRow2 = new RowDefinition();
            RowDefinition CommandRow3 = new RowDefinition();
            RowDefinition CommandRow4 = new RowDefinition();
            CommandGrid.RowDefinitions.Add(CommandRow1);
            CommandGrid.RowDefinitions.Add(CommandRow2);
            CommandGrid.RowDefinitions.Add(CommandRow3);
            CommandGrid.RowDefinitions.Add(CommandRow4);

            Grid.SetRow(CommandGrid, 1);
            Grid.SetColumn(CommandGrid, 1);
            grid.Children.Add(CommandGrid);

            TextBlock TextBlock = new TextBlock();
            TextBlock.Text = "Commande";

            Grid.SetRow(TextBlock, 0);
            Grid.SetColumn(TextBlock, 0);
            CommandGrid.Children.Add(TextBlock);

            StackPanel TextstackPanel = new StackPanel();
            TextstackPanel.Orientation = Orientation.Horizontal;
            string[] Texts = { "Color", "Size", "Number" };
            foreach (string s in Texts)
            {
                Label text = new Label();
                text.Content = s;
                TextstackPanel.Children.Add(text);
            }

            Grid.SetRow(TextstackPanel, 1);
            Grid.SetColumn(TextstackPanel, 0);
            CommandGrid.Children.Add(TextstackPanel);



            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            ComboBox ColorcomboBox = new ComboBox();
            ComboBox SizecomboBox = new ComboBox();
            foreach (string color in _Colors)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = color;
                ColorcomboBox.Items.Add(item);
            }
            foreach (int size in _Sizes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = size;
                SizecomboBox.Items.Add(item);
            }
            stackPanel.Children.Add(ColorcomboBox);
            stackPanel.Children.Add(SizecomboBox);
            
            TextBox TextBox2 = new TextBox();
            //TextBox2.AddHandler(TextBox.PreviewTextInput, checkInput);
            TextBox2.PreviewTextInput += checkInput;
            stackPanel.Children.Add(TextBox2);

            Button Confirm = new Button();
            Confirm.Content = "Confirm";
            Confirm.Name = model;
            Confirm.Click += ConfirmOrder;
            stackPanel.Children.Add(Confirm);

            Grid.SetRow(stackPanel, 2);
            Grid.SetColumn(stackPanel, 0);
            CommandGrid.Children.Add(stackPanel);


            return grid;
        }

        private void checkInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ConfirmOrder(object sender, RoutedEventArgs e)
        {
            Button test2 = (Button)sender;
            StackPanel ParentStack = (StackPanel)test2.Parent;
            Grid ParentGrid = (Grid)ParentStack.Parent;
            Label text = new Label();
            text.Content = "Added to cart";
            Grid.SetRow(text, 3);
            Grid.SetColumn(text, 0);
            ParentGrid.Children.Add(text);
        }

        //private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    TextBox dynamictextbox = new TextBox();
        //    dynamictextbox.Text = "(Enter some text)";
        //    dynamictextbox.Name = "dynamictextbox";
        //    (TabControl)sender.Items.Add(dynamictextbox);
        //}
    }
}
