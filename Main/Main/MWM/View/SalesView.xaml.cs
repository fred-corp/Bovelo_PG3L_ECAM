using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        /// <summary>
        ///  Class to build all the windows and  attach interactions, includes all the connexions to DB to fetch data 
        /// </summary>
        /// 

        List<int> Sizes = new List<int>();
        List<string> Models = new List<string>();
        List<string> Colors = new List<string>();
        List<TabItem> ModelsTab = new List<TabItem>();

        Dictionary<int, string> ID_Model = new Dictionary<int, string>();  //links single digit id to model, wich is the first number of the bike id
        Dictionary<int, string> ID_Size = new Dictionary<int, string>();  //links single digit id to size, wich is the second number of the bike id
        Dictionary<int, string> ID_Color = new Dictionary<int, string>();   //links single digit id to color, wich is the third number of the bike id
        Dictionary<int, int> BikePriceByID = new Dictionary<int, int>();  //links 3 digit bike id to price
        Grid SizeGrid;    //grid containing the current window selected by the tab, used to fix the size
        Cart Cart;

        Dictionary<string, int> ID_creator = new Dictionary<string, int>();  //links digits to their corrrespoonding color/size/model to build the id used in database
        Label EstimatedDateLabel = new Label();     //label to display the estimated date of delivery, created here to acces it in various cart

        MySqlConnection Connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);


        public SalesView()
        {




            //opening connetion to database
            try
            {
                Connection.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to connect to data source " + ex.ToString());
            }
            InitializeComponent();



            //fetch data from database to fill the dictionaries

            MySqlCommand cmd = new MySqlCommand("SELECT description FROM Colors", Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Colors.Add(reader[0].ToString());
            }
            reader.Close();

            cmd = new MySqlCommand("select size, min(id) from Catalog group by size", Connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Sizes.Add((int)reader[0]);
            }
            reader.Close();

            cmd = new MySqlCommand("select model, min(id) from Catalog group by model", Connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Models.Add(reader[0].ToString());
            }
            reader.Close();

            cmd = new MySqlCommand("select ID,price from Catalog", Connection);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                BikePriceByID.Add(Int16.Parse(reader[0].ToString()), Int16.Parse(reader[1].ToString()));
            }
            reader.Close();

            //fills the id_creator dictionary with data from the databse
            foreach (string model in Models)
            {
                cmd = new MySqlCommand("select id from Catalog where model = @model limit 1", Connection);
                cmd.Parameters.Add(new MySqlParameter("model", model));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader[0].ToString();
                    ID_creator.Add(model, Int16.Parse(id[0].ToString()));
                    ID_Model.Add(Int16.Parse(id[0].ToString()), model);
                }
                reader.Close();
            }

            foreach (string color in Colors)
            {
                cmd = new MySqlCommand("select id from Colors where description = @color limit 1", Connection);
                cmd.Parameters.Add(new MySqlParameter("color", color));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ID_creator.Add(color, Int16.Parse(reader[0].ToString()));
                    ID_Color.Add(Int16.Parse(reader[0].ToString()), color);
                }
                reader.Close();
            }

            foreach (int size in Sizes)
            {
                cmd = new MySqlCommand("select id from Catalog where size = @size limit 1", Connection);
                cmd.Parameters.Add(new MySqlParameter("size", size));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader[0].ToString();
                    ID_creator.Add(size.ToString(), Int16.Parse(id[1].ToString()));
                    ID_Size.Add(Int16.Parse(id[1].ToString()), size.ToString());
                }
                reader.Close();
            }


            // creates cart object used to contain the items to order
            Cart = new Cart(Connection, ID_Model, ID_Size, ID_Color, BikePriceByID, EstimatedDateLabel);


            //creates 1 tab per model, content of tab is a grid created by the GetModelGrid function
            foreach (var model in Models)
            {
                TabItem tab = new TabItem();
                tab.Name = model;
                tab.Header = model;
                tab.Height = 75;
                tab.Width = 100;
                tab.Background = Brushes.Transparent;
                tab.Content = GetModelGrid(model);
                tab.BorderBrush = Brushes.Transparent;
                tab.Foreground = Brushes.Gray;
                ModelsTab.Add(tab);
                MainTabControl.Items.Add(tab);
                SizeGrid =(Grid) tab.Content;
            }

            //adds the cart tab containing the summary of the order and the fillable customer info form
            TabItem carttab = new TabItem();
            carttab.Name = "Cart";
            carttab.Header = "Cart";
            carttab.Height = 75;
            carttab.Width = 100;
            carttab.BorderBrush = Brushes.Transparent;
            carttab.Foreground = Brushes.Gray;
            carttab.Background = Brushes.Transparent;
            carttab.Content = GetCartGrid();
            MainTabControl.Items.Add(carttab);
            MainTabControl.BorderBrush = Brushes.Transparent;
        }

       
        private Grid GetModelGrid(string model)
        {
            /// Creates a grid containing the photo, specification, price, and info of the bike, as well as the "add to cart" form


            ///creates the grid
           
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



            ///add the image from the database

            Image image = new Image();


            var fullFilePath = @"";

            MySqlCommand cmd = new MySqlCommand("select image, min(id) from Catalog where model=@model group by image", Connection);
            cmd.Parameters.Add(new MySqlParameter("model", model));
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                fullFilePath = reader[0].ToString();
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




            //creates the stackpanel containing the specification and price of the model

            StackPanel stackSpecs = new StackPanel();
            stackSpecs.Orientation = Orientation.Vertical;

            //adds a label to the stackpanel
            Label titleSpecs = new Label();
            titleSpecs.Content = "Specification";
            titleSpecs.Foreground = Brushes.White;
            titleSpecs.Background = Brushes.Transparent;
            titleSpecs.HorizontalAlignment = HorizontalAlignment.Center;
            
            
            stackSpecs.Children.Add(titleSpecs);


            

            //adds the specification to the stackpanel, comming from the database
            TextBox specs = new TextBox();
            
            specs.Foreground = Brushes.White;
            specs.Background = Brushes.Transparent;
            specs.HorizontalAlignment = HorizontalAlignment.Center;


            cmd = new MySqlCommand("SELECT specs FROM Catalog WHERE model=@model LIMIT 1", Connection);
            cmd.Parameters.Add(new MySqlParameter("model", model));
            reader = cmd.ExecuteReader();
            string sSpecs = "";
            while (reader.Read())
            {
                sSpecs = reader[0].ToString();
                
            }
            reader.Close();


            //formats the specification to be one specifiation per line

            string[] specsTable = sSpecs.Split(',');

            foreach(string s in specsTable)
            {
                specs.Text +=s+"\n";
            }
            
            stackSpecs.Children.Add(specs);



            //adds the price of the model per size to the stackpanel, price dfrom DB

            List<string> modelPrice = new List<string>();

            cmd = new MySqlCommand("SELECT price FROM Catalog where model = @model GROUP BY price", Connection);
            cmd.Parameters.Add(new MySqlParameter("model", model));
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                modelPrice.Add(reader[0].ToString());
            }
            reader.Close();        

            Label pricelabel = new Label();
            pricelabel.HorizontalAlignment = HorizontalAlignment.Center;
            pricelabel.Background = Brushes.Transparent;
            pricelabel.Foreground = Brushes.White;
            pricelabel.Content = $"Price 26': {modelPrice[0]}€ \nPrice 28': {modelPrice[1]}€";
            stackSpecs.Children.Add(pricelabel);


            //adds stack panel to grid
            Grid.SetRow(stackSpecs, 0);
            Grid.SetColumn(stackSpecs, 1);
            grid.Children.Add(stackSpecs);

            //Creates textbox containing description of the model
            TextBox textBox = new TextBox();

            cmd = new MySqlCommand("SELECT description FROM Catalog WHERE model=@model LIMIT 1", Connection);
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



            //creates a grid wich contains the add to cart form
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


            //adds "order" label to the commandgrid
            TextBlock TextBlock = new TextBlock();
            TextBlock.Foreground = Brushes.White;
            TextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            TextBlock.VerticalAlignment = VerticalAlignment.Center;
            TextBlock.Text = "Order";

            Grid.SetRow(TextBlock, 0);
            Grid.SetColumn(TextBlock, 0);
            CommandGrid.Children.Add(TextBlock);


            //adds new grid to the commandgrid, containing the title of the fillable form bellow
            Grid TextGrid = new Grid();
            for (int i = 0; i < 4; i++)
            {
                TextGrid.ColumnDefinitions.Add(new ColumnDefinition());
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
                TextGrid.Children.Add(text);
            }

            Grid.SetRow(TextGrid, 1);
            Grid.SetColumn(TextGrid, 0);
            CommandGrid.Children.Add(TextGrid);


            //adds new grid to the commandgrid containing the fillable form to add bikes to the cart
            Grid formGrid = new Grid();
            for (int i = 0; i < 4; i++)
            {
                formGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ComboBox ColorcomboBox = new ComboBox();
            ComboBox SizecomboBox = new ComboBox();


            // adds all options to the combo boxes
            foreach (string color in Colors)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = color;
                ColorcomboBox.Items.Add(item);
            }
            foreach (int size in Sizes)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = size;
                SizecomboBox.Items.Add(item);
            }

            //sets size and default selection of the comboboxes
            ColorcomboBox.Height = 30;
            SizecomboBox.Height = 30;

            ColorcomboBox.SelectedIndex = 0;
            SizecomboBox.SelectedIndex = 0;

            //fixes comboboxes to the grid
            Grid.SetColumn(ColorcomboBox, 0);
            Grid.SetColumn(SizecomboBox, 1);
            formGrid.Children.Add(ColorcomboBox);
            formGrid.Children.Add(SizecomboBox);
            

            //adds the fillable textbox to set the desired number of bikes to add to cart
            TextBox BikeNumberTextBox = new TextBox();
            BikeNumberTextBox.Height = 30;
            Grid.SetColumn(BikeNumberTextBox, 2);
            BikeNumberTextBox.PreviewTextInput += CheckInput;
            formGrid.Children.Add(BikeNumberTextBox);

            //adds the add to cart button
            Button Confirm = new Button();
            Confirm.Height = 30;
            Grid.SetColumn(Confirm, 3);
            Confirm.Content = "Add to cart";
            Confirm.Name = model;
            Confirm.Click += ComfirmAddToCart;
            formGrid.Children.Add(Confirm);

            Grid.SetRow(formGrid, 2);
            Grid.SetColumn(formGrid, 0);
            CommandGrid.Children.Add(formGrid);


            return grid;
        }

       

        private Grid GetCartGrid()
        {
            ///creates the cart grid displayed on the cart tab
            
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

            //creates the grid conatining the scrollviewer wich displayes the bikes currently in the cart
            Grid grid = new Grid();
            grid.Name = "Cart";
            ColumnDefinition Column1 = new ColumnDefinition();
            ColumnDefinition Column2 = new ColumnDefinition();
            RowDefinition Row1 = new RowDefinition();
            RowDefinition Row2 = new RowDefinition();
            Column1.Width = new GridLength(7, GridUnitType.Star);
            Row1.Height = new GridLength(7, GridUnitType.Star);

            grid.ColumnDefinitions.Add(Column1);

            grid.RowDefinitions.Add(Row1);
            grid.RowDefinitions.Add(Row2);

            //creates the scrollviewer
            ScrollViewer scrollViewer = new ScrollViewer();


            //creates the grid contained in the scrollviewer, this grid is used to correctly display the bikes in the cart
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


            scrollViewer.Content = cartgrid;

            Cart.SetContainer(cartgrid);

            grid.Children.Add(scrollViewer);
            
            


            Grid.SetRow(grid, 0);
            Grid.SetColumn(grid, 0);
            maingrid.Children.Add(grid);



            //creates the grid containing the fillable form for customer info
            Grid CustomerInfo = new Grid();

            ColumnDefinition CustomerColumn1 = new ColumnDefinition();
            ColumnDefinition CustomerColumn2 = new ColumnDefinition();
            ColumnDefinition CustomerColumn3 = new ColumnDefinition();
            RowDefinition CustomerRow1 = new RowDefinition();
            RowDefinition CustomerRow2 = new RowDefinition();

            //customer info grid row and collumn definitions
            CustomerInfo.RowDefinitions.Add(CustomerRow1);
            CustomerInfo.RowDefinitions.Add(CustomerRow2);
            
            CustomerInfo.ColumnDefinitions.Add(CustomerColumn1);
            CustomerInfo.ColumnDefinitions.Add(CustomerColumn2);
            CustomerInfo.ColumnDefinitions.Add(CustomerColumn3);


            //creates the Vertical stackpanel containing the label and the fillable text box for the fist name of the customer
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

            //creates the Vertical stackpanel containing the label and the fillable text box for the last name of the customer
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

            //creates the Vertical stackpanel containing the label and the fillable text box for the address of the customer
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

            //creates the Vertical stackpanel containing the label and the fillable text box for the mail address of the customer
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

            //creates the Vertical stackpanel containing the label and the fillable text box for the phone number of the customer
            StackPanel PhonePanel = new StackPanel();
            PhonePanel.Orientation = Orientation.Vertical;
            PhonePanel.HorizontalAlignment = HorizontalAlignment.Left;

            Label PhoneLabel = new Label();
            PhoneLabel.Foreground = Brushes.White;
            PhoneLabel.Content = "Phone number:";
            PhonePanel.Children.Add(PhoneLabel);

            TextBox PhoneTextBox = new TextBox();
            PhonePanel.Children.Add(PhoneTextBox);
            PhoneTextBox.PreviewTextInput += CheckInput;
            PhoneTextBox.Width = 200;

            Grid.SetRow(PhonePanel, 0);
            Grid.SetColumn(PhonePanel, 2);
            CustomerInfo.Children.Add(PhonePanel);

            //adds the button to comfirm the order
            Button Comfirm = new Button();
            Comfirm.Content = "Comfirm";
            Comfirm.Click += ComfirmCart;
            Comfirm.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
            Comfirm.Foreground = Brushes.White;
            Comfirm.BorderBrush = Brushes.Transparent;
            Comfirm.Height = 40;
            Comfirm.Width = 50;
            Grid.SetRow(Comfirm, 1);
            Grid.SetColumn(Comfirm, 2);
            CustomerInfo.Children.Add(Comfirm);


            Grid.SetRow(CustomerInfo, 2);
            Grid.SetColumn(CustomerInfo, 0);
            maingrid.Children.Add(CustomerInfo);

            //sets default date for delivery date
            EstimatedDateLabel.Content = $"Estimated delivery date : {DateTime.Now.ToString("dd-MM-yyyy")}";
            EstimatedDateLabel.Foreground = Brushes.White;

            Grid.SetColumn(EstimatedDateLabel, 0);
            Grid.SetRow(EstimatedDateLabel, 1);
            maingrid.Children.Add(EstimatedDateLabel);

            return maingrid;
        }



        private void CheckInput(object sender, TextCompositionEventArgs e)
        {
            ///method used to check if the input of a textbox is a number or not
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ComfirmCart(object sender, RoutedEventArgs e)
        {
            ///method used to send the order to the DB, linked to the Comfirm button in the cart tab


            // sets the variuable to acces different ui element from the cart tab
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


            //checks if all the textbox have been filled
            if (FirstNameTextBox.Text != "" & LastNameTextBox.Text != "" & AddressTextBox.Text != "" & MailTextBox.Text != "" & PhoneTextBox.Text != "")
            {
                Customer client = new Customer(FirstNameTextBox.Text, LastNameTextBox.Text, AddressTextBox.Text, MailTextBox.Text, PhoneTextBox.Text);

                MySqlCommand cmd = new MySqlCommand("select * from customers where firstname=@firstname and lastname=@lastname and address=@address and email=@mail and phone=@phone;", Connection);
                cmd.Parameters.Add(new MySqlParameter("firstname", client.FirstName));
                cmd.Parameters.Add(new MySqlParameter("lastname", client.LastName));
                cmd.Parameters.Add(new MySqlParameter("Address", client.Address));
                cmd.Parameters.Add(new MySqlParameter("mail", client.Mail));
                cmd.Parameters.Add(new MySqlParameter("phone", client.Phone));
                MySqlDataReader reader = cmd.ExecuteReader();
                if (!reader.HasRows)    //if the customer does not already exist, adds it to the DB, else, does nothing
                {
                    reader.Close();
                    Cart.RegisterCustomer(client);
                }
                else
                {
                    reader.Close();
                }
                


                //gets the customer number from the DB
                cmd = new MySqlCommand("select customer_number from customers where firstname=@firstname and lastname=@lastname and address=@address and email=@mail and phone=@phone;", Connection);
                cmd.Parameters.Add(new MySqlParameter("firstname", client.FirstName));
                cmd.Parameters.Add(new MySqlParameter("lastname", client.LastName));
                cmd.Parameters.Add(new MySqlParameter("Address", client.Address));
                cmd.Parameters.Add(new MySqlParameter("mail", client.Mail));
                cmd.Parameters.Add(new MySqlParameter("phone", client.Phone));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    client.CustomerNumber = Int16.Parse(reader[0].ToString());
                }
                reader.Close();


                //sends the invoice to the DB
                cmd = new MySqlCommand("INSERT INTO invoices (`customer_number`, `date`, `totalPrice`) VALUES (@customernumber, @date, @price);", Connection);
                cmd.Parameters.Add(new MySqlParameter("customernumber", client.CustomerNumber));
                cmd.Parameters.Add(new MySqlParameter("date", DateTime.Now.ToString("yyyy-MM-dd")));
                cmd.Parameters.Add(new MySqlParameter("price", Cart.GetPrice()));
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                }
                reader.Close();


                //for each different bike type in the cart, sends an invoice detail to the DB
                foreach (int key in Cart.BikeDict.Keys)
                {
                    cmd = new MySqlCommand("INSERT INTO invoice_details (invoice_number,ID,amount,price) VALUES ((select invoice_number from invoices where customer_number=@customernumber and totalPrice=@pricet and date=@date),@ID,@amount,@price);", Connection);
                    //cmd.Parameters.Add(new MySqlParameter("invoicenumber", invoice_number));
                    cmd.Parameters.Add(new MySqlParameter("ID", key));
                    cmd.Parameters.Add(new MySqlParameter("amount", Cart.BikeDict[key]));
                    cmd.Parameters.Add(new MySqlParameter("price", BikePriceByID[key]* Cart.BikeDict[key]));
                    cmd.Parameters.Add(new MySqlParameter("customernumber", client.CustomerNumber));
                    cmd.Parameters.Add(new MySqlParameter("pricet", Cart.GetPrice()));
                    cmd.Parameters.Add(new MySqlParameter("date", DateTime.Now.ToString("yyyy-MM-dd")));
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }

                //clears the cart and the customer info form
                Cart.ClearCart();

                FirstNameTextBox.Text = "";
                LastNameTextBox.Text = "";
                AddressTextBox.Text = "";
                MailTextBox.Text = "";
                PhoneTextBox.Text = "";


               

            }

        }

        private void ComfirmAddToCart(object sender, RoutedEventArgs e)
        {
            ///method used to add bikes to the cart from the form in the model tab
            
            //creates variable to acces ui elements in the model tab
            Button test2 = (Button)sender;
            Grid ParentGrid = (Grid)test2.Parent;
            Grid ParentGrid2 = (Grid)ParentGrid.Parent;
            Label text = new Label();
            

            UIElementCollection children = ParentGrid.Children;
            ComboBox color = (ComboBox)children[0];
            ComboBox size = (ComboBox)children[1];
            TextBox textBox = (TextBox)children[2];

            if (textBox.Text != "")     //checks if textbox is empty
            {
                ComboBoxItem itemColor = (ComboBoxItem)color.SelectedItem;
                ComboBoxItem itemSize = (ComboBoxItem)size.SelectedItem;

                string ID = ID_creator[test2.Name].ToString() + ID_creator[itemSize.Content.ToString()].ToString() + ID_creator[itemColor.Content.ToString()].ToString();  //recreate the id of the bike used in the DB

                Cart.AddToCart(Int16.Parse(ID), Int16.Parse(textBox.Text));     //adds the bikes to the cart object

                textBox.Text = "";      //clears the textbox

                text.Content = "Added to cart!";            //displays text to comfirm the bike are added to the cart
                text.Foreground = Brushes.White;
                Grid.SetRow(text, 3);
                Grid.SetColumn(text, 0);
                ParentGrid2.Children.Add(text);

            }



        }

        
    }

    class Cart
    {
        /// <summary>
        /// Cart class used to conatin the order of bikes, implements all the relevant method to manage the bike order
        /// </summary>
        public Dictionary<int, int> BikeDict;   //Dictionary containing all the bike currently in the cart
        Dictionary<int, int> BikePricebyID;     //Dictionary linking the bike id to their price
        Dictionary<int, RowDefinition> Rows = new Dictionary<int, RowDefinition>();     //dicationary containing all the Rows in the grid displaying the contents of the cart in the cart tab
        Dictionary<int, List<UIElement>> Children = new Dictionary<int, List<UIElement>>();     //dictionary of the children of each row in the grid displaying the contents of the cart in the cart tab
        Dictionary<int, string> ID_Model;
        Dictionary<int, string> ID_Size;
        Dictionary<int, string> ID_Color;
        Label EstimatedDateLabel;
        Grid Grid;
        int Price;
        MySqlConnection Connection;
        public Cart(MySqlConnection connection, Dictionary<int, string> ID_Model, Dictionary<int, string>ID_Size, Dictionary<int, string>ID_Color, Dictionary<int, int> BikePricebyID, Label estimatedDateLabel)
        {
            this.Connection = connection;

            this.ID_Color = ID_Color;
            this.ID_Model = ID_Model;
            this.ID_Size = ID_Size;
            this.BikePricebyID = BikePricebyID;
            EstimatedDateLabel = estimatedDateLabel;

            BikeDict = new Dictionary<int, int>();
            Price = 0;

        }

        
        public void SetContainer(Grid grid)
        {
            ///get the grid displaying the contents of the cart in the cart tab
            this.Grid = grid;
        }


        
        public void AddToCart(int ID, int nb)
        {
            ///adds a number of bike to the BikeDict dictionary and creates the row to diplay them in the cart tab grid
            

            //if the bike id is already in the bikeDict, add nb to the value of the key corresponding to ID in the dict, else, creates the key int the dict, sets its value to nb and creates the corresponding row
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
                    Children.Add(ID, new List<UIElement>());    //creates the list in wich all the children of the row displaying the bike order are stored
                }
                
                //sets the numpber of bike ordered
                BikeDict[ID] = nb;

                //creates and adds the row
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(50);
                
                if (!Rows.ContainsKey(ID)) {
                    Rows.Add(ID, row);
                }
                
                Grid.RowDefinitions.Add(row);


                //creates the label containing the description of the bike based on its ID
                Label label = new Label();

                //ID / 100       gets the first digit of the ID
                //(ID%100) / 10  gets the second digit of the ID
                //ID % 10        gets the third digit of the ID
                label.Content = ID_Model[ID/100] + " " + ID_Size[ID%100/10] + " " + ID_Color[ID%10];  //creates the content of the label based on the first,second and third digit of the ID
                label.Foreground = Brushes.White;
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, Grid.RowDefinitions.IndexOf(row));
                Grid.Children.Add(label);

                Children[ID].Add(label);


                //creates the stackpanels containing the display of the number of a given type of bike in the cart and the button to add/remove a bike-
                StackPanel stackHor = new StackPanel();
                StackPanel stackVert = new StackPanel();

                stackHor.Orientation = Orientation.Horizontal;
                stackVert.Orientation = Orientation.Vertical;   


                //Adds the textblock displaying the number of bike
                TextBlock text = new TextBlock();
                text.Foreground = Brushes.White;
                text.Text = nb.ToString();
                stackHor.Children.Add(text);
                Children[ID].Add(text);

                //Adds the button to add a bike
                Button buttonUp = new Button();
                buttonUp.Content = "+";
                buttonUp.Click += PlusOne;
                buttonUp.Uid = ID.ToString();

                buttonUp.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
                buttonUp.Foreground = Brushes.White;
                buttonUp.BorderBrush = Brushes.Transparent;

                Children[ID].Add(buttonUp);

                //Adds the button to remove a bike
                Button buttonDown = new Button();
                buttonDown.Content = "-";
                buttonDown.Click += MinusOne;
                buttonDown.Uid = ID.ToString();

                buttonDown.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
                buttonDown.Foreground = Brushes.White;
                buttonDown.BorderBrush = Brushes.Transparent;

                Children[ID].Add(buttonDown);

                stackVert.Children.Add(buttonUp);
                stackVert.Children.Add(buttonDown);
                stackHor.Children.Add(stackVert);

                Children[ID].Add(stackVert);
                Children[ID].Add(stackHor);
                Grid.SetRow(stackHor, Grid.RowDefinitions.IndexOf(row));
                Grid.SetColumn(stackHor, 1);
                Grid.Children.Add(stackHor);


                //Add the button to delete the given bike type order from the cart
                Button Delete = new Button();
                Delete.Width = 40;
                Delete.Background = new SolidColorBrush(Color.FromArgb(0x22, 0x20, 0, 2));
                Delete.Foreground = Brushes.White;
                Delete.BorderBrush = Brushes.Transparent;
                Delete.Content = "X";
                Delete.Uid = ID.ToString();
                Delete.VerticalAlignment = VerticalAlignment.Top;
                Delete.Height = 50;
                Delete.Click += DeleteFromCart;

                Children[ID].Add(Delete);

                Grid.SetRow(Delete, Grid.RowDefinitions.IndexOf(row));
                Grid.SetColumn(Delete, 2);
                Grid.Children.Add(Delete);

            }

            //updates the displayed estimated date of delivery
            UpdateEstimatedDate();
        }


        public void DeleteFromCart(object sender, RoutedEventArgs e)
        {
            ///deletes all the order for a given bike type, removing the row from the display grid in the cart tab and removing the order form the bikeDict dictionary
            Button button = (Button)sender;
            RemoveFromCart(Int16.Parse(button.Uid));
            DeleteRow(Int16.Parse(button.Uid));
            UpdateEstimatedDate();
        }

        public void ClearCart()
        {
            ///remove all the bikes from the cart and clears the display grid in the cart tab
            List<int> IDs = new List<int>();
            foreach(int ID in Rows.Keys)
            {
                IDs.Add(ID);
            }
            foreach(int ID in IDs)
            {
                RemoveFromCart(ID);
                DeleteRow(ID);
            }
            UpdateEstimatedDate();
        }

        public void DeleteRow(int id)
        {
            ///deletes a row from the display grid from the cart tab
            RowDefinition row = Rows[id];
            foreach (UIElement elem in Children[id])
            {
                Grid.Children.Remove(elem);     //to correctly delete a row, it is necessery to delete all its children first
                
            }
            Children.Remove(id);
            Grid.RowDefinitions.Remove(row);
            Rows.Remove(id);
            UpdateEstimatedDate();
        }

        public void MinusOne(object sender, RoutedEventArgs e)
        {
            ///method linked to buttondown, remove 1 bike from the order
            Button button = (Button)sender;
            StackPanel parentStack = (StackPanel)button.Parent;
            StackPanel horStack = (StackPanel)parentStack.Parent;

            TextBlock text = (TextBlock)horStack.Children[0];

            RemoveFromCart(Int16.Parse(button.Uid), 1);
            try
            {
                text.Text = BikeDict[Int16.Parse(button.Uid)].ToString();   //tries updateing the text, need try cathc for when the row is removed due to the number of bike orderd being 0
            }
            catch
            {
                
            }
            UpdateEstimatedDate();
        }

        public void PlusOne(object sender, RoutedEventArgs e)
        {
            ///method linked to buttonup, adds 1 bike from the order
            Button button = (Button)sender;
            StackPanel parentStack = (StackPanel)button.Parent;
            StackPanel horStack = (StackPanel)parentStack.Parent;

            TextBlock text = (TextBlock)horStack.Children[0];

            BikeDict[Int16.Parse(button.Uid)] += 1;
           
            text.Text = BikeDict[Int16.Parse(button.Uid)].ToString();       //updates the text displayed in the corresponding row
            UpdateEstimatedDate();
        }
        public void RemoveFromCart(int ID, int nb)
        {
            //removes a number nb of a given bike type ID from the cart
            if (BikeDict.ContainsKey(ID))
            {
                BikeDict[ID] -= nb;
                if (BikeDict[ID]<= 0){
                    BikeDict.Remove(ID);
                    DeleteRow(ID);
                }
                UpdateEstimatedDate();
            }
        }

        public void RemoveFromCart(int ID)
        {
            //removes all the bike from a given bike type ID
            if (BikeDict.ContainsKey(ID))
            {
                BikeDict.Remove(ID);
                UpdateEstimatedDate();
            }
        }

        public void RegisterCustomer(Customer customer)
        {
            ///Inserts a new custommer in the DB based on the info in the custommer object

            MySqlCommand cmd = new MySqlCommand("INSERT INTO customers (`firstname`, `lastname`, `address`, `phone`, `email`) VALUES (@first, @last, @add, @phone, @mail);", Connection);
            cmd.Parameters.Add(new MySqlParameter("first", customer.FirstName));
            cmd.Parameters.Add(new MySqlParameter("last", customer.LastName));
            cmd.Parameters.Add(new MySqlParameter("add", customer.Address));
            cmd.Parameters.Add(new MySqlParameter("phone", customer.Phone));
            cmd.Parameters.Add(new MySqlParameter("mail", customer.Mail));
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
            }
            reader.Close();

        }

        public int GetPrice()
        {
            ///computes the total price of the cart
            Price = 0;
            foreach (int key in BikeDict.Keys)
            {
                Price += BikeDict[key] * BikePricebyID[key];
            }
            return Price;
        }

        public DateTime GetEstimatedDate()
        {
            ///computes the estimated date of delivery based on the content of the cart and the number of bikes already ordered (by other customer or older oerders)

            DateTime estimatedDate = DateTime.Today;
            double BookedDays = 0;

            //Gets the total number of bikes already ordered
            MySqlCommand cmd = new MySqlCommand("SELECT SUM(amount) from invoice_details;", Connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                BookedDays = Int16.Parse(reader[0].ToString()) / 12;        //computes the number of days needed to complete the production of the already oerdered bikes, based on the fact that the production chain is able to make 12 bikes a day
            }
            reader.Close();

            int roundedbookeddays = (int)Math.Round(BookedDays);

            int orderedBikes = 0;
            foreach (int key in BikeDict.Keys)
            {
                orderedBikes += BikeDict[key];      //computes the total number of bike in this order
            }

            roundedbookeddays += (int)Math.Round((double)orderedBikes / 12);        //computes the number of days needed to complete the production of this number of bikes

            roundedbookeddays += ((int)roundedbookeddays / 7) * 2;                  //adds 2 days every sevend days to account for weekends

            estimatedDate = estimatedDate.AddDays(roundedbookeddays);

            return estimatedDate;
        }

        public void UpdateEstimatedDate()
        {
            ///updates the estimated delivery date dispalyed in the cart tab
            EstimatedDateLabel.Content = $"Estimated delivery date : {GetEstimatedDate().ToString("dd-MM-yyyy")}";
        }

    }

    class Customer
    {
        /// <summary>
        /// Class containing all the info of a customer
        /// </summary>
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public int CustomerNumber { get; set; }


        public Customer(string _First_Name, string _Last_Name, string _Adress, string _mail, string _phone)
        {
            this.FirstName = _First_Name;
            this.LastName = _Last_Name;
            this.Address = _Adress;
            this.Mail = _mail;
            this.Phone = _phone;
            
        }


    }
}
