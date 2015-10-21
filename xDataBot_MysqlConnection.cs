using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

//Add MySql Library
using MySql.Data.MySqlClient;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class xDataBot : Robot
    {
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }

        // INSTALL MYSQL CONNECTOR/NET
        // ADD MENAGED REFERENCES MYSQL.DATA from libraries browse         

        // Mysql COnnection variables
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        protected override void OnStart()
        {
            // MYSQL TABLES
            //create table account(id INT NOT NULL AUTO_INCREMENT,time int, accountid int, balance float(10,2),equity float(10,2),margin float(10,2),freemargin float(10,2), currency varchar(20), leverage int, PRIMARY KEY(id));
            server = "localhost";
            database = "dbname";
            uid = "user";
            password = "pass";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
            } catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Print("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Print("Invalid username/password, please try again");
                        break;
                    default:
                        Print("Connected");
                        break;
                }
            }
        }

        protected override void OnBar()
        {
            // Insert into mysql database
            Insert();
        }

        protected override void OnStop()
        {
            Print("cBot was stoppppped.");
        }

        //Insert statement
        public void Insert()
        {
            // CUrrent UNIX timestamp
            Int32 Stamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // mysql query
            string query = "INSERT INTO account (time, accountid, balance, equity) VALUES('" + Stamp + "','" + Account.Number + "', '" + Account.Balance + "', '" + Account.Equity + "')";

            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);

            //Execute command
            cmd.ExecuteNonQuery();

        }

    }
}
