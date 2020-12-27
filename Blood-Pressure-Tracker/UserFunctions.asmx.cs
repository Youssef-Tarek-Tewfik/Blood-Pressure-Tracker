using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

namespace Blood_Pressure_Tracker
{
    /// <summary>
    /// Summary description for UserFunctions
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class UserFunctions : System.Web.Services.WebService
    {
        //private const string conString = "Data Source=lenovo-t540p\\sqlexpress;AttachDbFilename=\"C:\\Users\\Youssef\\New folder\\Workspace\\.NET\\Blood Pressure Tracker\\Blood-Pressure-Tracker\\Database\\BloodPressureTracker.mdf\";Integrated Security=True";
        //private const string conString = "Data Source=DESKTOP-4RKI3HL\\SQLEXPRESS;AttachDbFilename=\"C:\\Users\\Public\\Downloads\\Blood Pressure Tracker\\Blood-Pressure-Tracker\\Database\\BloodPressureTracker.mdf\";Integrated Security=True";
        private const string conString = "Data Source=lenovo-t540p\\sqlexpress;AttachDbFilename=\"C:\\Users\\Youssef\\New folder\\Workspace\\.NET\\Blood Pressure Tracker\\Blood-Pressure-Tracker\\Database\\BloodPressureTracker.mdf\";Integrated Security=True";

        //DESKTOP-4RKI3HL\SQLEXPRESS
        [WebMethod]
        public bool Register(string email, string password, string name, int age, float weight, char gender)
        {
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            string commandString = "insert into Users values(@email, @password, @name, @age, @weight, @gender)";
            SqlCommand command = new SqlCommand(commandString, con);
            command.Parameters.Add(new SqlParameter("@email", email));
            command.Parameters.Add(new SqlParameter("@password", password));
            command.Parameters.Add(new SqlParameter("@name", name));
            command.Parameters.Add(new SqlParameter("@age", age));
            command.Parameters.Add(new SqlParameter("@weight", weight));
            command.Parameters.Add(new SqlParameter("@gender", gender));
            int ret = command.ExecuteNonQuery();

            if (ret != 1)
                return false;

            //commandString = "insert into BPMeasurements (Email, Measurement) values (@e, @m)";
            //command = new SqlCommand(commandString, con);
            //command.Parameters.Add(new SqlParameter("@e", email));
            //command.Parameters.Add(new SqlParameter("@m", bloodPressure));
            //command.ExecuteNonQuery();

            con.Close();
            return ret == 1;
        }

        [WebMethod]
        public bool Login(string email, string password)
        {
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            string commandString = "select password from Users where email=@e;";
            SqlCommand command = new SqlCommand(commandString, con);
            command.Parameters.Add(new SqlParameter("@e", email));

            try
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    bool ret = (password == (string)reader[0]);
                    con.Close();
                    return ret;
                }
            }
            catch
            {
                con.Close();
                return false;
            }
            con.Close();
            return false;
        }

        [WebMethod]
        public bool Update(string email, string password, string name, int age, float weight, string bloodPressure)
        {
            SqlConnection con = new SqlConnection(conString);
            try
            {
                con.Open();
                string commandString = "Update Users set ";
                SqlCommand command = new SqlCommand(commandString, con);
                if (!password.Equals("null"))
                {
                    command.CommandText += "Password=@p";
                    command.Parameters.AddWithValue("@p", password);
                }
                if (!name.Equals("null"))
                {
                    if (command.CommandText.Equals("Update Users set "))
                        command.CommandText += "Name=@n";
                    else
                        command.CommandText += ", Name=@n";
                    command.Parameters.AddWithValue("@n", name);
                }
                if (age != 0)
                {
                    if (command.CommandText.Equals("Update Users set "))
                        command.CommandText += "Age@a";
                    else
                        command.CommandText += ", Age=@a";
                    command.Parameters.AddWithValue("@a", age);
                }
                if (weight != 0)
                {
                    if (command.CommandText.Equals("Update Users set "))
                        command.CommandText += "Weight=@w";
                    else
                        command.CommandText += ", Weight=@w";
                    command.Parameters.AddWithValue("@w", weight);
                }
                if (!command.CommandText.Equals("Update Users set "))
                {
                    command.CommandText += " where email=@e;";
                    command.Parameters.AddWithValue("@e", email);
                    command.ExecuteNonQuery();
                }

                if (!bloodPressure.Equals("null"))
                {
                    commandString = "insert into BPMeasurements (Email, Measurement) values (@e, @m)";
                    command = new SqlCommand(commandString, con);
                    command.Parameters.Add(new SqlParameter("@e", email));
                    command.Parameters.Add(new SqlParameter("@m", bloodPressure));
                    command.ExecuteNonQuery();
                }

            }
            catch
            {
                con.Close();
                return false;
            }

            con.Close();
            return true;
        }

        [WebMethod]
        public DataSet ViewMeasurements(string email)
        {
            SqlConnection connection = new SqlConnection(conString);
            connection.Open();
            var statement = "SELECT Measurement, Time FROM BPMeasurements WHERE Email=@e";

            var dataAdapter = new SqlDataAdapter(statement, connection);
            dataAdapter.SelectCommand.Parameters.AddWithValue("@e", email);
            var ds = new DataSet();
            dataAdapter.Fill(ds);

            connection.Close();
            return ds;
        }

        [WebMethod]

        public bool PostMeasurements(string email, string bloodPressure, DateTime dt)
        {
            SqlConnection con = new SqlConnection(conString);
            con.Open();
            string commandString = "insert into BPMeasurements (Email, Measurement, Time) values (@e, @m, @t)";
            SqlCommand command = new SqlCommand(commandString, con);
            command.Parameters.Add(new SqlParameter("@e", email));
            command.Parameters.Add(new SqlParameter("@m", bloodPressure));
            command.Parameters.Add(new SqlParameter("@t", dt));
            int ret = command.ExecuteNonQuery();
            con.Close();
            return ret == 1;
        }

        [WebMethod]
        public string[] ViewInformation(string email)
        {
            string[] ret = new string[5];
            string statement = "select Password, Name, Age, Weight, Gender from Users where Email=@e;";
            SqlConnection connection = new SqlConnection(conString);
            connection.Open();
            SqlCommand command = new SqlCommand(statement, connection);
            command.Parameters.AddWithValue("@e", email);

            try
            {
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ret[0] = (string)reader[0];
                    ret[1] = (string)reader[1];
                    ret[2] = reader.GetInt32(2).ToString();
                    ret[3] = reader.GetDouble(3).ToString();
                    ret[4] = (string)reader[4];
                }
            }
            catch { }
            connection.Close();
            return ret;
        }
    }
}
