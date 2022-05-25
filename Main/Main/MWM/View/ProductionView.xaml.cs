using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System;

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

        private int GetInvoiceDetailNumber(int index)
        {
            TextBlock order = InvoicesDataGrid.Columns[1].GetCellContent(InvoicesDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(order.Text) ? 0 : int.Parse(order.Text);
        }

        private int GetProductionIdNumber(int index)
        {
            TextBlock order = ProductionDataGrid.Columns[0].GetCellContent(ProductionDataGrid.Items[index]) as TextBlock;

            return string.IsNullOrEmpty(order.Text) ? 0 : int.Parse(order.Text);
        }

        private void ShowScheduleButton(object sender, RoutedEventArgs e)
        {
            MenuStackPanel.Visibility = Visibility.Collapsed;
            ScheduleStackPanel.Visibility = Visibility.Visible;
            GetScheduleDb();
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
                adp.SelectCommand = new MySqlCommand(@"SELECT
                  invoices.invoice_number,
                  invoice_details.invoice_detail_id,
                  CONCAT_WS(' ', `firstname`, `lastname`) AS `name`,
                  invoice_details.amount,
                  invoice_details.status,
                  Catalog.model,
                  Catalog.color,
                  Catalog.size
                FROM invoice_details
                  INNER JOIN invoices
                    ON invoice_details.invoice_number = invoices.invoice_number
                  INNER JOIN customers
                    ON invoices.customer_number = customers.customer_number
                  INNER JOIN Catalog
                    ON invoice_details.ID = Catalog.ID
                  WHERE invoice_details.status = 'waiting'", connection);
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
                adp.SelectCommand = new MySqlCommand(@"SELECT
                  production.production_id,
                  production.invoice_detail_id,
                  production.ID,
                  production.amount,
                  production.amount_scheduled,
                  production.amount_completed,
                  Catalog.model,
                  Catalog.color,
                  Catalog.size
                FROM production
                  INNER JOIN Catalog
                    ON production.ID = Catalog.ID", connection);
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

        private void UpdateInvoiceStatusDb(int invoice, string status)
        {
            string query = "UPDATE invoice_details SET status = @status WHERE invoice_detail_id = @invoice";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@invoice", invoice);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void AddToProductionDb(int invoice)
        {
            string query = "INSERT INTO production (invoice_detail_id, ID, amount) SELECT invoice_detail_id, ID, amount FROM invoice_details WHERE invoice_detail_id = @invoice";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@invoice", invoice);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void SendProductionButton(object sender, RoutedEventArgs e)
        {
            int InvoiceDetailNumber = GetInvoiceDetailNumber(InvoicesDataGrid.SelectedIndex);
            UpdateInvoiceStatusDb(InvoiceDetailNumber, "production");
            AddToProductionDb(InvoiceDetailNumber);
            FillInvoicesGrid();
        }

        private void ScheduleButton(object sender, RoutedEventArgs e)
        {
            SchedulePopup.IsOpen = true;
        }

        private void AddToScheduleDb(int ProductionId, string date)
        {
            string query = "INSERT INTO schedule (production_id, ID, date) VALUES (@production_id, (SELECT ID FROM production WHERE production_id = @production_id), @date)";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@production_id", ProductionId);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.ExecuteNonQuery();

            string query2 = "UPDATE production SET amount_scheduled = amount_scheduled + 1 WHERE production_id = @production_id";
            MySqlCommand cmd2 = new MySqlCommand(query2, connection);
            cmd2.Parameters.AddWithValue("@production_id", ProductionId);
            cmd2.ExecuteNonQuery();
            connection.Close();
            FillProductionGrid();
        }

        private void ConfirmScheduleButton(object sender, RoutedEventArgs e)
        {
            DateTime SelectedDate = (DateTime)ScheduleDatePicker.SelectedDate;
            string FormattedDate = SelectedDate.ToString("yyyy-MM-dd");
            int ProductionId = GetProductionIdNumber(ProductionDataGrid.SelectedIndex);
            AddToScheduleDb(ProductionId, FormattedDate);
            SchedulePopup.IsOpen = false;
        }

        private void CancelScheduleButton(object sender, RoutedEventArgs e)
        {
            SchedulePopup.IsOpen = false;
        }

        private void GetScheduleDb()
        {
            connection.Open();
            adp.SelectCommand = new MySqlCommand("SELECT * FROM schedule", connection);
            cmbl = new MySqlCommandBuilder(adp);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            adp.Fill(ds, "mondayDataBinding");
            MondayListView.DataContext = ds;
            connection.Close();
        }
    }
}
