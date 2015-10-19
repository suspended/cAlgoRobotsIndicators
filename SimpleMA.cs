// -------------------------------------------------------------------------------------------------
//    This robot is intended to be used as a sample and does not guarantee any particular outcome or
//    profit of any kind. Use it at your own risk.
//   
//    Start Monday 00:00:00 Stop Friday or when earn
//    Monday to Friday robot tested M5
// -------------------------------------------------------------------------------------------------
 
using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Requests;
using cAlgo.Indicators;
 
namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class SampleTrendRobot : Robot
    {
 
//=====================================================================================================
// Parametrs
//=====================================================================================================
        [Parameter("MA Type")]
        public MovingAverageType MAType { get; set; }
 
        [Parameter()]
        public DataSeries SourceSeries { get; set; }
 
        [Parameter("Slow Periods", DefaultValue = 10)]
        public int SlowPeriods { get; set; }
 
        [Parameter(DefaultValue = 10000, MinValue = 0)]
        public int Volume { get; set; }
 
        private MovingAverage slowMa;
        private Position position;
        private double daystart;
 
//=====================================================================================================
// Trailing Parametrs
//=====================================================================================================
        [Parameter("Stop Loss (pips)", DefaultValue = 25)]
        public int StopLoss { get; set; }
 
        [Parameter("Take Profit (pips)", DefaultValue = 25)]
        public int TakeProfit { get; set; }
 
        [Parameter("Trigger (pips)", DefaultValue = 5)]
        public int Trigger { get; set; }
 
        [Parameter("Trailing Stop (pips)", DefaultValue = 5)]
        public int TrailingStop { get; set; }
 
 
        private Position _position;
 
 
//=====================================================================================================
// OnStart
//=====================================================================================================
        protected override void OnStart()
        {
            slowMa = Indicators.MovingAverage(SourceSeries, SlowPeriods, MAType);
            daystart = Symbol.Ask;
        }
 
//=====================================================================================================
// OnTick
//=====================================================================================================
        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;
 
            int lastIndex = slowMa.Result.Count - 1;
            int prevIndex = slowMa.Result.Count - 3;
 
            double currentSlowMa = slowMa.Result[lastIndex];
            double previousSlowMa = slowMa.Result[prevIndex];
 
            bool isLongPositionOpen = position != null && position.TradeType == TradeType.Buy;
            bool isShortPositionOpen = position != null && position.TradeType == TradeType.Sell;
 
            if (previousSlowMa < daystart && currentSlowMa > daystart && !isLongPositionOpen)
            {
                ClosePosition();
                Buy();
            }
 
            if (previousSlowMa > daystart && currentSlowMa < daystart && !isShortPositionOpen)
            {
                ClosePosition();
                Sell();
            }
 
 
//=====================================================================================================
// Trailing
//=====================================================================================================
            if (_position == null)
                return;
 
            double distance = _position.EntryPrice - Symbol.Ask;
 
            if (distance >= Trigger * Symbol.PipSize)
            {
                double newStopLossPrice = Symbol.Ask + TrailingStop * Symbol.PipSize;
                if (_position.StopLoss == null || newStopLossPrice < _position.StopLoss)
                {
                    Trade.ModifyPosition(_position, newStopLossPrice, _position.TakeProfit);
                }
            }
 
        }
 
//=====================================================================================================
// onPositionsOpened
//=====================================================================================================
        protected override void OnPositionOpened(Position openedPosition)
        {
            _position = openedPosition;
 
            double? stopLossPrice = null;
            double? takeProfitSize = null;
 
            if (StopLoss != 0)
                stopLossPrice = _position.EntryPrice + StopLoss * Symbol.PipSize;
 
            if (TakeProfit != 0)
                takeProfitSize = _position.EntryPrice - TakeProfit * Symbol.PipSize;
 
            Trade.ModifyPosition(openedPosition, stopLossPrice, takeProfitSize);
        }
 
//=====================================================================================================
// onPositionsClosed
//=====================================================================================================
        protected override void OnPositionClosed(Position closedPosition)
        {
            // Stop();
        }
 
//=====================================================================================================
// functions
//=====================================================================================================
        private void ClosePosition()
        {
            if (position != null)
            {
                Trade.Close(position);
                position = null;
            }
        }
 
        private void Buy()
        {
            Trade.CreateBuyMarketOrder(Symbol, Volume);
        }
 
        private void Sell()
        {
            Trade.CreateSellMarketOrder(Symbol, Volume);
        }
 
 
    }
}
