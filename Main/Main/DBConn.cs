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
    class DBConn
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


                Console.WriteLine("got to read data !");

                while (myDbReader.Read())
                {
                    var myString = myDbReader.GetString(0); //The 0 stands for "the 0'th column", so the first column of the result.
                                                     // Do somthing with this rows string, for example to put them in to a list
                    Console.WriteLine(myString);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to connect to data source " + ex);
            }
        }
    }
}
