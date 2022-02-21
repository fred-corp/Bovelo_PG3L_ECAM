using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Main
{
    internal class DBConn
    {
        private void testDB()
        {
            MySqlConnection sqldb = new MySqlConnection(@"server=;user id=;pwd=;port=;database=");
            try
            {
                sqldb.Open();

                String sql = "SELECT * FROM invoices";
                MySqlCommand cmd = new MySqlCommand(sql, sqldb);

                MySqlDataReader myDbReader = null;

                myDbReader = cmd.ExecuteReader();

                Console.WriteLine(myDbReader.ToString());

            }
        }
    }
}
