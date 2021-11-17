using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sprado.Enums.DatabaseMethodResponseEnum;

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

        public static bool ExistsUser(string email)
        {
            bool result = false;
            try
            {
                var command = new MySqlCommand($"SELECT Count(ID) FROM Account WHERE Email='{email}';", connection);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if(reader.GetInt32(0) > 0)
                    {
                        result = true;
                    }
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                result = true; // To not allow user to continue
            }
            return result;
        }

        public static bool CreateUser(string firstName, string lastName, string email, int phone, string password)
        {
            bool success = false;
            LogUtils.Log("Start create user email: " + email);
            try
            {
                var command = new MySqlCommand($"INSERT INTO Account(Firstname, Lastname, Email, Phone, Password, Created) VALUES('{firstName}', '{lastName}', '{email}', {phone}, '{password}', '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}');", connection);
                command.ExecuteNonQuery();
                success = true;
            }
            catch(Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                success = false;
            }
            return success;
        }

        public static Dictionary<string, object> GetUser(string email, string password)
        {
            Dictionary<string, object> result = null;
            try
            {
                var command = new MySqlCommand($"SELECT ID, Firstname, Lastname, Administrator, Profile FROM Account WHERE Email='{email}' AND Password='{password}';", connection);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    result = new Dictionary<string, object>();
                    result.Add("id", reader.GetInt32(0));
                    result.Add("firstname", reader.GetString(1));
                    result.Add("lastname", reader.GetString(2));
                    result.Add("admin", reader.GetBoolean(3));
                    result.Add("profile", reader.GetString(4));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
            }
            return result;
        }

        // CONTACT FORM DATABASE UTILS

        public static DatabaseResponse AddContact(string name, string firstname, string lastname, string mail, int phone, int house_id, bool isOwner, string description)
        {
            string lastEditStatus = "ADD";
            int lastEditAuthor, createAuthor;
            DateTime lastEditDate, createDate;
            lastEditAuthor = createAuthor = Convert.ToInt32(ProgramUtils.LoggedUser["id"]);
            lastEditDate = createDate = DateTime.Now;

            string columns = "Name, Firstname, Lastname, IsOwner, CreateAuthor, CreateDate, LastEditStatus, LastEditAuthor, LastEditDate, ";
            string data = $"'{name}', '{firstname}', '{lastname}', {isOwner}, {createAuthor}, '{createDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{lastEditStatus}', {lastEditAuthor}, '{lastEditDate.ToString("yyyy-MM-dd HH:mm:ss")}', ";

            if(mail != null)
            {
                columns += "Email, ";
                data += $"'{mail}', ";
            }
            if(phone >= 0)
            {
                columns += "Phone, ";
                data += $"{phone}, ";
            }
            if(house_id >= 0)
            {
                columns += "House_ID, ";
                data += $"{house_id}, ";
            }
            if(description != null)
            {
                columns += "Description, ";
                data += $"'{description}', ";
            }

            columns = columns.Substring(0, columns.Length - 2);
            data = data.Substring(0, data.Length - 2);

            try
            {
                var command = new MySqlCommand($"INSERT INTO Contact({columns}) VALUES({data});", connection);
                command.ExecuteNonQuery();
                return DatabaseResponse.CREATED;
            }
            catch(Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                return DatabaseResponse.ERROR;
            }

        }

        // END OF CONTACT FORM UTILS

    }
}
