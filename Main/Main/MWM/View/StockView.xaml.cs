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
        private MySqlDataAdapter adp = new MySqlDataAdapter();
        MySqlCommandBuilder cmbl;

        // connection used for database queries
        readonly MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public StockView()
        {
            InitializeComponent();
            FillStockGrid();
            FillOrderGrid();
        }

        /// <summary>
        /// Fill the stock DataGrid from the database
        /// </summary>
        private void FillStockGrid()
        {
            try
            {
                connection.Open();
                adp.SelectCommand = new MySqlCommand(@"SELECT * FROM Components
                LEFT JOIN (SELECT part_number, CAST(SUM(componentlink.amount * production.amount) AS SIGNED) AS production_need
                    FROM componentlink
                    INNER JOIN production
                    ON componentlink.ID = production.ID
                    GROUP BY part_number) AS prod
                ON Components.part_number = prod.part_number", connection);
                cmbl = new MySqlCommandBuilder(adp);
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

        /// <summary>
        /// Fill the part orders DataGrid from the database
        /// </summary>
        private void FillOrderGrid()
        {
            try
            {
                connection.Open();
                adp.SelectCommand = new MySqlCommand(@"SELECT DISTINCT part_orders.*, 
                (SELECT description FROM Components WHERE Components.part_number = part_orders.part_number)
                AS Description FROM part_orders
                INNER JOIN Components
                ON part_orders.part_number = Components.part_number", connection);
                cmbl = new MySqlCommandBuilder(adp);
                DataSet ds = new DataSet();
                adp.Fill(ds, "orderDataBinding");
                orderDataGrid.DataContext = ds;
                connection.Close();
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

        /// <summary>
        /// Returns a part id from it's position in the DataGrid
        /// </summary>
        /// <param name="index">Index of the part in the DataGrid</param>
        /// <returns>An integerer representing the part id</returns>
        private int GetPartNumber(int index)
        {
            TextBlock part = stockDataGrid.Columns[0].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(part.Text) ? 0 : int.Parse(part.Text);
        }

        /// <summary>
        /// Returns a part location from it's position in the DataGrid
        /// </summary>
        /// <param name="index">Index of the part in the DataGrid</param>
        /// <returns>A string representing the part id</returns>
        private string GetPartLocation(int index)
        {
            TextBlock location = stockDataGrid.Columns[2].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(location.Text) ? "" : location.Text;
        }

        /// <summary>
        /// Returns a part quantity from it's position in the DataGrid
        /// </summary>
        /// <param name="index">Index of the part in the DataGrid</param>
        /// <returns>An integerer representing the part quantity</returns>
        private int GetPartQuantity(int index)
        {
            TextBlock quantity = stockDataGrid.Columns[3].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(quantity.Text) ? 0 : int.Parse(quantity.Text);
        }

        /// <summary>
        /// Returns a part minimum quantity from it's position in the DataGrid
        /// </summary>
        /// <param name="index">Index of the part in the DataGrid</param>
        /// <returns>An integerer representing the part minimum quantity</returns>
        private int GetPartMinimumQuantity(int index)
        {
            TextBlock minimumQuantity = stockDataGrid.Columns[4].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(minimumQuantity.Text) ? 0 : int.Parse(minimumQuantity.Text);
        }

        /// <summary>
        /// Returns the last part id from the DataGrid
        /// </summary>
        /// <returns>An integerer representing the part id</returns>
        private int GetLastPartNumber()
        {
            int index = stockDataGrid.Items.Count - 1;
            TextBlock part = stockDataGrid.Columns[0].GetCellContent(stockDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(part.Text) ? 0 : int.Parse(part.Text);
        }

        /// <summary>
        /// Returns a part id from it's position in the DataGrid
        /// </summary>
        /// <param name="index">Index of the part in the DataGrid</param>
        /// <returns>An integerer representing the part id</returns>
        private int GetOrderNumber(int index)
        {
            TextBlock order = orderDataGrid.Columns[0].GetCellContent(orderDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(order.Text) ? 0 : int.Parse(order.Text);
        }

        private void UseButton(object sender, RoutedEventArgs e)
        {
            UsePopup.IsOpen = true;
        }

        private void UpdatePartQuantityDb(int part, int amount)
        {
            string query = "UPDATE Components SET in_stock = @quantity WHERE part_number = @part";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@quantity", amount);
            cmd.Parameters.AddWithValue("@part", part);
            cmd.ExecuteNonQuery();
            connection.Close();
            FillStockGrid();
        }

        private void ConfirmUseButton(object sender, RoutedEventArgs e)
        {
            int part = GetPartNumber(stockDataGrid.SelectedIndex);
            int amount = GetPartQuantity(stockDataGrid.SelectedIndex);
            int newAmount = amount - int.Parse(SetUseAmountTextBox.Text);
            UpdatePartQuantityDb(part, newAmount);
            UsePopup.IsOpen = false;
        }

        private void CancelUseButton(object sender, RoutedEventArgs e)
        {
            UsePopup.IsOpen = false;
        }

        private void OrderButton(object sender, RoutedEventArgs e)
        {
            OrderPopup.IsOpen = true;
        }

        private void CreateOrderDb(int PartNumber, int quantity)
        {
            string query = "INSERT INTO part_orders (part_number, amount, status) VALUES (@PartNumber, @amount, @status)";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@PartNumber", PartNumber);
            cmd.Parameters.AddWithValue("@amount", quantity);
            cmd.Parameters.AddWithValue("@status", "unpaid");
            cmd.ExecuteNonQuery();
            connection.Close();
            FillStockGrid();
        }

        private void UpdateOrderDb()
        {
            // update the modified information on the DataGrid to the database
            adp.Update((DataSet)orderDataGrid.DataContext, "orderDataBinding");
        }

        private void ConfirmOrderButton(object sender, RoutedEventArgs e)
        {
            int part = GetPartNumber(stockDataGrid.SelectedIndex);
            int quantity = int.Parse(SetOrderAmountTextBox.Text);
            CreateOrderDb(part, quantity);
            OrderPopup.IsOpen = false;
            FillOrderGrid();
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

        private void UpdatePartParametersDb(int part, string location,int amount, int MinAmount)
        {
            string query = @"UPDATE Components 
            SET in_stock = @amount, minimum_stock = @MinAmount, location = @location WHERE part_number = @part";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@MinAmount", MinAmount);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@part", part);
            cmd.ExecuteNonQuery();
            connection.Close();
            FillStockGrid();
        }

        private void ConfirmParametersButton(object sender, RoutedEventArgs e)
        {
            string location = SetNewLocationTextBox.Text;
            int amount = int.Parse(SetNewStockAmountTextBox.Text);
            int MinAmount = int.Parse(SetNewMinimumAmountTextBox.Text);
            int part = GetPartNumber(stockDataGrid.SelectedIndex);
            UpdatePartParametersDb(part, location, amount, MinAmount);
            ParametersPopup.IsOpen = false;
        }

        private void CancelParametersButton(object sender, RoutedEventArgs e)
        {
            ParametersPopup.IsOpen = false;
        }

        private void AddPartButton(object sender, RoutedEventArgs e)
        {
            AddPartPopup.IsOpen = true;
        }

        private void AddPartDb(int PartNumber, string description, string location, int amount, int MinAmount)
        {
            string query = @"INSERT INTO Components (part_number, in_stock, minimum_stock, description, location) 
            VALUES (@PartNumber, @amount, @MinAmount, @description, @location)";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@PartNumber", PartNumber);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@MinAmount", MinAmount);
            cmd.Parameters.AddWithValue("@location", location);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.ExecuteNonQuery();
            connection.Close();
            FillStockGrid();
        }

        private void ConfirmAddPartButton(object sender, RoutedEventArgs e)
        {
            int PartNumber = GetLastPartNumber() + 1;
            string description = SetNewPartNameTextBox.Text;
            string location = SetNewPartLocationTextBox.Text;
            int amount = int.Parse(SetNewPartStockTextBox.Text);
            int MinAmount = int.Parse(SetNewPartMinimumAmountTextBox.Text);
            AddPartDb(PartNumber, description, location, amount, MinAmount);
            AddPartPopup.IsOpen = false;
        }

        private void CancelAddPartButton(object sender, RoutedEventArgs e)
        {
            AddPartPopup.IsOpen = false;
        }

        private void ShowOrdersButton(object sender, RoutedEventArgs e)
        {
            StockStackPanel.Visibility = Visibility.Collapsed;
            OrderStackPanel.Visibility = Visibility.Visible;
        }

        private void ShowStockButton(object sender, RoutedEventArgs e)
        {
            OrderStackPanel.Visibility = Visibility.Collapsed;
            StockStackPanel.Visibility = Visibility.Visible;
        }

        private void SaveModificationsButton(object sender, RoutedEventArgs e)
        {
            UpdateOrderDb();
        }

        private void DeleteOrderDb(int order)
        {
            string query = @"UPDATE Components, part_orders 
            SET Components.in_stock = Components.in_stock + part_orders.amount 
            WHERE Components.part_number = part_orders.part_number 
            AND part_orders.order_number = @order; DELETE FROM part_orders 
            WHERE order_number = @order";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@order", order);
            cmd.ExecuteNonQuery();
            connection.Close();
            // update the DataGrids
            FillOrderGrid();
            FillStockGrid();
        }

        private void AddToStockButton(object sender, RoutedEventArgs e)
        {
            int order = GetOrderNumber(orderDataGrid.SelectedIndex);
            DeleteOrderDb(order);
        }
    }

    /// <summary>
    ///  Interface <c>PartColorConverter</c> returns a color based on the difference between 2 quantities
    /// </summary>
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
            else if (value[1] is long)
            {
                minimumAmount = System.Convert.ToInt32(value[1]);
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
