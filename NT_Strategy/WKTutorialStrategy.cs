#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
using System.Net.Http;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class WKTutorialStrategy : Strategy
	{
		private SMA sma1;
		private SMA sma2;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description = @"Strategy from https://www.youtube.com/watch?v=Tpvs70YktgM and https://www.youtube.com/watch?v=BA0W4ECyVdc";
				Name = "WKTutorialStrategy";
				Calculate = Calculate.OnBarClose;
				EntriesPerDirection = 1;
				EntryHandling = EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy = true;
				ExitOnSessionCloseSeconds = 30;
				IsFillLimitOnTouch = false;
				MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution = OrderFillResolution.Standard;
				Slippage = 0;
				StartBehavior = StartBehavior.WaitUntilFlat;
				TimeInForce = TimeInForce.Gtc;
				TraceOrders = false;
				RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling = StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade = 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration = true;
				FastSMA = 4;
				SlowSMA = 14;
				QuantitySize = DefaultQuantity;
				AIEnabled = true;
			}
			else if (State == State.Configure)
			{
				SetStopLoss(CalculationMode.Percent, 0.08);
				SetProfitTarget(CalculationMode.Percent, 0.12);
			}
			else if (State == State.DataLoaded)
			{
				sma1 = SMA(FastSMA);
				sma2 = SMA(SlowSMA);

				// Add RSI and ADX indicators to the chart for display
				// This only displays the indicators for the primary Bars object (main instrument) on the chart
				AddChartIndicator(sma1);
				AddChartIndicator(sma2);
			}
		}

		protected override void OnBarUpdate()
		{
			if (AIEnabled)
			{
				// In AI mode you can call external REST service to suggest action (go long/short)

				HttpClient client = new HttpClient();
				string currentPrice = Bars[CurrentBar].ToString(System.Globalization.CultureInfo.InvariantCulture);
				string requestUrl = string.Format("https://localhost:44320/api/AiAdvice?price={0}", currentPrice);
				//https://localhost:44320/api/AiAdvice?price=0.87500

				var getResponse = client.GetAsync(requestUrl).Result;

				if (getResponse.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var aiAdvice = getResponse.Content.ReadAsStringAsync().Result;
					if (aiAdvice == "\"enter long\"")
					{
						EnterLong(QuantitySize);
					}
					if (aiAdvice == "\"enter short\"")
					{
						EnterShort(QuantitySize);
					}
				}
			}
			else
			{
				// In classic approach you would do something like that

				if (SMA(FastSMA)[0] > SMA(SlowSMA)[0])
				{
					EnterLong(QuantitySize);
				}

				if (SMA(FastSMA)[0] < SMA(SlowSMA)[0])
				{
					EnterShort(QuantitySize);
				}
			}
		}

		#region Properties
		[NinjaScriptProperty]
		[Display(Name = "AI_Enabled", Description = "Use AI to suggest transactions", Order = 1, GroupName = "Parameters")]
		public bool AIEnabled
		{ get; set; }

		[NinjaScriptProperty]
		[Range(3, int.MaxValue)]
		[Display(Name = "FastSMA", Description = "Window size for FastSMA", Order = 2, GroupName = "Parameters")]
		public int FastSMA
		{ get; set; }

		[NinjaScriptProperty]
		[Range(5, int.MaxValue)]
		[Display(Name = "SlowSMA", Description = "Window size for SlowSMA", Order = 3, GroupName = "Parameters")]
		public int SlowSMA
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name = "QuantitySize", Description = "QuantitySize", Order = 4, GroupName = "Parameters")]
		public int QuantitySize
		{ get; set; }


		#endregion
	}
}
