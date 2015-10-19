using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false)]
    public class boomood3MA : Indicator
    {
        public int LineBold;
        public Colors LineColor;

        private int Period = 20;
        private MovingAverage expo;
        private MovingAverage expo1;
        private MovingAverage expo2;
        private MovingAverage expo3;

        [Parameter(DefaultValue = 1)]
        public bool MAx3Set { get; set; }

        [Parameter(DefaultValue = 50)]
        public int MA1Period { get; set; }

        [Parameter(DefaultValue = 100)]
        public int MA2Period { get; set; }

        [Parameter(DefaultValue = 200)]
        public int MA3Period { get; set; }


        [Parameter(DefaultValue = 1)]
        public bool RainbowSet { get; set; }

        [Parameter(DefaultValue = 400)]
        public int EnvelopePeriod { get; set; }

        [Parameter(DefaultValue = 0.25)]
        public double BandDistance1 { get; set; }

        [Parameter(DefaultValue = 0.45)]
        public double BandDistance2 { get; set; }

        [Parameter(DefaultValue = 0.6)]
        public double BandDistance3 { get; set; }

        [Parameter(DefaultValue = 0.8)]
        public double BandDistance4 { get; set; }

        [Parameter(DefaultValue = 1.0)]
        public double BandDistance5 { get; set; }

        [Parameter(DefaultValue = 1.7)]
        public double BandDistance6 { get; set; }

        [Output("MA1", Color = Colors.Aqua)]
        public IndicatorDataSeries MA1 { get; set; }

        [Parameter("MA1Type", DefaultValue = 5)]
        public MovingAverageType matype1 { get; set; }


        [Output("MA2", Color = Colors.Red)]
        public IndicatorDataSeries MA2 { get; set; }

        [Parameter("MA2Type", DefaultValue = 5)]
        public MovingAverageType matype2 { get; set; }


        [Output("MA3", Color = Colors.Yellow)]
        public IndicatorDataSeries MA3 { get; set; }

        [Parameter("MA3Type", DefaultValue = 5)]
        public MovingAverageType matype3 { get; set; }


        [Output("MainMA")]
        public IndicatorDataSeries EnvelopeMain { get; set; }

        [Output("ChannelUp1", Color = Colors.Aqua)]
        public IndicatorDataSeries ChannelUp1 { get; set; }

        [Output("ChannelUp2", Color = Colors.Teal)]
        public IndicatorDataSeries ChannelUp2 { get; set; }

        [Output("ChannelUp3", Color = Colors.Yellow)]
        public IndicatorDataSeries ChannelUp3 { get; set; }

        [Output("ChannelUp4", Color = Colors.OrangeRed)]
        public IndicatorDataSeries ChannelUp4 { get; set; }

        [Output("ChannelUp5", Color = Colors.Red)]
        public IndicatorDataSeries ChannelUp5 { get; set; }

        [Output("ChannelUp6", Color = Colors.Red)]
        public IndicatorDataSeries ChannelUp6 { get; set; }



        [Output("ChannelLow1", Color = Colors.Aqua)]
        public IndicatorDataSeries ChannelLow1 { get; set; }

        [Output("ChannelLow2", Color = Colors.Teal)]
        public IndicatorDataSeries ChannelLow2 { get; set; }

        [Output("ChannelLow3", Color = Colors.Yellow)]
        public IndicatorDataSeries ChannelLow3 { get; set; }

        [Output("ChannelLow4", Color = Colors.OrangeRed)]
        public IndicatorDataSeries ChannelLow4 { get; set; }

        [Output("ChannelLow5", Color = Colors.Red)]
        public IndicatorDataSeries ChannelLow5 { get; set; }

        [Output("ChannelLow6", Color = Colors.Red)]
        public IndicatorDataSeries ChannelLow6 { get; set; }


        [Parameter("MainMAType", DefaultValue = 5)]
        public MovingAverageType matype { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool Set100Levels { get; set; }


        // line bold
        [Parameter(DefaultValue = 1)]
        public bool L1 { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool L2 { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool L3 { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool L4 { get; set; }

        // ===== linr color
        [Parameter(DefaultValue = 1)]
        public bool SetRed { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetWhite { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetBlack { get; set; }

        [Parameter(DefaultValue = 0)]
        public bool SetGreen { get; set; }

        protected override void Initialize()
        {

            expo = Indicators.MovingAverage(MarketSeries.Close, EnvelopePeriod, matype);
            expo1 = Indicators.MovingAverage(MarketSeries.Close, MA1Period, matype1);
            expo2 = Indicators.MovingAverage(MarketSeries.Close, MA2Period, matype2);
            expo3 = Indicators.MovingAverage(MarketSeries.Close, MA3Period, matype3);
        }

        public override void Calculate(int index)
        {

            if (RainbowSet)
            {
                EnvelopeMain[index] = expo.Result[index];
                ChannelUp1[index] = expo.Result[index] + (expo.Result[index] * BandDistance1) / 100;
                ChannelUp2[index] = expo.Result[index] + (expo.Result[index] * BandDistance2) / 100;
                ChannelUp3[index] = expo.Result[index] + (expo.Result[index] * BandDistance3) / 100;
                ChannelUp4[index] = expo.Result[index] + (expo.Result[index] * BandDistance4) / 100;
                ChannelUp5[index] = expo.Result[index] + (expo.Result[index] * BandDistance5) / 100;
                ChannelUp6[index] = expo.Result[index] + (expo.Result[index] * BandDistance6) / 100;

                ChannelLow1[index] = expo.Result[index] - (expo.Result[index] * BandDistance1) / 100;
                ChannelLow2[index] = expo.Result[index] - (expo.Result[index] * BandDistance2) / 100;
                ChannelLow3[index] = expo.Result[index] - (expo.Result[index] * BandDistance3) / 100;
                ChannelLow4[index] = expo.Result[index] - (expo.Result[index] * BandDistance4) / 100;
                ChannelLow5[index] = expo.Result[index] - (expo.Result[index] * BandDistance5) / 100;
                ChannelLow6[index] = expo.Result[index] - (expo.Result[index] * BandDistance6) / 100;
            }


            if (MAx3Set)
            {
                MA1[index] = expo1.Result[index];
                MA2[index] = expo2.Result[index];
                MA3[index] = expo3.Result[index];

            }

            if (L1)
            {
                LineBold = 1;
            }
            if (L2)
            {
                LineBold = 2;
            }
            if (L3)
            {
                LineBold = 3;
            }
            if (L4)
            {
                LineBold = 4;
            }

            if (SetRed)
            {
                LineColor = Colors.Red;
            }
            if (SetWhite)
            {
                LineColor = Colors.White;
            }
            if (SetBlack)
            {
                LineColor = Colors.Black;
            }
            if (SetGreen)
            {
                LineColor = Colors.YellowGreen;
            }

            if (Set100Levels)
            {
                for (int i = 1; i < 200; i++)
                {
                    ChartObjects.DrawHorizontalLine("LineLevel" + i, i * 100 * Symbol.PipSize, LineColor, LineBold);
                }
            }


        }
    }
}
