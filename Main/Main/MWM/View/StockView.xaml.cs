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
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

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

        private void AddPart(object sender, RoutedEventArgs e)
        {
            addPartPopup.IsOpen = true;
        }
        private void CancelPart(object sender, RoutedEventArgs e)
        {
            addPartPopup.IsOpen = false;
        }
        private void FillGrid()
        {

            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Components", conn);

                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);

                DataSet ds = new DataSet();

                adp.Fill(ds, "LoadDataBinding");

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

    }

}
