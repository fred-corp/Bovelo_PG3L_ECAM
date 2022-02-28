using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;


namespace Main
{
    internal class DBConn
    {
        public void testDB()
        {
            MySqlConnection sqldb = new MySqlConnection(@"server=fredcorp.cc;user id=bovelo;pwd=bovelo2022;port=33006;database=bovelo");
            try
            {
                sqldb.Open();

                String sql = "SELECT * FROM Catalog";
                MySqlCommand cmd = new MySqlCommand(sql, sqldb);

                MySqlDataReader myDbReader = null;

                myDbReader = cmd.ExecuteReader();

                Console.WriteLine(myDbReader.ToString());

            }
            catch (Exception ex)
            {
            MessageBox.Show("Failed to connect to data source " + ex).ToString();
            }
        }
    }
}
