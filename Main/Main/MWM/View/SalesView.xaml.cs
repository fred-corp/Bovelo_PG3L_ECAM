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
        Grid size_grid;
        Cart cart;

        Dictionary<string, int> _ID_creator = new Dictionary<string, int>();

        public SalesView()
        {
            InitializeComponent();
            _Sizes.Add(26);
            _Sizes.Add(28);
            _Colors.Add("Red");
            _Colors.Add("Blue");
            _Colors.Add("Black");
            _Models.Add("City");
            _Models.Add("Explorer");
            _Models.Add("Adventure");

            _ID_creator.Add("City", 1);
            _ID_creator.Add("Explorer", 2);
            _ID_creator.Add("Adventure", 3);
            _ID_creator.Add("26", 0);
            _ID_creator.Add("28", 1);
            _ID_creator.Add("Blue", 1);
            _ID_creator.Add("Red", 2);
            _ID_creator.Add("Black", 3);

            cart = new Cart();

            foreach (var model in _Models)
            {
                TabItem tab = new TabItem();
                tab.Name = model;
                tab.Header = model;
                tab.Height = 75;
                tab.Content = GetGrid(model);
                _ModelsTab.Add(tab);
                MainTabControl.Items.Add(tab);
                size_grid =(Grid) tab.Content;
            }
            TabItem carttab = new TabItem();
            carttab.Name = "Cart";
            carttab.Header = "Cart";
            carttab.Height = 75;
            carttab.Content = GetCartGrid(size_grid.ActualHeight,size_grid.ActualWidth);
            MainTabControl.Items.Add(carttab);
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

            Grid TextstackPanel = new Grid();
            for (int i = 0; i < 4; i++)
            {
                TextstackPanel.ColumnDefinitions.Add(new ColumnDefinition());
            }
            string[] Texts = { "Color", "Size", "Number" };
            for (int i = 0;i<Texts.Count();i++)
            {
                Label text = new Label();
                text.Content = Texts[i];
                Grid.SetRow(text, 0);
                Grid.SetColumn(text, i);
                TextstackPanel.Children.Add(text);
            }

            Grid.SetRow(TextstackPanel, 1);
            Grid.SetColumn(TextstackPanel, 0);
            CommandGrid.Children.Add(TextstackPanel);



            Grid stackPanel = new Grid();
            for (int i = 0; i < 4; i++)
            {
                stackPanel.ColumnDefinitions.Add(new ColumnDefinition());
            }

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
            Grid.SetColumn(ColorcomboBox, 0);
            Grid.SetColumn(SizecomboBox, 1);
            stackPanel.Children.Add(ColorcomboBox);
            stackPanel.Children.Add(SizecomboBox);
            
            TextBox TextBox2 = new TextBox();
            Grid.SetColumn(TextBox2, 2);
            //TextBox2.AddHandler(TextBox.PreviewTextInput, checkInput);
            TextBox2.PreviewTextInput += checkInput;
            stackPanel.Children.Add(TextBox2);

            Button Confirm = new Button();
            Grid.SetColumn(Confirm, 3);
            Confirm.Content = "Confirm";
            Confirm.Name = model;
            Confirm.Click += ConfirmOrder;
            stackPanel.Children.Add(Confirm);

            Grid.SetRow(stackPanel, 2);
            Grid.SetColumn(stackPanel, 0);
            CommandGrid.Children.Add(stackPanel);


            return grid;
        }

        private Grid GetCartGrid(double Height, double Width)
        {
            Grid grid = new Grid();
            grid.Name = "Cart";
            ColumnDefinition Column1 = new ColumnDefinition();
            ColumnDefinition Column2 = new ColumnDefinition();
            RowDefinition Row1 = new RowDefinition();
            RowDefinition Row2 = new RowDefinition();
            Column1.Width = new GridLength(7, GridUnitType.Star);
            Row1.Height = new GridLength(7, GridUnitType.Star);

            grid.ColumnDefinitions.Add(Column1);
            grid.ColumnDefinitions.Add(Column2);

            grid.RowDefinitions.Add(Row1);
            grid.RowDefinitions.Add(Row2);


            ScrollViewer scrollViewer = new ScrollViewer();

            Grid cartgrid = new Grid();
            ColumnDefinition CartColumn1 = new ColumnDefinition();
            ColumnDefinition CartColumn2 = new ColumnDefinition();
            ColumnDefinition CartColumn3 = new ColumnDefinition();

            cartgrid.ColumnDefinitions.Add(CartColumn1);
            cartgrid.ColumnDefinitions.Add(CartColumn2);
            cartgrid.ColumnDefinitions.Add(CartColumn3);

            Grid.SetRow(scrollViewer, 0);
            Grid.SetColumn(scrollViewer, 0);

            scrollViewer.Content = cartgrid;

            cart.GetConatiner(cartgrid);

            grid.Children.Add(scrollViewer);
            
            Button Comfirm = new Button();
            Comfirm.Content = "Comfirm";
            Comfirm.Click += ComfirmCart;
            //Comfirm.Width = 50;
            Grid.SetRow(Comfirm, 1);
            Grid.SetColumn(Comfirm, 1);
            grid.Children.Add(Comfirm);

            return grid;
        }

        

        private void checkInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ComfirmCart(object sender, RoutedEventArgs e)
        {

        }
        private void ConfirmOrder(object sender, RoutedEventArgs e)
        {
            Button test2 = (Button)sender;
            Grid ParentGrid = (Grid)test2.Parent;
            Grid ParentGrid2 = (Grid)ParentGrid.Parent;
            Label text = new Label();
            

            UIElementCollection children = ParentGrid.Children;
            ComboBox color = (ComboBox)children[0];
            ComboBox size = (ComboBox)children[1];
            TextBox textBox = (TextBox)children[2];

            ComboBoxItem itemColor = (ComboBoxItem)color.SelectedItem;
            ComboBoxItem itemSize = (ComboBoxItem)size.SelectedItem;

            string ID = _ID_creator[test2.Name].ToString() + _ID_creator[itemSize.Content.ToString()].ToString()+ _ID_creator[itemColor.Content.ToString()].ToString();

            cart.addToCart(Int16.Parse(ID), Int16.Parse(textBox.Text));
            
            
            text.Content = ID;
            Grid.SetRow(text, 3);
            Grid.SetColumn(text, 0);
            ParentGrid2.Children.Add(text);


        }

        
    }

    class Cart
    {
        Dictionary<int, int> BikeDict;
        Dictionary<int, RowDefinition> Rows;
        int price;
        Grid grid;
        //int Current_ID;
        //UIElement Current_Row;
        public Cart()
        {
            BikeDict = new Dictionary<int, int>();
            price = 0;
        }

        public void GetConatiner(Grid grid)
        {
            this.grid = grid;
        }

        public void addToCart(int ID, int nb)
        {
            //Current_ID = ID;
            if (BikeDict.ContainsKey(ID))
            {
                BikeDict[ID] += nb;
            }
            else
            {
                BikeDict[ID] = nb;
                RowDefinition row = new RowDefinition();
                Rows.Add(ID, row);
                grid.RowDefinitions.Add(row);

                Label label = new Label();

                label.Content = ID;
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, grid.RowDefinitions.IndexOf(row));
                grid.Children.Add(label);

                StackPanel stackHor = new StackPanel();
                StackPanel stackVert = new StackPanel();

                stackHor.Orientation = Orientation.Horizontal;
                stackVert.Orientation = Orientation.Vertical;   

                TextBlock text = new TextBlock();
                text.Foreground = Brushes.White;
                text.Text = nb.ToString();
                stackHor.Children.Add(text);

                

                Button buttonUp = new Button();
                buttonUp.Content = "+";
                //buttonUp.Name = ID.ToString();
                buttonUp.Click += PlusOne;
                buttonUp.Uid = ID.ToString();
                Button buttonDown = new Button();
                buttonDown.Content = "-";
                //buttonUp.Name = ID.ToString();
                buttonDown.Click += MinusOne;
                buttonDown.Uid = ID.ToString();
                stackVert.Children.Add(buttonUp);
                stackVert.Children.Add(buttonDown);
                stackHor.Children.Add(stackVert);

                Grid.SetColumn(stackHor, 1);
                grid.Children.Add(stackHor);

                Button Delete = new Button();
                Delete.Content = "X";
                Delete.Uid = ID.ToString();
                //Current_Row = row;
                Delete.Click += DeleteFromCart;

                Grid.SetColumn(Delete, 2);
                grid.Children.Add(Delete);

            }
        }


        public void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            removeFromCart(Int16.Parse(button.Uid));
            grid.Children.Remove((UIElement)grid.FindName("A"+button.Uid));
        }

        public void MinusOne(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel parentStack = (StackPanel)button.Parent;
            StackPanel horStack = (StackPanel)parentStack.Parent;

            TextBlock text = (TextBlock)horStack.Children[0];

            removeFromCart(Int16.Parse(button.Uid), 1);

            text.Text = BikeDict[Int16.Parse(button.Uid)].ToString();
        }

        public void PlusOne(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel parentStack = (StackPanel)button.Parent;
            StackPanel horStack = (StackPanel)parentStack.Parent;

            TextBlock text = (TextBlock)horStack.Children[0];

            BikeDict[Int16.Parse(button.Uid)] += 1;
           

            text.Text = BikeDict[Int16.Parse(button.Uid)].ToString();
        }
        public void removeFromCart(int ID, int nb)
        {
            if (BikeDict.ContainsKey(ID))
            {
                BikeDict[ID] -= nb;
                if (BikeDict[ID]<= 0){
                    BikeDict.Remove(ID);
                }
            }
        }

        public void removeFromCart(int ID)
        {
            if (BikeDict.ContainsKey(ID))
            {
                BikeDict.Remove(ID);
            }
        }
    }


}
