using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Globalization;

namespace Main.MWM.View
{
    /// <summary>
    /// Interaction logic for StockView.xaml
    /// </summary>
    public partial class StockView : UserControl
    {
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public StockView()
        {
            InitializeComponent();
            FillGrid();
        }

        private void FillGrid()
        {

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Components", conn);

                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();

                adp.Fill(ds, "orderDataBinding");

                stockDataGrid.DataContext = ds;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Failed to connect to data source " + ex).ToString();
            }
            finally
            {
                conn.Close();
            }
        }

        private void UseButton(object sender, RoutedEventArgs e)
        {
            UsePopup.IsOpen = true;
        }

        private void CancelUseButton(object sender, RoutedEventArgs e)
        {
            UsePopup.IsOpen = false;
        }

        private void OrderButton(object sender, RoutedEventArgs e)
        {
            OrderPopup.IsOpen = true;
        }

        private void CancelOrderButton(object sender, RoutedEventArgs e)
        {
            OrderPopup.IsOpen = false;
        }

        private void ParametersButton(object sender, RoutedEventArgs e)
        {
            ParametersPopup.IsOpen = true;
        }

        private void CancelParametersButton(object sender, RoutedEventArgs e)
        {
            ParametersPopup.IsOpen = false;
        }

        private void AddPartButton(object sender, RoutedEventArgs e)
        {
            AddPartPopup.IsOpen = true;
        }

        private void CancelAddPartButton(object sender, RoutedEventArgs e)
        {
            AddPartPopup.IsOpen = false;
        }
    }

    public class PartColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int amount = System.Convert.ToInt32(value);
            int minimumAmount = System.Convert.ToInt32(parameter);
            if (amount - minimumAmount > 10)
            {
                return "Green";
            }

            else if (amount - minimumAmount > 5)
            {
                return "Yellow";
            }
            else
            {
                return "Red";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
