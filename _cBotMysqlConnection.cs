# Download and install Mysql Connector/.NET from:
# https://dev.mysql.com/downloads/connector/net/6.9.html
# and add reference file Mysql.Data

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;
using System.Diagnostics;

//Add MySql Library
using MySql.Data.MySqlClient;



namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class FxStarEu : Robot
    {
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }

        // Mysql COnnection variables
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;



        protected override void OnStart()
        {
            /*
            private Regression cog;
            cog = Indicators.GetIndicator<Regression>(3, 2000, 2);
            double up = cog.sqh.LastValue;
            double zero = cog.prc.LastValue;
            double dn = cog.sql.LastValue;
            Print("Regression: " + up + " low " + dn);
            */


            // MYSQL TABLES            
            //create table account(id INT NOT NULL AUTO_INCREMENT,time int, accountid int, balance float(10,2),equity float(10,2),margin float(10,2),freemargin float(10,2), currency varchar(20), leverage int, PRIMARY KEY(id));
            server = "localhost";
            database = "baza";
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

            // GBPJPY OHLC
            try
            {
                Insert("GBPJPY");
            } catch (Exception ee)
            {
                Print(ee);
            }

            try
            {
                Balance();
            } catch (Exception ee)
            {
                Print(ee);
            }

            try
            {
                PositionsAdd();
            } catch (Exception ee)
            {
                Print(ee);
            }


        }

        protected override void OnTick()
        {

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

        //Balance statement
        public void Balance()
        {
            Int32 Stamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string query = "INSERT INTO account (time, accountid, balance, equity, margin, freemargin, currency, leverage) VALUES('" + Stamp + "','" + Account.Number + "', '" + Account.Balance + "', '" + Account.Equity + "', '" + Account.Margin + "', '" + Account.FreeMargin + "', '" + Account.Currency + "', '" + Account.Leverage + "')";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.ExecuteNonQuery();
            Print("Account balance added.");
        }

//====================================================================================================================
//                                                                                          Get Positions
//====================================================================================================================
        public void PositionsAdd()
        {
            foreach (var position in Positions)
            {

                // BUY positions
                if (position.TradeType == TradeType.Buy)
                {
                    Int32 EntryTime = (Int32)(position.EntryTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    string query = "INSERT INTO OpenSignal (id, symbol, volume, type, opent, openp, account) VALUES('" + position.Id + "','" + position.SymbolCode + "', '" + position.Volume + "','BUY','" + EntryTime + "','" + position.EntryPrice + "','" + Account.Number + "')";

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        Print("Add Position " + position.Id);
                    } catch
                    {
                        Print("Duplicate position");
                    }


                }

                // SELL positions
                if (position.TradeType == TradeType.Sell)
                {
                    Int32 EntryTime = (Int32)(position.EntryTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                    string query = "INSERT INTO OpenSignal (id, symbol, volume, type, opent, openp, account) VALUES('" + position.Id + "','" + position.SymbolCode + "', '" + position.Volume + "','SELL','" + EntryTime + "','" + position.EntryPrice + "','" + Account.Number + "')";

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.ExecuteNonQuery();
                        Print("Add Position " + position.Id);
                    } catch
                    {
                        Print("Duplicate position" + position.Id);
                    }
                }

            }


            foreach (HistoricalTrade tr in History)
            {
                // this month closed positions
                if (DateTime.Now.Month == tr.EntryTime.Month)
                {
                    if (tr.TradeType == TradeType.Sell || tr.TradeType == TradeType.Buy)
                    {
                        Int32 EntryTime = (Int32)(tr.EntryTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                        string query = "INSERT INTO CloseSignal (id, closet, closep, profit, pips, account) VALUES('" + tr.PositionId + "','" + EntryTime + "','" + tr.EntryPrice + "','" + tr.NetProfit + "','" + tr.Pips + "','" + Account.Number + "')";

                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            cmd.ExecuteNonQuery();
                            Print("Close Position " + tr.PositionId);
                        } catch
                        {
                            Print("Duplicate close position" + tr.PositionId);
                        }
                    }
                }
            }
        }
    }
}
