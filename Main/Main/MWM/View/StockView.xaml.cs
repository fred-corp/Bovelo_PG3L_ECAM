using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.Diagnostics;
using System.Windows.Media;

namespace Main.MWM.View
{
    /// <summary>
    /// Interaction logic for StockView.xaml
    /// </summary>
    public partial class StockView : UserControl
    {
        readonly MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public StockView()
        {
            InitializeComponent();
            FillGrid();
        }

        private void FillGrid()
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Components", connection);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "stockDataBinding");
                stockDataGrid.DataContext = ds;
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

        private int GetPartNumber(int index)
        {
            TextBlock part = stockDataGrid.Columns[0].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(part.Text) ? 0 : int.Parse(part.Text);
        }

        private string GetPartLocation(int index)
        {
            TextBlock location = stockDataGrid.Columns[2].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(location.Text) ? "" : location.Text;
        }

        private int GetPartQuantity(int index)
        {
            TextBlock quantity = stockDataGrid.Columns[3].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(quantity.Text) ? 0 : int.Parse(quantity.Text);
        }

        private int GetPartMinimumQuantity(int index)
        {
            TextBlock minimumQuantity = stockDataGrid.Columns[4].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(minimumQuantity.Text) ? 0 : int.Parse(minimumQuantity.Text);
        }

        private void UseButton(object sender, RoutedEventArgs e)
        {
            UsePopup.IsOpen = true;
        }

        private void UpdatePartQuantity(int part, int amount)
        {
            string query = "UPDATE Components SET in_stock = @quantity WHERE part_number = @part";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@quantity", amount);
            cmd.Parameters.AddWithValue("@part", part);
            cmd.ExecuteNonQuery();
            connection.Close();
            FillGrid();
        }

        private void ConfirmUseButton(object sender, RoutedEventArgs e)
        {
            int part = GetPartNumber(stockDataGrid.SelectedIndex);
            int currentAmount = GetPartQuantity(stockDataGrid.SelectedIndex);
            int newAmount = currentAmount - int.Parse(SetUseAmountTextBox.Text);
            UpdatePartQuantity(part, newAmount);
            UsePopup.IsOpen = false;
        }

        private void CancelUseButton(object sender, RoutedEventArgs e)
        {
            UsePopup.IsOpen = false;
        }

        private void OrderButton(object sender, RoutedEventArgs e)
        {
            // todo
            OrderPopup.IsOpen = true;
        }

        private void CancelOrderButton(object sender, RoutedEventArgs e)
        {
            OrderPopup.IsOpen = false;
        }

        private void ParametersButton(object sender, RoutedEventArgs e)
        {
            ParametersPopup.IsOpen = true;
            SetNewLocationTextBox.Text = GetPartLocation(stockDataGrid.SelectedIndex);
            SetNewStockAmountTextBox.Text = GetPartQuantity(stockDataGrid.SelectedIndex).ToString();
            SetNewMinimumAmountTextBox.Text = GetPartMinimumQuantity(stockDataGrid.SelectedIndex).ToString();
        }

        private void UpdatePartParameters(int part, string location,int amount, int MinAmount)
        {
            string query = "UPDATE Components SET in_stock = @amount, minimum_stock = @MinAmount, location = @location WHERE part_number = @part";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@MinAmount", MinAmount);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@part", part);
            cmd.ExecuteNonQuery();
            connection.Close();
            FillGrid();
        }

        private void ConfirmParametersButton(object sender, RoutedEventArgs e)
        {
            string location = SetNewLocationTextBox.Text;
            int amount = int.Parse(SetNewStockAmountTextBox.Text);
            int MinAmount = int.Parse(SetNewMinimumAmountTextBox.Text);
            int part = GetPartNumber(stockDataGrid.SelectedIndex);
            UpdatePartParameters(part, location, amount, MinAmount);
            ParametersPopup.IsOpen = false;
        }

        private void CancelParametersButton(object sender, RoutedEventArgs e)
        {
            ParametersPopup.IsOpen = false;
        }

        private void AddPartButton(object sender, RoutedEventArgs e)
        {
            // todo
            AddPartPopup.IsOpen = true;
        }

        private void CancelAddPartButton(object sender, RoutedEventArgs e)
        {
            AddPartPopup.IsOpen = false;
        }
    }

    public class PartColorConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            int amount;
            int minimumAmount;
            if (value[0] is int)
            {
                amount = (int)value[0];
            }
            else
            {
                return Brushes.White;
            }
            if (value[1] is int)
            {
                minimumAmount = (int)value[1];
            }
            else
            {
                return Brushes.White;
            }

            if (amount - minimumAmount > 10)
            {
                return Brushes.Green;
            }

             else if (amount - minimumAmount > 5)
            {
                return Brushes.Yellow;
            }
            else
            {
                return Brushes.Red;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
