// System
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
// cAlgo
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.API.Requests;
using cAlgo.Indicators;


namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class NewcBot : Robot
    {

        private string responseFromServer = "";
        //private string openPositionsString = "";

        [Parameter("Username", DefaultValue = "")]
        public string Username { get; set; }

        [Parameter("Password", DefaultValue = "")]
        public string Password { get; set; }

        [Parameter("MoneyForSignalUSD", DefaultValue = 100, MinValue = 100)]
        public int MoneyForSignalUSD { get; set; }

        List<string> PosOpenID = new List<string>();
        List<string> PosCloseID = new List<string>();
        List<string> PosServerID = new List<string>();
        List<string> PosServerAll = new List<string>();


        int multiply = 0;

        protected override void OnStart()
        {
            // Put your initialization logic here
        }

        protected override void OnBar()
        {
            // Put your initialization logic here

        }

        protected override void OnTick()
        {



            if (MoneyForSignalUSD >= 100)
            {
                multiply = (int)MoneyForSignalUSD / 100;
                multiply = multiply * 1000;

                Print("Multiply " + multiply);
            }

            // Put your core logic here
            //sendPositions(Username, Password);

            // Initialize
            initializePositions();
            //getPositions(Username, Password);

            closeAll();
            //comparePositions();

        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }



        protected void closeAll()
        {
            string[] posres = responseFromServer.Split('#');
            string inp = Convert.ToString(posres[1]);
            Print("String " + inp);
            char znak = Convert.ToChar(inp[0]);
            char znak1 = Convert.ToChar("[");

            if (znak == znak1)
            {
                Print(inp);
                foreach (var position in Positions)
                {

                    ClosePosition(position);

                }

            }
        }
//====================================================================================================================
//                                                                                                Compare    Positions
//====================================================================================================================
        protected void comparePositions()
        {

            try
            {
                string[] openclose = responseFromServer.Split('#');
                string[] posres = openclose[0].Split('|');

                string inp = "" + posres[1];
                // cut [GO] and [OG]
                //inp = inp.Substring(4);
                //inp = inp.Substring(0, inp.Length - 6);

                // pociapać na pozycję lista
                string[] posin = inp.Split(';');
                PosServerAll = new List<string>(posin);

                initializePositions();


                foreach (string pos in posin)
                {

                    string[] p = pos.Split(';');

                    if (!PosOpenID.Contains(p[1]) && p[1] != "" && !PosCloseID.Contains(p[1]))
                    {

                        Symbol symbol = MarketData.GetSymbol(p[3]);
                        // Print(p[3]);
                        if (p[5] == "BUY")
                        {
                            Print("BUY " + p[5]);
                            //p[5].Replace(",", p[5]);



                            double pips1 = double.Parse(p[2], System.Globalization.CultureInfo.InvariantCulture) - double.Parse(p[6], System.Globalization.CultureInfo.InvariantCulture);
                            double sl1 = pips1 / symbol.PipSize;
                            double pips2 = double.Parse(p[7], System.Globalization.CultureInfo.InvariantCulture) - double.Parse(p[2], System.Globalization.CultureInfo.InvariantCulture);
                            double tp1 = pips2 / symbol.PipSize;

                            Print("Volume: " + p[4]);
                            double volume = double.Parse(p[4], System.Globalization.CultureInfo.InvariantCulture) * multiply;

                            Print(volume);
                            ExecuteMarketOrder(TradeType.Buy, symbol, Convert.ToInt64(volume), "", sl1, tp1, 1, p[1]);

                            //ExecuteMarketOrder(TradeType.Buy, Symbol, Convert.ToInt64(Convert.ToDecimal(p[4])), "Slave", Convert.ToDouble(p[6]), Convert.ToDouble(p[7]));
                        }

                        if (p[5] == "SELL")
                        {
                            Print("SELL " + p[5]);
                            double pips1 = double.Parse(p[6], System.Globalization.CultureInfo.InvariantCulture) - double.Parse(p[2], System.Globalization.CultureInfo.InvariantCulture);
                            double sl = pips1 / symbol.PipSize;
                            double pips2 = double.Parse(p[2], System.Globalization.CultureInfo.InvariantCulture) - double.Parse(p[7], System.Globalization.CultureInfo.InvariantCulture);
                            double tp = pips2 / symbol.PipSize;
                            double volume = double.Parse(p[4], System.Globalization.CultureInfo.InvariantCulture) * multiply;

                            ExecuteMarketOrder(TradeType.Sell, symbol, Convert.ToInt64(volume), "", sl, tp, 1, p[1]);
                            //ExecuteMarketOrder(TradeType.Sell, Symbol, Volume, MyLabel, StopLoss, TakeProfit);
                        }
                    }
                    initializePositions();
                }

            } catch (Exception rrr)
            {
                Print(rrr);
            }
        }
//====================================================================================================================
//                                                                                                Initialize Positions
//====================================================================================================================
        protected void initializePositions()
        {
            // open id
            PosOpenID.Clear();
            foreach (var position in Positions)
            {
                if (position.Comment != "")
                {
                    PosOpenID.Add(position.Comment);
                    Print(position.Comment);
                }
            }

            // colose id
            PosCloseID.Clear();
            foreach (var trade in History)
            {
                if (trade.Comment != "")
                {
                    PosCloseID.Add(trade.Comment);
                    Print(trade.Comment);
                }

            }

            // Loop through List with foreach
            //Print("OPEN POS ID COMMENT ");
            string o = "";
            foreach (string prime in PosOpenID)
            {
                o = prime + "| ";
            }
            Print(o);

            // Print("CLOSED POS ID COMMENT ");
            // Loop through List with foreach
            string c = "";
            foreach (string prime in PosCloseID)
            {
                c = prime + "| ";

            }
            Print(c);
        }

//====================================================================================================================
//                                                                                          Send POST to HTTPS Server
//====================================================================================================================
        protected void getPositions(string Username, string Password)
        {
            // log file path
            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //string logPath = Path.Combine(desktopFolder, "MasterLog.db");

            //================================================================================
            //                                                                    Send request
            //================================================================================
            try
            {
                /*
                using (StreamWriter w = File.AppendText(logPath))
                {
                    // log request
                    w.WriteLine("REQUEST: " + DateTimeToUnixTimestamp(DateTime.UtcNow) + " : " + openPositionsString);
                    w.Flush();
                    w.Close();
                }
                */


                responseFromServer = Get(1);

            } catch (Exception e)
            {

                Print("====================================================================================");
                Print("Post Error: " + e);
                Print("====================================================================================");
                /*
                using (StreamWriter w = File.AppendText(logPath))
                {
                    // log response
                    w.WriteLine("ERROR: " + DateTimeToUnixTimestamp(DateTime.UtcNow) + " : " + e);
                    w.Flush();
                    w.Close();
                }
                */
            }            
            Print("====================================================================================");
            Print("<<== " + responseFromServer);
            Print("====================================================================================");
            return;
        }
        /*
            using (StreamWriter w = File.AppendText(logPath))
            {
                // log response
                w.WriteLine("RESPONSE: " + DateTimeToUnixTimestamp(DateTime.UtcNow) + " : " + responseFromServer);
                w.Flush();
                w.Close();
            }
            */


        /// datatime to timestamp
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }
        // end


        public static string Get(int id = 1)
        {
            // Create a request for the URL. 
            WebRequest request = WebRequest.Create("https://breakermind.com/api/trader.php?id=" + id);
            // If required by the server, set the credentials.
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string FromServer = reader.ReadToEnd();
            // Clean up the streams and the response.
            reader.Close();
            response.Close();
            return FromServer;
        }


        public static string Post(int id = 1)
        {
            WebRequest request = WebRequest.Create("https://breakermind.com/api/traderpost.php");
            byte[] postBytes = Encoding.ASCII.GetBytes("id=" + id);
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string FromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return FromServer;
        }
//================================================================================================================
//                                                                                   End Send POST to HTTPS Server
//================================================================================================================


    }
}


//====================================================================================================================
//                                                                                                Initialize Positions
//====================================================================================================================
