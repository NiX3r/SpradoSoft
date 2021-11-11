using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprado.Utils
{
    class DatabaseUtils
    {

        // Instance of MySQL connection
        private static MySqlConnection connection;

        public static bool OpenConnection()
        {
            string connectionCredentials = $"server={SecretUtils.GetDatabaseServer()};user={SecretUtils.GetDatabaseUsername()};pwd={SecretUtils.GetDatabasePassword()};database={SecretUtils.GetDatabaseDatabase()}";
            connection = new MySqlConnection(connectionCredentials);
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                return false;
            }
        }

    }
}
