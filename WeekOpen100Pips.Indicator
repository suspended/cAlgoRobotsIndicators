using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class WeekOpen : Indicator
    {
        [Output("Open", Color = Colors.YellowGreen, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Open { get; set; }

        [Parameter("Show 100PipsLevels", DefaultValue = 1)]
        public bool Set100Levels { get; set; }

        [Parameter("MinLevel", DefaultValue = 0, MinValue = 0)]
        public int MinLevel { get; set; }

        [Parameter("MaxLevel", DefaultValue = 200, MinValue = 2)]
        public int MaxLevel { get; set; }


        public double openprice = 0;

        public override void Calculate(int index)
        {

            if (index < 1)
            {
                // If first bar is first bar of the day set open
                if (MarketSeries.OpenTime[index].TimeOfDay == TimeSpan.Zero)
                    Open[index] = MarketSeries.Open[index];
                return;
            }

            DateTime openTime = MarketSeries.OpenTime[index];
            DateTime lastOpenTime = MarketSeries.OpenTime[index - 1];
            const string objectName = "messageNA";

            if (!ApplicableTimeFrame(openTime, lastOpenTime))
            {
                // Display message that timeframe is N/A
                const string text = "TimeFrame Not Applicable. Choose a lower Timeframe";
                ChartObjects.DrawText(objectName, text, StaticPosition.TopLeft, Colors.Red);
                return;
            }

            // If TimeFrame chosen is applicable remove N/A message
            ChartObjects.RemoveObject(objectName);

            // Plot Daily Open and Close
            PlotDailyOpenClose(openTime, lastOpenTime, index);

            double Pips = 0;
            if (Symbol.Ask > openprice)
                Pips = (Symbol.Ask - openprice) / Symbol.PipSize;

            if (Symbol.Ask < openprice)
                Pips = (openprice - Symbol.Ask) / Symbol.PipSize;

            double Profit = (Pips / 100) * 1000;


            var name1 = "Week";
            var text1 = "Week Open : " + openprice.ToString() + "\n Pips from week open :  " + (int)Pips;
            var staticPos = StaticPosition.TopRight;
            var color = Colors.Yellow;
            ChartObjects.DrawText(name1, text1, staticPos, color);

            var name11 = "Open";
            var text11 = "Open: " + openprice.ToString() + " Ask: " + Symbol.Ask + "\n Pips: " + (int)Pips + "\n Month Open Profit (1 Lot): " + (int)Profit + " USD";
            var staticPos1 = StaticPosition.TopRight;
            var color1 = Colors.Yellow;
            ChartObjects.DrawText(name11, text1, staticPos1, color1);

            var name12 = "Pips";
            var text12 = "Pips from week open : " + (int)Pips + "\n Week Open Profits from: \n 100.0 Lot => " + (int)Profit * 100 + " USD (Initial Deposit 1 000 000$)" + "\n 10.0 Lot => " + (int)Profit * 10 + " USD (Initial Deposit 100 000$)" + "\n 1.0 Lot => " + (int)Profit + " USD (Initial Deposit 10 000$)" + "\n 0.10 Lot => " + (int)Profit / 10 + " USD (Initial Deposit 1 000$)" + "\n 0.01 Lot => " + (int)Profit / 100 + " USD (Initial Deposit 100$)  \n Profit = (Pips / 100) * 1000USD for 1Lot(100000) \n You can add a few pending orders on 100 pips levels to increase profits :) \n Have a nice day";
            var staticPos12 = StaticPosition.BottomLeft;
            var color12 = Colors.YellowGreen;
            ChartObjects.DrawText(name12, text12, staticPos12, color12);


            if (Set100Levels && MinLevel < MaxLevel)
            {
                for (int i = MinLevel; i < MaxLevel; i++)
                {
                    ChartObjects.DrawHorizontalLine("Level" + i, i * 100 * Symbol.PipSize, Colors.DodgerBlue, 2, LineStyle.Solid);
                }
            }

        }

        private bool ApplicableTimeFrame(DateTime openTime, DateTime lastOpenTime)
        {
            // minutes difference between bars
            var timeFrameMinutes = (int)(openTime - lastOpenTime).TotalMinutes;

            bool daily = timeFrameMinutes == 1440;
            bool weeklyOrGreater = timeFrameMinutes >= 7200;

            bool timeFrameNotApplicable = daily || weeklyOrGreater;

            if (timeFrameNotApplicable)
                return false;

            return true;
        }

        private void PlotDailyOpenClose(DateTime openTime, DateTime lastOpenTime, int index)
        {

            DateTime currentTime = MarketSeries.OpenTime[MarketSeries.OpenTime.Count - 1];
            DateTime previousTime = MarketSeries.OpenTime[MarketSeries.OpenTime.Count - 2];

            int index1 = MarketSeries.OpenTime.Count - 1;

            // Day change
            //if (openTime.Day != lastOpenTime.Day)
            if (currentTime.DayOfWeek == DayOfWeek.Monday && previousTime.DayOfWeek != DayOfWeek.Monday)
            {
                // Plot Open
                Open[index] = MarketSeries.Open[index];
                openprice = MarketSeries.Open[index];
            }
            // Same Day
            else
            {
                // Plot Open
                Open[index] = Open[index - 1];
                //openprice = MarketSeries.Open[index];
            }

            // Plot todays close
            DateTime today = DateTime.Now.Date;
            if (openTime.Date != today)
                return;
        }
    }
}
