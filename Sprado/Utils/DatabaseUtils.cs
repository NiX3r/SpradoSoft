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
            email = email.Replace("'", "");
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
            firstName = firstName.Replace("'", "");
            lastName = lastName.Replace("'","");
            email = email.Replace("'", "");
            password = password.Replace("'", "");
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
            email = email.Replace("'", "");
            password = password.Replace("'", "");
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
            name = name.Replace("'", "");
            firstname = firstname.Replace("'", "");
            lastname = lastname.Replace("'", "");
            mail = mail.Replace("'", "");
            description = description.Replace("'", "");
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
            name = name.Replace("'", "");
            firstname = firstname.Replace("'", "");
            lastname = lastname.Replace("'", "");
            mail = mail.Replace("'", "");
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
            name = name.Replace("'", "");
            firstname = firstname.Replace("'", "");
            lastname = lastname.Replace("'", "");
            mail = mail.Replace("'", "");
            description = description.Replace("'", "");
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
                                               $"Description='{description}'," +
                                               $"LastEditStatus='EDIT'," +
                                               $"LastEditDate='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                                               $"LastEditAuthor={Convert.ToInt32(ProgramUtils.LoggedUser["id"])}" +
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

            var command = new MySqlCommand("SELECT ID,Firstname,Lastname,Name FROM Contact;", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                output.Add(reader.GetString(1) + " " + reader.GetString(2) + ", " + reader.GetString(3), reader.GetInt32(0));
            }
            reader.Close();
            return output;

        }

        public static DatabaseResponse AddHouse(string city, int zip, string street, int streetNo, int flats, int type, int owner, string description)
        {
            city = city.Replace("'", "");
            street = street.Replace("'", "");
            description = description.Replace("'", "");
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

            city = city.Replace("'", "");
            address = address.Replace("'", "");
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

        public static DatabaseResponse EditHouse(int id, string city, int zip, string street, int streetNo, int flats, int type, int owner, string description)
        {

            city = city.Replace("'", "");
            street = street.Replace("'", "");
            description = description.Replace("'", "");
            DatabaseResponse response;
            try
            {

                string cmd = "UPDATE House SET " +
                             (city == null ? "" : $"City='{city}',") +
                             (zip == -1 ? "" : $"ZipCode={zip},") +
                             (street == null ? "" : $"Street='{street}',") +
                             (streetNo == -1 ? "" : $"StreetNo={streetNo},") +
                             (flats == -1 ? "" : $"Flats={flats},") +
                             (type == -1 ? "" : $"Type={type},") +
                             (owner == -1 ? "" : $"Owner={owner},") +
                             (description == null ? "" : $"Description='{description}',");

                cmd += $"LastEditStatus='EDIT'," +
                       $"LastEditDate='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                       $"LastEditAuthor={Convert.ToInt32(ProgramUtils.LoggedUser["id"])} WHERE ID={id};";

                var command = new MySqlCommand(cmd, connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.EDITED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response = DatabaseResponse.ERROR;
            }
            return response;
        }

        public static List<string> GetContactEmailsByHouseID(int id)
        {
            List<string> response = new List<string>();
            string cmd = $"SELECT Email FROM Contact WHERE House_ID={id};";
            try
            {
                var command = new MySqlCommand(cmd, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    response.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
            }
            return response;
        }

        public static List<string> GetOwnerContactEmailsByHouseID(int id)
        {
            List<string> response = new List<string>();
            string cmd = $"SELECT Email FROM Contact WHERE House_ID={id} AND IsOwner=1;";
            try
            {
                var command = new MySqlCommand(cmd, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    response.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
            }
            return response;
        }

        // END OF HOUSE FORM UTILS

        // REVISION MAN FORM DATABASE UTILS

        public static DatabaseResponse AddRevisionMan(string company, string firstname, string lastname, int phone, string email, string description)
        {
            company = company.Replace("'", "");
            firstname = firstname.Replace("'", "");
            lastname = lastname.Replace("'", "");
            email = email.Replace("'", "");
            description = description.Replace("'", "");
            string lastEditStatus = "ADD";
            int lastEditAuthor, createAuthor;
            DateTime lastEditDate, createDate;
            lastEditAuthor = createAuthor = Convert.ToInt32(ProgramUtils.LoggedUser["id"]);
            lastEditDate = createDate = DateTime.Now;

            string columns = "Company, Firstname, Lastname, Phone, Email, CreateAuthor, CreateDate, LastEditStatus, LastEditDate, LastEditAuthor, ";
            string data = $"'{company}', '{firstname}', '{lastname}', {phone}, '{email}', {createAuthor}, '{createDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{lastEditStatus}', '{lastEditDate.ToString("yyyy-MM-dd HH:mm:ss")}', {lastEditAuthor}, ";

            if (description != "")
            {
                columns += "Description, ";
                data += $"'{description}', ";
            }

            columns = columns.Substring(0, columns.Length - 2);
            data = data.Substring(0, data.Length - 2);

            try
            {
                var command = new MySqlCommand($"INSERT INTO RevisionMan({columns}) VALUES({data});", connection);
                command.ExecuteNonQuery();
                return DatabaseResponse.CREATED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                return DatabaseResponse.ERROR;
            }

        }

        public static Dictionary<int, Dictionary<string, object>> GetRevisionMan(string company, string firstname, string lastname, string email, int phone)
        {

            company = company.Replace("'", "");
            firstname = firstname.Replace("'", "");
            lastname = lastname.Replace("'", "");
            email = email.Replace("'", "");

            Dictionary<int, Dictionary<string, object>> response = new Dictionary<int, Dictionary<string, object>>();

            string cmd = "SELECT * FROM RevisionMan WHERE " +
                         (company == "" ? "" : $"Company LIKE '%{company}%' AND ") +
                         (phone >= 0 ? $"Phone={phone} AND " : "") +
                         (firstname == "" ? "" : $"Firstname LIKE '%{firstname}%' AND ") +
                         (lastname == "" ? "" : $"Lastname LIKE '%{lastname}%' AND ") +
                         (email == "" ? "" : $"Email LIKE '%{email}%' AND ");

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

        public static DatabaseResponse RemoveRevisionMan(int id)
        {
            DatabaseResponse response;
            try
            {
                var command = new MySqlCommand($"DELETE FROM RevisionMan WHERE ID={id};", connection);
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

        public static DatabaseResponse EditRevisionMan(int id, string company, string firstname, string lastname, int phone, string email, string description)
        {

            company = company.Replace("'", "");
            firstname = firstname.Replace("'", "");
            lastname = lastname.Replace("'", "");
            email = email.Replace("'", "");
            description = description.Replace("'", "");
            DatabaseResponse response;
            try
            {

                string cmd = "UPDATE RevisionMan SET " +
                             (company == "" ? "" : $"Company='{company}',") +
                             (firstname == "" ? "" : $"Firstname='{firstname}',") +
                             (lastname == "" ? "" : $"Lastname='{lastname}',") +
                             (phone == -1 ? "" : $"Phone={phone},") +
                             (email == "" ? "" : $"Email='{email}',") +
                             (description == "" ? "" : $"Description='{description}',");

                cmd += $"LastEditStatus='EDIT'," +
                       $"LastEditDate='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                       $"LastEditAuthor={Convert.ToInt32(ProgramUtils.LoggedUser["id"])} WHERE ID={id};";

                var command = new MySqlCommand(cmd, connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.EDITED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response = DatabaseResponse.ERROR;
            }
            return response;
        }

        // END OF REVISION MAN UTILS

        // REVISION TYPE FORM DATABASE UTILS

        public static DatabaseResponse AddRevisionType(string name, int cycle, string description)
        {
            name = name.Replace("'", "");
            description = description.Replace("'", "");

            string lastEditStatus = "ADD";
            int lastEditAuthor, createAuthor;
            DateTime lastEditDate, createDate;
            lastEditAuthor = createAuthor = Convert.ToInt32(ProgramUtils.LoggedUser["id"]);
            lastEditDate = createDate = DateTime.Now;

            string columns = "Name, YearLoop, CreateAuthor, CreateDate, LastEditStatus, LastEditDate, LastEditAuthor, ";
            string data = $"'{name}', {cycle}, {createAuthor}, '{createDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{lastEditStatus}', '{lastEditDate.ToString("yyyy-MM-dd HH:mm:ss")}', {lastEditAuthor}, ";

            if (description != "")
            {
                columns += "Description, ";
                data += $"'{description}', ";
            }

            columns = columns.Substring(0, columns.Length - 2);
            data = data.Substring(0, data.Length - 2);

            try
            {
                var command = new MySqlCommand($"INSERT INTO RevisionType({columns}) VALUES({data});", connection);
                command.ExecuteNonQuery();
                return DatabaseResponse.CREATED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                return DatabaseResponse.ERROR;
            }

        }

        public static Dictionary<int, Dictionary<string, object>> GetRevisionType(string name, int cycle)
        {
            name = name.Replace("'", "");

            Dictionary<int, Dictionary<string, object>> response = new Dictionary<int, Dictionary<string, object>>();

            string cmd = "SELECT * FROM RevisionType WHERE " +
                         (name == "" ? "" : $"Name LIKE '%{name}%' AND ") +
                         (cycle >= 0 ? $"YearLoop={cycle} AND " : "");

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

        public static DatabaseResponse RemoveRevisionType(int id)
        {
            DatabaseResponse response;
            try
            {
                var command = new MySqlCommand($"DELETE FROM RevisionType WHERE ID={id};", connection);
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

        public static DatabaseResponse EditRevisionType(int id, string name, int cycle, string description)
        {
            name = name.Replace("'", "");
            description = description.Replace("'", "");
            DatabaseResponse response;
            try
            {

                string cmd = "UPDATE RevisionType SET " +
                             (name == "" ? "" : $"Name='{name}',") +
                             (cycle == -1 ? "" : $"YearLoop={cycle},") +
                             (description == "" ? "" : $"Description='{description}',");

                cmd += $"LastEditStatus='EDIT'," +
                       $"LastEditDate='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                       $"LastEditAuthor={Convert.ToInt32(ProgramUtils.LoggedUser["id"])} WHERE ID={id};";

                var command = new MySqlCommand(cmd, connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.EDITED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response = DatabaseResponse.ERROR;
            }
            return response;
        }

        // END OF REVISION TYPE UTILS

        // REVISION FORM DATABASE

        public static Dictionary<int, string> GetHouses()
        {

            Dictionary<int, string> output = new Dictionary<int, string>();

            var command = new MySqlCommand("SELECT ID,Street,StreetNo FROM House;", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                output.Add(reader.GetInt32(0), reader.GetString(1) + " " + reader.GetInt32(2));
            }
            reader.Close();
            return output;

        }

        public static Dictionary<int, string> GetRevisionTypes()
        {

            Dictionary<int, string> output = new Dictionary<int, string>();

            var command = new MySqlCommand("SELECT ID,Name FROM RevisionType;", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                output.Add(reader.GetInt32(0), reader.GetString(1));
            }
            reader.Close();
            return output;

        }

        public static Dictionary<int, string> GetRevisionMen()
        {

            Dictionary<int, string> output = new Dictionary<int, string>();

            var command = new MySqlCommand("SELECT ID,Firstname,Lastname FROM RevisionMan;", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                output.Add(reader.GetInt32(0), reader.GetString(1) + " " + reader.GetString(2));
            }
            reader.Close();
            return output;

        }

        public static DatabaseResponse AddRevision(int houseId, int revisionTypeId, int revisionManId, DateTime lastDate, string description)
        {

            description = description.Replace("'", "");
            string lastEditStatus = "ADD";
            int lastEditAuthor, createAuthor;
            DateTime lastEditDate, createDate;
            lastEditAuthor = createAuthor = Convert.ToInt32(ProgramUtils.LoggedUser["id"]);
            lastEditDate = createDate = DateTime.Now;

            string columns = "RevisionMan_ID, RevisionType_ID, House_ID, LastDate, CreateAuthor, CreateDate, LastEditStatus, LastEditDate, LastEditAuthor, ";
            string data = $"{revisionManId}, {revisionTypeId}, {houseId}, '{lastDate.ToString("yyyy-MM-dd")}', {createAuthor}, '{createDate.ToString("yyyy-MM-dd HH:mm:ss")}', '{lastEditStatus}', '{lastEditDate.ToString("yyyy-MM-dd HH:mm:ss")}', {lastEditAuthor}, ";

            if (description != "")
            {
                columns += "Description, ";
                data += $"'{description}', ";
            }

            columns = columns.Substring(0, columns.Length - 2);
            data = data.Substring(0, data.Length - 2);

            try
            {
                var command = new MySqlCommand($"INSERT INTO Revision({columns}) VALUES({data});", connection);
                command.ExecuteNonQuery();
                return DatabaseResponse.CREATED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                return DatabaseResponse.ERROR;
            }

        }

        public static Dictionary<int, Dictionary<string, object>> GetRevision(int houseId, int typeId, int manId, DateTime lastDate, bool useDate)
        {

            Dictionary<int, Dictionary<string, object>> response = new Dictionary<int, Dictionary<string, object>>();

            string cmd = "SELECT * FROM Revision WHERE " +
                         (houseId == -1 ? "" : $"House_ID={houseId} AND ") +
                         (typeId == -1 ? "" : $"RevisionType_ID={typeId} AND ") +
                         (manId == -1 ? "" : $"RevisionMan_ID={manId} AND ") +
                         (useDate ? $"LastDate='{lastDate.ToString("yyyy-MM-dd")}' AND " : "");

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

        public static DatabaseResponse RemoveRevision(int id)
        {
            DatabaseResponse response;
            try
            {
                var command = new MySqlCommand($"DELETE FROM Revision WHERE ID={id};", connection);
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

        public static DatabaseResponse EditRevision(int id, int houseId, int typeId, int manId, DateTime lastDate, bool useDate, string description)
        {
            description = description.Replace("'", "");
            DatabaseResponse response;
            try
            {

                string cmd = "UPDATE Revision SET " +
                             (houseId == -1 ? "" : $"House_ID={houseId},") +
                             (typeId == -1 ? "" : $"RevisionType_ID={typeId},") +
                             (manId == -1 ? "" : $"RevisionMan_ID={manId},") +
                             (useDate ? $"LastDate='{lastDate.ToString("yyyy-MM-dd")}'," : "") +
                             (description == "" ? "" : $"Description='{description}',");

                cmd += $"LastEditStatus='EDIT'," +
                       $"LastEditDate='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}'," +
                       $"LastEditAuthor={Convert.ToInt32(ProgramUtils.LoggedUser["id"])} WHERE ID={id};";

                Clipboard.SetText(cmd);

                var command = new MySqlCommand(cmd, connection);
                command.ExecuteNonQuery();
                response = DatabaseResponse.EDITED;
            }
            catch (Exception ex)
            {
                ProgramUtils.ExceptionThrowned(ex);
                response = DatabaseResponse.ERROR;
            }
            return response;
        }

    }
}
