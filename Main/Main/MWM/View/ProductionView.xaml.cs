using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace Main.MWM.View
{
    /// <summary>
    /// Interaction logic for Production.xaml
    /// </summary>
    public partial class ProductionView : UserControl
    {
        private MySqlDataAdapter adp = new MySqlDataAdapter();
        private MySqlCommandBuilder cmbl;
        readonly MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        public ProductionView()
        {
            InitializeComponent();
        }

        private void ShowScheduleButton(object sender, RoutedEventArgs e)
        {
            MenuStackPanel.Visibility = Visibility.Collapsed;
            ScheduleStackPanel.Visibility = Visibility.Visible;
        }

        private void ShowInvoicesButton(object sender, RoutedEventArgs e)
        {
            MenuStackPanel.Visibility = Visibility.Collapsed;
            FillInvoicesGrid();
            InvoicesStackPanel.Visibility = Visibility.Visible;
        }

        private void ShowProductionButton(object sender, RoutedEventArgs e)
        {
            MenuStackPanel.Visibility = Visibility.Collapsed;
            FillProductionGrid();
            ProductionStackPanel.Visibility = Visibility.Visible;
        }

        private void BackButton(object sender, RoutedEventArgs e)
        {
            ScheduleStackPanel.Visibility = Visibility.Collapsed;
            InvoicesStackPanel.Visibility = Visibility.Collapsed;
            ProductionStackPanel.Visibility = Visibility.Collapsed;
            MenuStackPanel.Visibility = Visibility.Visible;
        }

        private void FillInvoicesGrid()
        {
            try
            {
                connection.Open();
                adp.SelectCommand = new MySqlCommand(@"SELECT * FROM invoices", connection);
                cmbl = new MySqlCommandBuilder(adp);
                DataSet ds = new DataSet();
                adp.Fill(ds, "invoicesDataBinding");
                InvoicesDataGrid.DataContext = ds;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to connect to data source " + ex).ToString();
            }
            finally
            {
                connection.Close();
            }
        }

        private void FillProductionGrid()
        {
            try
            {
                connection.Open();
                adp.SelectCommand = new MySqlCommand(@"SELECT * FROM production", connection);
                cmbl = new MySqlCommandBuilder(adp);
                DataSet ds = new DataSet();
                adp.Fill(ds, "productionDataBinding");
                ProductionDataGrid.DataContext = ds;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to connect to data source " + ex).ToString();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
