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
            SundayListView.Items.Clear();
            MondayListView.Items.Clear();
            TuesdayListView.Items.Clear();
            WednesdayListView.Items.Clear();
            ThursdayListView.Items.Clear();
            FridayListView.Items.Clear();
            SaturdayListView.Items.Clear();

            connection.Open();
            MySqlCommand cmd = new MySqlCommand(@"SELECT
              bike_id,
              DAYOFWEEK(date) 
            FROM schedule
            WHERE schedule.date >= date_sub(curdate(), interval weekday(curdate()) day)
            AND schedule.date <= date_sub(curdate(), interval weekday(curdate()) - 7 day)", connection);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Button btn = new Button();
                btn.Content = dr[0].ToString();
                btn.Height = 20;
                btn.Width = 80;
                btn.Click += new RoutedEventHandler(BikeInfoButton);

                if (dr[1].ToString() == "1")
                {
                    SundayListView.Items.Add(btn);
                }
                else if (dr[1].ToString() == "2")
                {
                    
                    MondayListView.Items.Add(btn);
                }
                else if (dr[1].ToString() == "3")
                {
                    TuesdayListView.Items.Add(btn);
                }
                else if (dr[1].ToString() == "4")
                {
                    WednesdayListView.Items.Add(btn);
                }
                else if (dr[1].ToString() == "5")
                {
                    ThursdayListView.Items.Add(btn);
                }
                else if (dr[1].ToString() == "6")
                {
                    FridayListView.Items.Add(btn);
                }
                else if (dr[1].ToString() == "7")
                {
                    SaturdayListView.Items.Add(btn);
                }
            }
            connection.Close();
        }

        private void BikeInformationDb(int BikeId)
        {
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(@"SELECT
              Catalog.model,
              Catalog.color,
              Catalog.size,
              production_ID,
              schedule.bike_id
            FROM schedule
              INNER JOIN Catalog
                ON schedule.ID = Catalog.ID
            WHERE schedule.bike_id = @id", connection);
            cmd.Parameters.AddWithValue("@id", BikeId);
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                BikeModelLabel.Content = dr[0].ToString();
                BikeColorLabel.Content = dr[1].ToString();
                BikeSizeLabel.Content = dr[2].ToString();
                ProductionIdLabel.Content = dr[3].ToString();
                BikeIdLabel.Content = dr[4].ToString();
            }
            connection.Close();
        }

        private void BikeInfoButton(object sender, RoutedEventArgs e)
        {
            string content = (e.Source as Button).Content.ToString();
            int BikeId = int.Parse(content);
            ValidateBikePopup.IsOpen = true;
            BikeInformationDb(BikeId);
        }

        private void CompleteBikeDb(int BikeId)
        {
            int ProductionId = Convert.ToInt32(ProductionIdLabel.Content);
            string query = @"UPDATE production p 
              SET 
                p.amount_scheduled = p.amount_scheduled - 1,
                p.amount_completed = p.amount_completed + 1 
            WHERE p.production_ID = (SELECT production_id FROM schedule WHERE schedule.bike_id = @bike_id); 
            DELETE FROM schedule WHERE bike_id = @bike_id";
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@bike_id", BikeId);
            cmd.ExecuteNonQuery();

            MySqlCommand cmd2 = new MySqlCommand(@"SELECT amount, amount_completed 
            FROM production WHERE production_ID = @production_id", connection);
            cmd2.Parameters.AddWithValue("@production_id", ProductionId);
            MySqlDataReader dr = cmd2.ExecuteReader();
            bool CompletedOrder = false;
            while (dr.Read())
            {
                if (Convert.ToInt32(dr[0]) == Convert.ToInt32(dr[1]))
                {
                    CompletedOrder = true;
                    dr.Close();
                }
                break;
            }
            if (CompletedOrder is true)
            {
                MySqlCommand cmd3 = new MySqlCommand(@"UPDATE invoice_details
                  INNER JOIN
                  production ON invoice_details.invoice_detail_id = production.invoice_detail_id
                SET status = 'complete' 
                  WHERE production.production_ID = @production_id;
                SET FOREIGN_KEY_CHECKS=0;
                DELETE FROM production WHERE production_ID = @production_id;
                SET FOREIGN_KEY_CHECKS=1;", connection);
                cmd3.Parameters.AddWithValue("@production_id", ProductionId);
                cmd3.ExecuteNonQuery();
            }
            connection.Close();
        }

        private void ValidateBikeButton(object sender, RoutedEventArgs e)
        {
            CompleteBikeDb(Convert.ToInt32(BikeIdLabel.Content));
            GetScheduleDb();
            ValidateBikePopup.IsOpen = false;
        }

        private void CloseValidatePopupButton(object sender, RoutedEventArgs e)
        {
            ValidateBikePopup.IsOpen = false;
        }
    }
}
