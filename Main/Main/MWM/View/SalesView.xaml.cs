﻿using System;
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
                tab.Background = Brushes.Transparent;
                tab.Content = GetGrid(model);
                tab.BorderBrush = Brushes.Transparent;
                tab.Foreground = Brushes.Gray;
                //tab.GotFocus += OnGotFocusHandler;
                //tab.LostFocus += OnLostFocusHandler;
                _ModelsTab.Add(tab);
                MainTabControl.Items.Add(tab);
                size_grid =(Grid) tab.Content;
            }
            TabItem carttab = new TabItem();
            carttab.Name = "Cart";
            carttab.Header = "Cart";
            carttab.Height = 75;
            carttab.BorderBrush = Brushes.Transparent;
            carttab.Foreground = Brushes.Gray;
            carttab.Background = Brushes.Transparent;
            //carttab.GotFocus += OnGotFocusHandler;
            //carttab.LostFocus += OnLostFocusHandler;
            carttab.Content = GetCartGrid(size_grid.ActualHeight,size_grid.ActualWidth);
            MainTabControl.Items.Add(carttab);
        }

        //private void OnGotFocusHandler(object sender, RoutedEventArgs e)
        //{
        //    TabItem tab = (TabItem)sender;
        //    tab.Background = Brushes.Red;
        //}
        //// Raised when Button losses focus.
        //// Changes the color of the Button back to white.
        //private void OnLostFocusHandler(object sender, RoutedEventArgs e)
        //{
        //    TabItem tab = (TabItem)sender;
        //    tab.Background = Brushes.Blue;
        //}

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
            
            image.Source = new BitmapImage(new Uri(@"C:\Users\engel\Downloads\9729122d-pure-flux-one-un-velo-electrique-leger-et-epure__1200_675__126-351-3093-2024_wtmk.jpeg", UriKind.Absolute));
            //image.Source = new BitmapImage(new Uri(@"https://github.com/fred-corp/Bovelo_PG3L_ECAM/blob/8d5c59f8b8f26f053af3004e018d842c842c9449/database/images/Adventure.jpg", UriKind.Absolute));

            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);

            TextBox textBox = new TextBox();
            textBox.Text = model+" Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam lobortis sit amet nisl eu aliquet. Mauris rutrum bibendum ligula, vel imperdiet nunc ultrices at. Nullam luctus a enim vel sagittis. Fusce tempor dignissim urna, non elementum lorem ultrices et. Phasellus ac nisl convallis, gravida ante ac, bibendum ligula. Proin efficitur scelerisque magna vel scelerisque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Duis ut tincidunt elit, at congue leo. Nullam id leo ante. Phasellus nec mauris a dui pretium fringilla eu sed dui. Nam auctor sed purus eget dignissim. Nam purus diam, vulputate ac lorem vitae, viverra euismod elit. Quisque scelerisque ultrices dapibus. Sed hendrerit nibh mattis libero viverra placerat. Duis quis lorem consequat, finibus erat ut, rutrum nisi.";
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.Background = Brushes.Transparent;
            textBox.Foreground = Brushes.White;
            textBox.BorderThickness = new Thickness(0);
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
            TextBlock.Foreground = Brushes.White;
            TextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock.Text = "Order";

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
                text.Foreground = Brushes.White;
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

            //ColorcomboBox.Background = Brushes.Red;
            //SizecomboBox.Background = Brushes.Transparent;

            foreach (string color in _Colors)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = color;
                //item.Background = Brushes.Bl;
                ColorcomboBox.Items.Add(item);
            }
            foreach (int size in _Sizes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = size;
                //item.Background = Brushes.Transparent;
                SizecomboBox.Items.Add(item);
            }

            ColorcomboBox.Height = 30;
            SizecomboBox.Height = 30;

            ColorcomboBox.SelectedIndex = 0;
            SizecomboBox.SelectedIndex = 0;
            Grid.SetColumn(ColorcomboBox, 0);
            Grid.SetColumn(SizecomboBox, 1);
            stackPanel.Children.Add(ColorcomboBox);
            stackPanel.Children.Add(SizecomboBox);
            
            TextBox TextBox2 = new TextBox();
            TextBox2.Height = 30;
            Grid.SetColumn(TextBox2, 2);
            //TextBox2.AddHandler(TextBox.PreviewTextInput, checkInput);
            TextBox2.PreviewTextInput += checkInput;
            stackPanel.Children.Add(TextBox2);

            Button Confirm = new Button();
            Confirm.Height = 30;
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

            CartColumn1.Width = new GridLength(500);

            ColumnDefinition CartColumn2 = new ColumnDefinition();
            ColumnDefinition CartColumn3 = new ColumnDefinition();

            cartgrid.ColumnDefinitions.Add(CartColumn1);
            cartgrid.ColumnDefinitions.Add(CartColumn2);
            cartgrid.ColumnDefinitions.Add(CartColumn3);



            Grid.SetRow(scrollViewer, 0);
            Grid.SetColumn(scrollViewer, 0);

            //scrollViewer.Background = Brushes.Red;

            scrollViewer.Content = cartgrid;

            cart.GetConatiner(cartgrid);

            grid.Children.Add(scrollViewer);
            
            Button Comfirm = new Button();
            Comfirm.Content = "Comfirm";
            Comfirm.Click += ComfirmCart;
            Comfirm.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
            Comfirm.Foreground = Brushes.White;
            Comfirm.BorderBrush = Brushes.Transparent;
            Comfirm.Height = 40;
            Comfirm.Width = 50;
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
            //envoiyer tout a la db
            cart.ClearCart();
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

            if (textBox.Text != "")
            {
                ComboBoxItem itemColor = (ComboBoxItem)color.SelectedItem;
                ComboBoxItem itemSize = (ComboBoxItem)size.SelectedItem;

                string ID = _ID_creator[test2.Name].ToString() + _ID_creator[itemSize.Content.ToString()].ToString() + _ID_creator[itemColor.Content.ToString()].ToString();

                cart.addToCart(Int16.Parse(ID), Int16.Parse(textBox.Text));


                text.Content = "Added to cart!";
                text.Foreground = Brushes.White;
                Grid.SetRow(text, 3);
                Grid.SetColumn(text, 0);
                ParentGrid2.Children.Add(text);

            }



        }

        
    }

    class Cart
    {
        Dictionary<int, int> BikeDict;
        Dictionary<int, RowDefinition> Rows = new Dictionary<int, RowDefinition>();
        Dictionary<int, List<UIElement>> Children = new Dictionary<int, List<UIElement>>();
        Dictionary<int, string> _ID_Model = new Dictionary<int, string>();
        Dictionary<int, string> _ID_Size = new Dictionary<int, string>();
        Dictionary<int, string> _ID_Color = new Dictionary<int, string>();
        Grid grid;
        int price;
        //int Current_ID;
        //UIElement Current_Row;
        public Cart()
        {
            BikeDict = new Dictionary<int, int>();
            price = 0;

            _ID_Color.Add(1, "Blue");
            _ID_Color.Add(2, "Red");
            _ID_Color.Add(3, "Black");

            _ID_Model.Add(1, "City");
            _ID_Model.Add(2, "Explorer");
            _ID_Model.Add(3, "Adventure");

            _ID_Size.Add(0, "26");
            _ID_Size.Add(1, "28");

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
                foreach (UIElement item in Children[ID])
                {
                    if (typeof(TextBlock) == item.GetType())
                    {
                        TextBlock temp  = (TextBlock)item;
                        temp.Text = BikeDict[ID].ToString();
                    }
                }
            }
            else
            {
                if (!Children.ContainsKey(ID))
                {
                    Children.Add(ID, new List<UIElement>());
                }
                

                BikeDict[ID] = nb;
                RowDefinition row = new RowDefinition();
                
                if (!Rows.ContainsKey(ID)) {
                    Rows.Add(ID, row);
                }
                
                grid.RowDefinitions.Add(row);

                Label label = new Label();

                //label.Content = ID;
                //string[] ID_s = ID.ToString().Split();
                //string[] digits = ID.ToString().Select(x => int.Parse(x.ToString()));
                int a = ID / 100;
                int b = (ID%100) / 10;
                int c = ID % 10;
                label.Content = _ID_Model[ID/100] + " " + _ID_Size[ID%100/10] + " " + _ID_Color[ID%10];
                label.Foreground = Brushes.White;
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, grid.RowDefinitions.IndexOf(row));
                grid.Children.Add(label);

                Children[ID].Add(label);

                StackPanel stackHor = new StackPanel();
                StackPanel stackVert = new StackPanel();

                stackHor.Orientation = Orientation.Horizontal;
                stackVert.Orientation = Orientation.Vertical;   

                TextBlock text = new TextBlock();
                text.Foreground = Brushes.White;
                text.Text = nb.ToString();
                stackHor.Children.Add(text);
                Children[ID].Add(text);


                Button buttonUp = new Button();
                buttonUp.Content = "+";
                //buttonUp.Name = ID.ToString();
                buttonUp.Click += PlusOne;
                buttonUp.Uid = ID.ToString();

                Children[ID].Add(buttonUp);

                Button buttonDown = new Button();
                buttonDown.Content = "-";
                //buttonUp.Name = ID.ToString();
                buttonDown.Click += MinusOne;
                buttonDown.Uid = ID.ToString();

                buttonDown.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
                buttonDown.Foreground = Brushes.White;
                buttonDown.BorderBrush = Brushes.Transparent;

                buttonUp.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
                buttonUp.Foreground = Brushes.White;
                buttonUp.BorderBrush = Brushes.Transparent;

                Children[ID].Add(buttonDown);

                stackVert.Children.Add(buttonUp);
                stackVert.Children.Add(buttonDown);
                stackHor.Children.Add(stackVert);

                Children[ID].Add(stackVert);
                Children[ID].Add(stackHor);
                Grid.SetRow(stackHor, grid.RowDefinitions.IndexOf(row));
                Grid.SetColumn(stackHor, 1);
                grid.Children.Add(stackHor);

                Button Delete = new Button();
                Delete.Width = 40;
                Delete.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
                Delete.Foreground = Brushes.White;
                Delete.BorderBrush = Brushes.Transparent;
                Delete.Content = "X";
                Delete.Uid = ID.ToString();
                Delete.VerticalAlignment = VerticalAlignment.Top;
                Delete.Height = 50;
                //Current_Row = row;
                Delete.Click += DeleteFromCart;

                Children[ID].Add(Delete);

                Grid.SetRow(Delete, grid.RowDefinitions.IndexOf(row));
                Grid.SetColumn(Delete, 2);
                grid.Children.Add(Delete);

            }
        }


        public void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            removeFromCart(Int16.Parse(button.Uid));
            DeleteRow(Int16.Parse(button.Uid));
        }

        public void ClearCart()
        {
            List<int> IDs = new List<int>();
            foreach(int ID in Rows.Keys)
            {
                IDs.Add(ID);
            }
            foreach(int ID in IDs)
            {
                removeFromCart(ID);
                DeleteRow(ID);
            }
        }

        public void DeleteRow(int id)
        {
            RowDefinition row = Rows[id];
            foreach (UIElement elem in Children[id])
            {
                grid.Children.Remove(elem);
                
            }
            Children.Remove(id);
            grid.RowDefinitions.Remove(row);
            Rows.Remove(id);
        }

        public void MinusOne(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel parentStack = (StackPanel)button.Parent;
            StackPanel horStack = (StackPanel)parentStack.Parent;

            TextBlock text = (TextBlock)horStack.Children[0];

            removeFromCart(Int16.Parse(button.Uid), 1);
            try
            {
                text.Text = BikeDict[Int16.Parse(button.Uid)].ToString();
            }
            catch
            {
                
            }
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
                    DeleteRow(ID);
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
