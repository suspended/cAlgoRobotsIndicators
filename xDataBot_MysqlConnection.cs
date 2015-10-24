using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

//Add MySql Library
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Diagnostics;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class xDataBot : Robot
    {
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }

        // INSTALL MYSQL CONNECTOR/NET
        // ADD MENAGED REFERENCES MYSQL.DATA from libraries browse
        // Super example (with backup and restore mysql database):
        // http://www.codeproject.com/Articles/43438/Connect-C-to-MySQL

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
            database = "fxstareu";
            uid = "root";
            password = "toor";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
            } catch (MySqlException ex)
            {
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


            // RUN WEB BROWSER with my web page
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "IExplore.exe";
            psi.Arguments = "forex.fxstar.eu";
            Process.Start(psi);
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

        //Insert statement
        public void Insert(string Currency = "GBPJPY")
        {
            // Current UNIX timestamp
            //Int32 Stamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            // Put your core logic here
            MarketSeries data = MarketData.GetSeries(Currency, TimeFrame.Weekly);
            DataSeries series = data.Close;
            int index = series.Count - 1;

            double close = data.Close[index];
            double high = data.High[index];
            double low = data.Low[index];
            double open = data.Open[index];
            Int32 opentime = (Int32)(data.OpenTime[index].Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            Print(open);

            // mysql query
            //string query = "INSERT INTO account (time, accountid, balance, equity) VALUES('" + Stamp + "','" + Account.Number + "', '" + Account.Balance + "', '" + Account.Equity + "')";
            string query = "INSERT INTO " + Currency + " (time, open, close, low, high) VALUES('" + opentime + "','" + open + "', '" + close + "', '" + low + "', '" + high + "')";

            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);

            //Execute command
            cmd.ExecuteNonQuery();
        }
        //Select statement
        public List<string>[] Select()
        {
            string query = "SELECT * FROM account";

            //Create a list to store the result
            List<string>[] list = new List<string>[3];
            list[0] = new List<string>();
            list[1] = new List<string>();
            list[2] = new List<string>();

            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                list[0].Add(dataReader["accountid"] + "");
                list[1].Add(dataReader["balance"] + "");
                list[2].Add(dataReader["equity"] + "");
            }

            //close Data Reader
            dataReader.Close();

            //return list to be displayed
            return list;
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM account";
            int Count = -1;
            //Create Mysql Command
            MySqlCommand cmd = new MySqlCommand(query, connection);

            //ExecuteScalar will return one value
            Count = int.Parse(cmd.ExecuteScalar() + "");

            return Count;
        }


    }
}
