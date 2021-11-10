// SANDBOX
namespace MyCode
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class AllPeriodsClass
    {
        const string OUTPUT_FILE_PATH = "D:\\TWS API\\Data\\";
        bool FirstTime = true;

        const string TICK_FILE_SUFFIX = " Tick Data.csv";
        const string SECOND_FILE_SUFFIX = " Secondwise Data.csv";
        const string MINUTE_FILE_SUFFIX = " Minutewise Data.csv";
        const string HOUR_FILE_SUFFIX = " Hourly Data.csv";
        const string DAY_FILE_SUFFIX = " Daily Data.csv";

        PeriodClass TickPeriod, SecondPeriod, MinutePeriod, HourPeriod, DayPeriod;
        DateTime PrevTime;

        public AllPeriodsClass(string StockSymbol)
        {
            PrevTime = DateTime.Now;
            TickPeriod = new TickPeriodClass(OUTPUT_FILE_PATH + StockSymbol + TICK_FILE_SUFFIX, PrevTime);
            SecondPeriod = new SecondPeriodClass(OUTPUT_FILE_PATH + StockSymbol + SECOND_FILE_SUFFIX, PrevTime);
            MinutePeriod = new MinutePeriodClass(OUTPUT_FILE_PATH + StockSymbol + MINUTE_FILE_SUFFIX, PrevTime);
            HourPeriod = new HourPeriodClass(OUTPUT_FILE_PATH + StockSymbol + HOUR_FILE_SUFFIX, PrevTime);
            DayPeriod = new DayPeriodClass(OUTPUT_FILE_PATH + StockSymbol + DAY_FILE_SUFFIX, PrevTime);
        }
        public void HandleTickOrder(DateTime NowTime, double Price, string StockSymbol)
        {
            //System.Console.WriteLine("HandleTickOrder for " + StockSymbol);
            if (FirstTime)
            {
                TickPeriod.ProcessFirstTick(NowTime, Price);
                SecondPeriod.ProcessFirstTick(NowTime, Price);
                MinutePeriod.ProcessFirstTick(NowTime, Price);
                HourPeriod.ProcessFirstTick(NowTime, Price);
                DayPeriod.ProcessFirstTick(NowTime, Price);
            }
            else
            {
                TickPeriod.ProcessTick(NowTime, Price);
                SecondPeriod.ProcessTick(NowTime, Price);
                MinutePeriod.ProcessTick(NowTime, Price);
                HourPeriod.ProcessTick(NowTime, Price);
                DayPeriod.ProcessTick(NowTime, Price);
            }

            PrevTime = NowTime;
            FirstTime = false;

        } // HandleTickOrder
    } // AllPeriodsClass

    public abstract class PeriodClass
    {
        string FileName;
        double High;
        double Low;
        double Close;
        double Open;
        public double PrevPrice;
        public string strPrevDate;

        public PeriodClass(string fileName)
        {
            FileName = fileName;
        }

        public abstract bool ShouldWrite(DateTime TimeNow);

        public virtual void ProcessFirstTick(DateTime TimeNow, double StockPrice)
        {
            // This is for all periods except tick periods.
            //System.Console.WriteLine("Handle first non-tick periods.");
            string strDateNow = TimeNow.ToString("yyyy-MM-dd");
            WriteData(",,,," + strDateNow);

            strPrevDate = strDateNow;
            High = StockPrice;
            Low = StockPrice;
            Open = StockPrice;
            Close = StockPrice;
        }

        public virtual void ProcessTick(
            DateTime TimeNow,
            double StockPrice
        )
        {
            // This is only for periods that are not tick periods.
            string strOutput = null;
            string strDateNow = TimeNow.ToString("yyyy-MM-dd");
            //System.Console.WriteLine("Handle non-tick periods.");

            High = Math.Max(StockPrice, High);
            Low = Math.Min(StockPrice, Low);

            if (ShouldWrite(TimeNow))
            {
                Close = PrevPrice;

                if (strDateNow != strPrevDate)
                {
                    strOutput += ",,,," + strDateNow;
                    WriteData(strOutput);
                }

                strOutput = TimeNow.ToString("HH:mm:ss.fff") + ",";

                strOutput += Open.ToString( "0.00") + ","
                           + High.ToString( "0.00") + ","
                           + Low.ToString(  "0.00") + ","
                           + Close.ToString("0.00");
                WriteData(strOutput);

                Open = StockPrice;
                PrevPrice = StockPrice;
                strPrevDate = strDateNow;
            }
        }


        public void WriteData(string OutputText)
        {
            //System.Console.WriteLine("C");
            using (StreamWriter FileOut = new StreamWriter(FileName, true))
            {
                System.Console.WriteLine(FileName + ": " + OutputText);
                FileOut.WriteLine(OutputText);
            }
        } // WriteData

    } // PeriodClass

    public class TickPeriodClass : PeriodClass
    {
        DateTime PrevTime;
        public TickPeriodClass(string filename, DateTime TimeNow)
            : base(filename)
        {
        }

        public override void ProcessFirstTick(DateTime TimeNow, double StockPrice)
        {
            // This is for tick periods only.
            //System.Console.WriteLine("Handle tick periods first tick.");
            string strDateNow = TimeNow.ToString("yyyy-MM-dd");
            WriteData(",," + strDateNow);

            string strOutput = TimeNow.ToString("HH:mm:ss.fff") + "," 
                + StockPrice.ToString("0.00");
            WriteData(strOutput);

            strPrevDate = strDateNow;
        }

        public override void ProcessTick(
            DateTime TimeNow,
            double StockPrice
        )
        {
            // This is only for periods that are tick periods.
            //System.Console.WriteLine("Handle tick periods.");

            string strOutput = null;
            string strDateNow = TimeNow.ToString("yyyy-MM-dd");

            if (strDateNow != strPrevDate)
            {
                strOutput += ",," + strDateNow;
                WriteData(strOutput);
            }

            strOutput = TimeNow.ToString("HH:mm:ss.fff") + "," 
                + StockPrice.ToString("0.00");
            WriteData(strOutput);

            PrevPrice = StockPrice;
            strPrevDate = strDateNow;
        }

        public override bool ShouldWrite(DateTime TimeNow)
        {
            return true;
        }
    } // TickPeriodClass

    class SecondPeriodClass : PeriodClass
    {
        string strPrevWriteTime;
        public SecondPeriodClass(string filename, DateTime TimeNow)
            : base(filename)
        {
        }

        public override bool ShouldWrite(DateTime TimeNow)
        {
            string strWriteTime = TimeNow.ToString("YYYY-MM-DD HH:mm:ss");

            bool Result = (strWriteTime != strPrevWriteTime);
            if (Result)
                strPrevWriteTime = strWriteTime;
            return Result;
        }
    } // SecondPeriodClass

    class MinutePeriodClass : PeriodClass
    {
        string strPrevWriteTime;
        public MinutePeriodClass(string filename, DateTime TimeNow)
            : base(filename)
        {
        }

        public override bool ShouldWrite(DateTime TimeNow)
        {
            string strWriteTime = TimeNow.ToString("YYYY-MM-DD HH:mm");

            bool Result = (strWriteTime != strPrevWriteTime);
            if (Result)
                strPrevWriteTime = strWriteTime;
            return Result;
        }
    } // MinutePeriodClass

    class HourPeriodClass : PeriodClass
    {
        string strPrevWriteTime;
        public HourPeriodClass(string filename, DateTime TimeNow)
            : base(filename)
        {
        }

        public override bool ShouldWrite(DateTime TimeNow)
        {
            string strWriteTime = TimeNow.ToString("YYYY-MM-DD HH");

            bool Result = (strWriteTime != strPrevWriteTime);
            if (Result)
                strPrevWriteTime = strWriteTime;
            return Result;
        }
    } // HourPeriodClass

    class DayPeriodClass : PeriodClass
    {
        string strPrevWriteTime;
        public DayPeriodClass(string filename, DateTime TimeNow)
            : base(filename)
        {
        }

        public override bool ShouldWrite(DateTime TimeNow)
        {
            string strWriteTime = TimeNow.ToString("YYYY-MM-DD HH");

            bool Result = (strWriteTime != strPrevWriteTime);
            if (Result)
                strPrevWriteTime = strWriteTime;
            return Result;
        }
    } // DayPeriodClass
} // MyCode
