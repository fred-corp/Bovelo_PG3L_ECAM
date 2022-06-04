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
        Dictionary<int, int> BikePriceByID = new Dictionary<int, int>();
        Grid size_grid;
        Cart cart;

        Dictionary<string, int> _ID_creator = new Dictionary<string, int>();
        Label estimatedDateLabel = new Label();

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

            cmd = new MySqlCommand("select ID,price from Catalog", conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                BikePriceByID.Add(Int16.Parse(reader[0].ToString()), Int16.Parse(reader[1].ToString()));
            }
            reader.Close();


            foreach (string model in _Models)
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



            cart = new Cart(conn, _ID_Model, _ID_Size, _ID_Color, BikePriceByID, estimatedDateLabel);

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
            titleSpecs.Content = "Specification";
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
            textBox.HorizontalAlignment = HorizontalAlignment.Center;
            textBox.VerticalAlignment = VerticalAlignment.Center;
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
            TextBlock.VerticalAlignment = VerticalAlignment.Center;
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
                text.VerticalAlignment = VerticalAlignment.Bottom;
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
            Grid maingrid =new Grid();
            ColumnDefinition mainColumn = new ColumnDefinition();
            RowDefinition mainRow1 = new RowDefinition();
            RowDefinition mainRow2 = new RowDefinition();
            RowDefinition mainRowdate = new RowDefinition();

            mainRow2.Height = new GridLength(120);
            mainRowdate.Height = new GridLength(40);

            maingrid.ColumnDefinitions.Add(mainColumn);
            maingrid.RowDefinitions.Add(mainRow1);
            maingrid.RowDefinitions.Add(mainRowdate);
            maingrid.RowDefinitions.Add(mainRow2);


            Grid grid = new Grid();
            grid.Name = "Cart";
            ColumnDefinition Column1 = new ColumnDefinition();
            ColumnDefinition Column2 = new ColumnDefinition();
            RowDefinition Row1 = new RowDefinition();
            RowDefinition Row2 = new RowDefinition();
            Column1.Width = new GridLength(7, GridUnitType.Star);
            Row1.Height = new GridLength(7, GridUnitType.Star);

            grid.ColumnDefinitions.Add(Column1);
            //grid.ColumnDefinitions.Add(Column2);

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
            
            


            Grid.SetRow(grid, 0);
            Grid.SetColumn(grid, 0);
            maingrid.Children.Add(grid);

            Grid CustomerInfo = new Grid();

            ColumnDefinition CustomerColumn1 = new ColumnDefinition();
            ColumnDefinition CustomerColumn2 = new ColumnDefinition();
            ColumnDefinition CustomerColumn3 = new ColumnDefinition();
            RowDefinition CustomerRow1 = new RowDefinition();
            RowDefinition CustomerRow2 = new RowDefinition();


            CustomerInfo.RowDefinitions.Add(CustomerRow1);
            CustomerInfo.RowDefinitions.Add(CustomerRow2);
            
            CustomerInfo.ColumnDefinitions.Add(CustomerColumn1);
            CustomerInfo.ColumnDefinitions.Add(CustomerColumn2);
            CustomerInfo.ColumnDefinitions.Add(CustomerColumn3);

            StackPanel FirstNamePanel = new StackPanel();
            FirstNamePanel.Orientation  = Orientation.Vertical;
            FirstNamePanel.HorizontalAlignment = HorizontalAlignment.Left;

            Label FirstNameLabel = new Label();
            FirstNameLabel.Foreground = Brushes.White;
            FirstNameLabel.Content = "First Name:";
            FirstNamePanel.Children.Add(FirstNameLabel);

            TextBox FirstNameTextBox = new TextBox();
            FirstNameTextBox.Width = 200;
            FirstNamePanel.Children.Add(FirstNameTextBox);

            Grid.SetRow(FirstNamePanel, 0);
            Grid.SetColumn(FirstNamePanel, 0);
            CustomerInfo.Children.Add(FirstNamePanel);

            StackPanel LastNamePanel = new StackPanel();
            LastNamePanel.Orientation = Orientation.Vertical;
            LastNamePanel.HorizontalAlignment = HorizontalAlignment.Left;

            Label LastNameLabel = new Label();
            LastNameLabel.Foreground = Brushes.White;
            LastNameLabel.Content = "Last Name:";
            LastNamePanel.Children.Add(LastNameLabel);

            TextBox LastNameTextBox = new TextBox();
            LastNamePanel.Children.Add(LastNameTextBox);
            LastNameTextBox.Width = 200;

            Grid.SetRow(LastNamePanel, 1);
            Grid.SetColumn(LastNamePanel, 0);
            CustomerInfo.Children.Add(LastNamePanel);

            StackPanel AddressPanel = new StackPanel();
            AddressPanel.Orientation = Orientation.Vertical;
            AddressPanel.HorizontalAlignment = HorizontalAlignment.Left;

            Label AddressLabel = new Label();
            AddressLabel.Foreground = Brushes.White;
            AddressLabel.Content = "Address:";
            AddressPanel.Children.Add(AddressLabel);

            TextBox AddressTextBox = new TextBox();
            AddressPanel.Children.Add(AddressTextBox);
            AddressTextBox.Width = 200;

            Grid.SetRow(AddressPanel, 0);
            Grid.SetColumn(AddressPanel, 1);
            CustomerInfo.Children.Add(AddressPanel);

            StackPanel MailPanel = new StackPanel();
            MailPanel.Orientation = Orientation.Vertical;
            MailPanel.HorizontalAlignment = HorizontalAlignment.Left;

            Label MailLabel = new Label();
            MailLabel.Foreground = Brushes.White;
            MailLabel.Content = "Mail address:";
            MailPanel.Children.Add(MailLabel);

            TextBox MailTextBox = new TextBox();
            MailPanel.Children.Add(MailTextBox);
            MailTextBox.Width = 200;

            Grid.SetRow(MailPanel, 1);
            Grid.SetColumn(MailPanel, 1);
            CustomerInfo.Children.Add(MailPanel);

            StackPanel PhonePanel = new StackPanel();
            PhonePanel.Orientation = Orientation.Vertical;
            PhonePanel.HorizontalAlignment = HorizontalAlignment.Left;

            Label PhoneLabel = new Label();
            PhoneLabel.Foreground = Brushes.White;
            PhoneLabel.Content = "Phone number:";
            PhonePanel.Children.Add(PhoneLabel);

            TextBox PhoneTextBox = new TextBox();
            PhonePanel.Children.Add(PhoneTextBox);
            PhoneTextBox.PreviewTextInput += checkInput;
            PhoneTextBox.Width = 200;

            Grid.SetRow(PhonePanel, 0);
            Grid.SetColumn(PhonePanel, 2);
            CustomerInfo.Children.Add(PhonePanel);


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
            Grid.SetColumn(Comfirm, 2);
            CustomerInfo.Children.Add(Comfirm);


            Grid.SetRow(CustomerInfo, 2);
            Grid.SetColumn(CustomerInfo, 0);
            maingrid.Children.Add(CustomerInfo);


            

            

            estimatedDateLabel.Content = $"Estimated delivery date : {DateTime.Now.ToString("dd-MM-yyyy")}";
            estimatedDateLabel.Foreground = Brushes.White;

            Grid.SetColumn(estimatedDateLabel, 0);
            Grid.SetRow(estimatedDateLabel, 1);
            maingrid.Children.Add(estimatedDateLabel);

            return maingrid;
        }



        private void checkInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ComfirmCart(object sender, RoutedEventArgs e)
        {
            //ouvrir popup pour avoir connées client puis envoyer le tout a la db
            Button source = (Button)sender;
            Grid sourcegrid = (Grid)source.Parent;

            StackPanel FirstNamePanel = (StackPanel) sourcegrid.Children[0];
            TextBox FirstNameTextBox = (TextBox)FirstNamePanel.Children[1];

            StackPanel LastNamePanel = (StackPanel)sourcegrid.Children[1];
            TextBox LastNameTextBox = (TextBox)LastNamePanel.Children[1];

            StackPanel AddressPanel = (StackPanel)sourcegrid.Children[2];
            TextBox AddressTextBox = (TextBox)AddressPanel.Children[1];

            StackPanel MailPanel = (StackPanel)sourcegrid.Children[3];
            TextBox MailTextBox = (TextBox)MailPanel.Children[1];

            StackPanel PhonePanel = (StackPanel)sourcegrid.Children[4];
            TextBox PhoneTextBox = (TextBox)PhonePanel.Children[1];



            if (FirstNameTextBox.Text != "" & LastNameTextBox.Text != "" & AddressTextBox.Text != "" & MailTextBox.Text != "" & PhoneTextBox.Text != "")
            {
                Customer client = new Customer(FirstNameTextBox.Text, LastNameTextBox.Text, AddressTextBox.Text, MailTextBox.Text, PhoneTextBox.Text);

                MySqlCommand cmd = new MySqlCommand("select * from Customers where firstname=@firstname and lastname=@lastname and address=@address and email=@mail and phone=@phone;", conn);
                cmd.Parameters.Add(new MySqlParameter("firstname", client.First_Name));
                cmd.Parameters.Add(new MySqlParameter("lastname", client.Last_Name));
                cmd.Parameters.Add(new MySqlParameter("Address", client.Address));
                cmd.Parameters.Add(new MySqlParameter("mail", client.mail));
                cmd.Parameters.Add(new MySqlParameter("phone", client.phone));
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)
                {
                    cart.RegisterCustomer(client);
                }
                reader.Close();

                cmd = new MySqlCommand("select customer_number from Customers where firstname=@firstname and lastname=@lastname and address=@address and email=@mail and phone=@phone;", conn);
                cmd.Parameters.Add(new MySqlParameter("firstname", client.First_Name));
                cmd.Parameters.Add(new MySqlParameter("lastname", client.Last_Name));
                cmd.Parameters.Add(new MySqlParameter("Address", client.Address));
                cmd.Parameters.Add(new MySqlParameter("mail", client.mail));
                cmd.Parameters.Add(new MySqlParameter("phone", client.phone));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    client.customer_number = Int16.Parse(reader[0].ToString());
                }
                reader.Close();

                cmd = new MySqlCommand("INSERT INTO invoices (`customer_number`, `date`, `totalPrice`) VALUES (@customernumber, @date, @price);", conn);
                cmd.Parameters.Add(new MySqlParameter("customernumber", client.customer_number));
                cmd.Parameters.Add(new MySqlParameter("date", DateTime.Now.ToString("yyyy-MM-dd")));
                cmd.Parameters.Add(new MySqlParameter("price", cart.getPrice()));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                reader.Close();

                foreach(int key in cart.BikeDict.Keys)
                {
                    cmd = new MySqlCommand("INSERT INTO invoice_details ('invoice_number','ID','amount','price') VALUES (@invoicenumber,@ID,@amount,@price);",conn);
                    cmd.Parameters.Add(new MySqlParameter("invoicenumber", getInvoiceNumber(client)));
                    cmd.Parameters.Add(new MySqlParameter("ID", key));
                    cmd.Parameters.Add(new MySqlParameter("amount", cart.BikeDict[key]));
                    cmd.Parameters.Add(new MySqlParameter("price", BikePriceByID[key]* cart.BikeDict[key]));
                }
                cart.ClearCart();

                FirstNameTextBox.Text = "";
                LastNameTextBox.Text = "";
                AddressTextBox.Text = "";
                MailTextBox.Text = "";
                PhoneTextBox.Text = "";


               

            }

        }

        private int getInvoiceNumber(Customer client)
        {
            int InvoiceNumber = 0;
            MySqlCommand cmd = new MySqlCommand("select invoice_number from invoices where customer_number=@customernumber and totalPrice=@price", conn);
            cmd.Parameters.Add(new MySqlParameter("customernumber", client.customer_number));
            cmd.Parameters.Add(new MySqlParameter("price", cart.getPrice()));
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                InvoiceNumber = Int16.Parse(reader[0].ToString()); 
            }
            reader.Close();
            return InvoiceNumber;
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
        public Dictionary<int, int> BikeDict;
        Dictionary<int, int> _BikePricebyID;
        Dictionary<int, RowDefinition> Rows = new Dictionary<int, RowDefinition>();
        Dictionary<int, List<UIElement>> Children = new Dictionary<int, List<UIElement>>();
        Dictionary<int, string> _ID_Model;
        Dictionary<int, string> _ID_Size;
        Dictionary<int, string> _ID_Color;
        Label _estimatedDateLabel;
        Grid grid;
        int price;
        MySqlConnection conn;
        //int Current_ID;
        //UIElement Current_Row;
        public Cart(MySqlConnection _conn, Dictionary<int, string> ID_Model, Dictionary<int, string>ID_Size, Dictionary<int, string>ID_Color, Dictionary<int, int> BikePricebyID, Label estimatedDateLabel)
        {
            conn = _conn;

            _ID_Color = ID_Color;
            _ID_Model = ID_Model;
            _ID_Size = ID_Size;
            _BikePricebyID = BikePricebyID;
            _estimatedDateLabel = estimatedDateLabel;

            BikeDict = new Dictionary<int, int>();
            price = 0;

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
            updateEstimatedDate();
        }


        public void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            removeFromCart(Int16.Parse(button.Uid));
            DeleteRow(Int16.Parse(button.Uid));
            updateEstimatedDate();
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
            updateEstimatedDate();
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
            updateEstimatedDate();
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
            updateEstimatedDate();
        }

        public void PlusOne(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel parentStack = (StackPanel)button.Parent;
            StackPanel horStack = (StackPanel)parentStack.Parent;

            TextBlock text = (TextBlock)horStack.Children[0];

            BikeDict[Int16.Parse(button.Uid)] += 1;
           
            text.Text = BikeDict[Int16.Parse(button.Uid)].ToString();
            updateEstimatedDate();
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
                updateEstimatedDate();
            }
        }

        public void removeFromCart(int ID)
        {
            if (BikeDict.ContainsKey(ID))
            {
                BikeDict.Remove(ID);
                updateEstimatedDate();
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

        public int getPrice()
        {
            foreach (int key in BikeDict.Keys)
            {
                price += BikeDict[key] * _BikePricebyID[key];
            }
            return price;
        }

        public DateTime getEstimatedDate()
        {
            DateTime estimatedDate = DateTime.Today;
            double BookedDays = 0;

            MySqlCommand cmd = new MySqlCommand("SELECT SUM(amount) from invoice_details;", conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                object a = reader[0];
                BookedDays = Int16.Parse(reader[0].ToString()) / 12;
            }
            reader.Close();

            int roundedbookeddays = (int)Math.Round(BookedDays);

            int orderedBikes = 0;
            foreach (int key in BikeDict.Keys)
            {
                orderedBikes += BikeDict[key];
            }

            roundedbookeddays += (int)Math.Round((double)orderedBikes / 12);

            roundedbookeddays += ((int)roundedbookeddays / 7) * 2;

            estimatedDate = estimatedDate.AddDays(roundedbookeddays);

            return estimatedDate;
        }

        public void updateEstimatedDate()
        {
            _estimatedDateLabel.Content = $"Estimated delivery date : {getEstimatedDate().ToString("dd-MM-yyyy")}";
        }

    }

    class Customer
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Address { get; set; }
        public string phone { get; set; }
        public string mail { get; set; }
        public int customer_number { get; set; }


        public Customer(string _First_Name, string _Last_Name, string _Adress, string _mail, string _phone)
        {
            this.First_Name = _First_Name;
            this.Last_Name = _Last_Name;
            this.Address = _Adress;
            this.mail = _mail;
            this.phone = _phone;
            
        }


    }
}
