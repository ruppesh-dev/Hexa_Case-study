using System;
using Microsoft.Data.SqlClient;
using System;

namespace util
{
    public class DBConnection
    {
        public static SqlConnection GetConnection()
        {
            try
            {
                string connectionString = "Server=localhost; Database=TransportManagementDB; Integrated Security=True;TrustServerCertificate=True;\r\n";
                SqlConnection connection = new SqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting database connection: " + ex.Message);
                throw;
            }
        }
    }
}

