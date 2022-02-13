using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public static Dictionary<int, Dictionary<string, object>> GetContact(string name, string firstname, string lastname, string mail, int phone, int house_id)
        {
            Dictionary<int, Dictionary<string, object>> response = new Dictionary<int, Dictionary<string, object>>();
            string cmd = $"SELECT * FROM Contact WHERE " +
                         (name.Equals("") ? "" : $"Name LIKE '%{name}%' AND ") +
                         (firstname.Equals("") ? "" : $"Firstname LIKE '%{firstname}%' AND ") +
                         (lastname.Equals("") ? "" : $"Lastname LIKE '%{lastname}%' AND ") +
                         (mail.Equals("") ? "" : $"Email LIKE '%{mail}%' AND ") +
                         (phone == -1 ? "" : "Phone=" + phone + " AND ") +
                         (house_id == -1 ? "" : "House_ID=" + house_id + " AND ");
            if (cmd.Substring(cmd.Length - 5).Equals(" AND "))
                cmd = cmd.Substring(0, cmd.Length - 5);
            MessageBox.Show(cmd);
            try
            {
                var command = new MySqlCommand(cmd, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        data.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    response.Add(id, data);
                }
                reader.Close();
            }
            catch(Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
            }
            return response;
        }

        public static DatabaseResponse RemoveContact(int id)
        {
            DatabaseResponse response;
            try
            {
                var command = new MySqlCommand($"DELETE FROM Contact WHERE ID={id};", connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.REMOVED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response = DatabaseResponse.ERROR;
            }
            return response;
        }

        public static DatabaseResponse EditContact(int id, string name, string firstname, string lastname, string mail, int phone, int house_id, bool isOwner, string description)
        {
            DatabaseResponse response;
            try
            {
                var command = new MySqlCommand($"UPDATE Contact SET " +
                                               $"Name='{name}'," +
                                               $"Firstname='{firstname}'," +
                                               $"Lastname='{lastname}'," + 
                                               $"Email='{mail}'," +
                                               (phone == -1 ? "" : $"Phone={phone},") +
                                               (house_id == -1 ? "" : $"House_ID={house_id},") +
                                               $"IsOwner={isOwner}," +
                                               $"Description='{description}'" +
                                               $" WHERE ID={id};", connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.EDITED;
            }
            catch(Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response= DatabaseResponse.ERROR;
            }
            return response;
        }

        // END OF CONTACT FORM UTILS

        // HOUSE FORM DATABASE UTILS

        public static Dictionary<string, int> GetContacts()
        {
            
            Dictionary<string,int> output = new Dictionary<string,int>();

            var command = new MySqlCommand("SELECT ID,Firstname,Lastname FROM Contact;", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                output.Add(reader.GetString(1) + " " + reader.GetString(2), reader.GetInt32(0));
            }
            reader.Close();
            return output;

        }

        public static DatabaseResponse AddHouse(string city, int zip, string street, int streetNo, int flats, int type, int owner, string description)
        {

            string lastEditStatus = "ADD";
            int lastEditAuthor, createAuthor;
            DateTime lastEditDate, createDate;
            lastEditAuthor = createAuthor = Convert.ToInt32(ProgramUtils.LoggedUser["id"]);
            lastEditDate = createDate = DateTime.Now;

            string columns = "City, ZipCode, Street, StreetNo, Flats, Type, Owner, CreateAuthor, CreateDate, LastEditStatus, LastEditDate, LastEditAuthor, ";
            string data = $"'{city}', {zip}, '{street}', {streetNo}, {flats}, {type}, {owner}, {createAuthor}, '{createDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{lastEditStatus}', '{lastEditDate.ToString("yyyy-MM-dd HH:mm:ss")}', {lastEditAuthor}, ";

            if (description != "")
            {
                columns += "Description, ";
                data += $"'{description}', ";
            }

            columns = columns.Substring(0, columns.Length - 2);
            data = data.Substring(0, data.Length - 2);

            try
            {
                var command = new MySqlCommand($"INSERT INTO House({columns}) VALUES({data});", connection);
                command.ExecuteNonQuery();
                return DatabaseResponse.CREATED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                return DatabaseResponse.ERROR;
            }

        }

        public static Dictionary<int, Dictionary<string, object>> GetHouse(string address, int addressNo, string city, int zip, int ownerId)
        {

            Dictionary<int, Dictionary<string, object>> response = new Dictionary<int, Dictionary<string,object>>();

            string cmd = "SELECT * FROM House WHERE " +
                         (address == "" ? "" : $"Street LIKE '%{address}%' AND ") +
                         (addressNo >= 0 ? $"StreetNo={addressNo} AND " : "") +
                         (city == "" ? "" : $"City LIKE '%{city}%' AND ") +
                         (zip >= 0 ? $"ZipCode={zip} AND " : "") +
                         (ownerId >= 0 ? $"Owner={ownerId} AND " : "");

            if (cmd.Substring(cmd.Length - 5).Equals(" AND "))
                cmd = cmd.Substring(0, cmd.Length - 5);
            MessageBox.Show(cmd);
            try
            {
                var command = new MySqlCommand(cmd, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    Dictionary<string, object> data = new Dictionary<string, object>();
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        data.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    response.Add(id, data);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
            }
            return response;

        }

        public static DatabaseResponse RemoveHouse(int id)
        {
            DatabaseResponse response;
            try
            {
                var command = new MySqlCommand($"DELETE FROM House WHERE ID={id};", connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.REMOVED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response = DatabaseResponse.ERROR;
            }
            return response;
        }

        // END OF HOUSE FORM UTILS

    }
}
