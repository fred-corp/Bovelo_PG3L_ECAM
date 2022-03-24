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


namespace Main.MWM.View
{
    /// <summary>
    /// Interaction logic for Production.xaml
    /// </summary>
    public partial class ProductionView : UserControl
    {
        List<string> _Available = new List<string>();
        Grid grid = new Grid();
        DatePicker Picker = new DatePicker();
        Button Planning = new Button();
        Button Hide = new Button();
        Button AddCommand = new Button();
        Button DisplayStock = new Button();
        TextBox textBox = new TextBox();
        public string Text { get; set; }


        public ProductionView()
        {
            

            InitializeComponent();
            _Available.Add("Yes");
            _Available.Add("NO");
            TabItem tab = new TabItem();
            tab.Content = GetGrid();
            MainTabControl.Items.Add(tab);


            grid.Name = "GridProduction"; ;

            ColumnDefinition Column1 = new ColumnDefinition();
            ColumnDefinition Column2 = new ColumnDefinition();
            ColumnDefinition Column3 = new ColumnDefinition();
            ColumnDefinition Column4 = new ColumnDefinition();

            RowDefinition Row1 = new RowDefinition();
            RowDefinition Row2 = new RowDefinition();
            RowDefinition Row3 = new RowDefinition();
            RowDefinition Row4 = new RowDefinition();

            grid.ColumnDefinitions.Add(Column1);
            grid.ColumnDefinitions.Add(Column2);
            grid.ColumnDefinitions.Add(Column3);
            //grid.ColumnDefinitions.Add(Column4);

            grid.RowDefinitions.Add(Row1);
            grid.RowDefinitions.Add(Row2);
            grid.RowDefinitions.Add(Row3);
            //grid.RowDefinitions.Add(Row4);

            

            ///Searching input 
            //textBox.TextChanged += GetText;
            //textBox.KeyDown += GetText;

            Grid.SetRow(textBox, 0);
            Grid.SetColumn(textBox, 0);
            grid.Children.Add(textBox);

            ///DisplayStock Button
            Grid.SetColumn(DisplayStock, 1);
            Grid.SetRow(DisplayStock, 0);
            DisplayStock.Content = "DisplayStock";
            DisplayStock.Name = "DisplayStock";
            //DisplayStock.Click += Displaystock;
            //grid.Children.Add(DisplayStock);

            //Planning button
            Grid.SetColumn(Planning, 2);
            Grid.SetRow(Planning, 0);
            Planning.Content = "Calendar ";
            Planning.Name = "Planning";
            Planning.Click += DisplayPlanning;
            grid.Children.Add(Planning);

            //ADD COMMAND 
            Grid.SetColumn(AddCommand, 1);
            Grid.SetRow(AddCommand, 1);
            AddCommand.Content = "AddCommand";
            AddCommand.Name = "AddCommand";
            AddCommand.Click += AddComand;
            grid.Children.Add(AddCommand);


            
        }

        public Grid GetGrid()
        {
     
            return grid;

        }

        public void DisplayPlanning(object sender, RoutedEventArgs e)
            //
        {
            //Remove planning button
            grid.Children.Remove(Planning);

            //DatePicker
            Picker.Name = "Picker";
            String Text = Picker.Text;
            //Picker.SelectedDateChanged += ShowDate;
            //Console.WriteLine(Text);
            grid.Children.Add(Picker);
            Grid.SetColumn(Picker, 2);
            Grid.SetRow(Picker, 0);

            
            ///Hide Calendar
            Grid.SetColumn(Hide, 2);
            Grid.SetRow(Hide, 1);
            Hide.Content = "Hide Calendar";
            Hide.Name = "Hide";
            Hide.Click += HideCalendar;
            grid.Children.Add(Hide);
            
        }
        
        private void HideCalendar(object sender, RoutedEventArgs e)
        {
            
            grid.Children.Remove(Picker);
            //Planning button
            Grid.SetColumn(Planning, 2);
            Grid.SetRow(Planning, 0);
            grid.Children.Add(Planning);
            Console.WriteLine("Hide");
            grid.Children.Remove(Hide);

        }
        /*
        private void ShowDate(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(sender.ToString());  //Date selectionnée dans la calendrier 
        }
        
        private void GetText(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Enter) 
            {
                Console.WriteLine(sender.ToString());

            }
            

        }
        private void Displaystock(object sender, RoutedEventArgs e)
        {
            
            Console.WriteLine("DisplayStock");
        }*/
        private void  AddComand(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(Picker.Text);
            Console.WriteLine(textBox.Text);


            Console.WriteLine("Add Command");
            

        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
