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
using MySql.Data.MySqlClient;
using System.Configuration;

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
        Dictionary<int, string> _ID_Color = new Dictionary<int,string>();
        Dictionary<int, string> _ID_Model = new Dictionary<int, string>();
        Dictionary<int, string> _ID_Size = new Dictionary<int, string>();
        Grid size_grid;
        Cart cart;

        Dictionary<string, int> _ID_creator = new Dictionary<string, int>();


        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);


        public SalesView()
        {

            try
            {
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to connect to data source " + ex.ToString());
            }
            InitializeComponent();



            MySqlCommand cmd = new MySqlCommand("SELECT description FROM Colors", conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _Colors.Add(reader[0].ToString());
            }
            reader.Close();
            //_Colors.Add("Red");
            //_Colors.Add("Blue");
            //_Colors.Add("Black");


            //cmd = new MySqlCommand("SELECT description FROM Catalog WHERE model=@model LIMIT 1", conn);
            //reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    textBox.Text = reader[0].ToString();
            //}
            //reader.Close();
            cmd = new MySqlCommand("select size, min(id) from Catalog group by size", conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _Sizes.Add((int)reader[0]);
            }
            reader.Close();

            cmd = new MySqlCommand("select model, min(id) from Catalog group by model", conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _Models.Add(reader[0].ToString());
            }
            reader.Close();

            //_Sizes.Add(26);
            //_Sizes.Add(28);


            //_Models.Add("City");
            //_Models.Add("Explorer");
            //_Models.Add("Adventure");

            //for (int i = 0; i < _Models.Count; i++)
            //{
            //    _ID_creator.Add(_Models[i], i + 1);
            //}

            //for (int i = 0; i < _Sizes.Count; i++)
            //{
            //    _ID_creator.Add(_Sizes[i].ToString(), i);
            //}

            //for (int i = 0; i < _Colors.Count; i++)
            //{
            //    _ID_creator.Add(_Colors[i], i + 1);
            //}


            foreach(string model in _Models)
            {
                cmd = new MySqlCommand("select id from Catalog where model = @model limit 1", conn);
                cmd.Parameters.Add(new MySqlParameter("model", model));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader[0].ToString();
                    _ID_creator.Add(model, Int16.Parse(id[0].ToString()));
                    _ID_Model.Add(Int16.Parse(id[0].ToString()), model);
                }
                reader.Close();
            }

            foreach (string color in _Colors)
            {
                cmd = new MySqlCommand("select id from Colors where description = @color limit 1", conn);
                cmd.Parameters.Add(new MySqlParameter("color", color));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _ID_creator.Add(color, Int16.Parse(reader[0].ToString()));
                    _ID_Color.Add(Int16.Parse(reader[0].ToString()), color);
                }
                reader.Close();
            }

            foreach (int size in _Sizes)
            {
                cmd = new MySqlCommand("select id from Catalog where size = @size limit 1", conn);
                cmd.Parameters.Add(new MySqlParameter("size", size));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader[0].ToString();
                    _ID_creator.Add(size.ToString(), Int16.Parse(id[1].ToString()));
                    _ID_Size.Add(Int16.Parse(id[1].ToString()), size.ToString());
                }
                reader.Close();
            }


            //_ID_creator.Add("City", 1);
            //_ID_creator.Add("Explorer", 2);
            //_ID_creator.Add("Adventure", 3);



            //_ID_creator.Add("26", 0);
            //_ID_creator.Add("28", 1);


            //_ID_creator.Add("Blue", 1);
            //_ID_creator.Add("Red", 2);
            //_ID_creator.Add("Black", 3);



            cart = new Cart(conn, _ID_Model, _ID_Size, _ID_Color);

            foreach (var model in _Models)
            {
                TabItem tab = new TabItem();
                tab.Name = model;
                tab.Header = model;
                tab.Height = 75;
                tab.Width = 100;
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
            carttab.Width = 100;
            carttab.BorderBrush = Brushes.Transparent;
            carttab.Foreground = Brushes.Gray;
            carttab.Background = Brushes.Transparent;
            //carttab.GotFocus += OnGotFocusHandler;
            //carttab.LostFocus += OnLostFocusHandler;
            carttab.Content = GetCartGrid(size_grid.ActualHeight,size_grid.ActualWidth);
            MainTabControl.Items.Add(carttab);
            MainTabControl.BorderBrush = Brushes.Transparent;
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


            var fullFilePath = @"";

            MySqlCommand cmd = new MySqlCommand("select image, min(id) from Catalog where model=@model group by image", conn);
            cmd.Parameters.Add(new MySqlParameter("model", model));
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                fullFilePath = reader[0].ToString();
                //fullFilePath = @"http://www.americanlayout.com/wp/wp-content/uploads/2012/08/C-To-Go-300x300.png";
            }
            reader.Close();

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            image.Source = bitmap;


            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            grid.Children.Add(image);






            StackPanel stackSpecs = new StackPanel();
            stackSpecs.Orientation = Orientation.Vertical;
            Label titleSpecs = new Label();
            titleSpecs.Content = "Spesification";
            titleSpecs.Foreground = Brushes.White;
            titleSpecs.Background = Brushes.Transparent;
            titleSpecs.HorizontalAlignment = HorizontalAlignment.Center;
            
            
            stackSpecs.Children.Add(titleSpecs);


            


            TextBox specs = new TextBox();
            
            specs.Foreground = Brushes.White;
            specs.Background = Brushes.Transparent;
            specs.HorizontalAlignment = HorizontalAlignment.Center;


            cmd = new MySqlCommand("SELECT specs FROM Catalog WHERE model=@model LIMIT 1", conn);
            cmd.Parameters.Add(new MySqlParameter("model", model));
            reader = cmd.ExecuteReader();
            string sSpecs = "";
            while (reader.Read())
            {
                sSpecs = reader[0].ToString();
                
            }
            reader.Close();

            string[] specsTable = sSpecs.Split(',');

            foreach(string s in specsTable)
            {
                specs.Text +=s+"\n";
            }
            

            stackSpecs.Children.Add(specs);

            Grid.SetRow(stackSpecs, 0);
            Grid.SetColumn(stackSpecs, 1);
            grid.Children.Add(stackSpecs);


            TextBox textBox = new TextBox();

            cmd = new MySqlCommand("SELECT description FROM Catalog WHERE model=@model LIMIT 1", conn);
            cmd.Parameters.Add(new MySqlParameter("model", model));
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                textBox.Text = reader[0].ToString();
            }
            reader.Close();


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

            cart.SetContainer(cartgrid);

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
            //ouvrir popup pour avoir connées client puis envoyer le tout a la db

            cart.ClearCart();
            Window1 secondWindow = new Window1();
            secondWindow.Show();
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
        Dictionary<int, string> _ID_Model;
        Dictionary<int, string> _ID_Size;
        Dictionary<int, string> _ID_Color;
        Grid grid;
        int price;
        MySqlConnection conn;
        //int Current_ID;
        //UIElement Current_Row;
        public Cart(MySqlConnection _conn, Dictionary<int, string> ID_Model, Dictionary<int, string>ID_Size, Dictionary<int, string>ID_Color)
        {
            conn = _conn;

            _ID_Color = ID_Color;
            _ID_Model = ID_Model;
            _ID_Size = ID_Size;

            BikeDict = new Dictionary<int, int>();
            price = 0;

            //_ID_Color.Add(1, "Blue");
            //_ID_Color.Add(2, "Red");
            //_ID_Color.Add(3, "Black");

            //_ID_Model.Add(1, "City");
            //_ID_Model.Add(2, "Explorer");
            //_ID_Model.Add(3, "Adventure");

            //_ID_Size.Add(0, "26");
            //_ID_Size.Add(1, "28");

        }

        public void SetContainer(Grid grid)
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
                row.Height = new GridLength(50);
                
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

        public void RegisterCustomer(Customer customer)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO customers (`firstname`, `lastname`, `address`, `phone`, `email`) VALUES (@first, @last, @add, @phone, @mail);", conn);
            cmd.Parameters.Add(new MySqlParameter("first", customer.First_Name));
            cmd.Parameters.Add(new MySqlParameter("last", customer.Last_Name));
            cmd.Parameters.Add(new MySqlParameter("add", customer.Address));
            cmd.Parameters.Add(new MySqlParameter("first", customer.First_Name));
            cmd.Parameters.Add(new MySqlParameter("first", customer.First_Name));
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                
            }
            reader.Close();

        }

    }

    class Customer
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Address { get; set; }
        public string phone { get; set; }
        public string mail { get; set; }


        public Customer(string _First_Name, string _Last_Name, string _Adress, string _phone, string _mail)
        {
            this.First_Name = _First_Name;
            this.Last_Name = _Last_Name;
            this.Address = _Adress;
            this.mail = _mail;
            this.phone = _phone;
        }


    }
}
