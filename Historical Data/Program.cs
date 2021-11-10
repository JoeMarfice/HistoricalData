using System;
using System.Collections.Generic;   // Add this for TagValue pairs
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

// Use the Interactive Brokers "IBApi" 
// Make sure add the reference to the CSharpAPI.dll
// or TwsLib.dll in the Project.
using IBApi;

namespace IB_Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string strOutput = "";
            string StockSymbol = "SPY";
            // Project properties sets arg[1]="UPRO"
            strOutput = "Number of args =" + args.Count();
            if (args.Count() > 0)
            {
                StockSymbol = args[0];
                strOutput += ", args[0] = " + args[0] + ".";
            }
            System.Console.WriteLine(strOutput);
            StockSymbol = "IBM";

            // Ending date for the time series (change as necessary)
            String strEndDate = "20211008 16:00:00";
            // Amount of time up to the end date
            String strDuration = "1 M";
            // Bar size
            String strBarSize = "1 Day";
            // Data type TRADES= OHLC Trades with volume
            String strWhatToShow = "TRADES";
            // Create the ibClient object to represent the connection
            // If you changed the samples Namespace name, use your new 
            // name here in place of "Samples".
            EWrapperImpl ibClient = new EWrapperImpl(StockSymbol);

            // Connect to the IB Server through TWS. Parameters are:
            // host       - Host name or IP address of the host running TWS
            //              (defaults to IP address 127.0.0.1)
            // port       - The port TWS listens through for connections
            // clientId   - The identifier of the client application
            ibClient.ClientSocket.eConnect("", 7496, 0);

            // Create a new contract to specify the security we are searching for.
            Contract contract = new Contract();
            // Fill in the Contract properties
            contract.Symbol = StockSymbol;
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.Currency = "USD";
            // Create a new TagValue List object (for API version 9.71) 
            List<TagValue> historicalDataOptions = new List<TagValue>();

            // Now call reqHistoricalData with parameters:
            // tickerId    - A unique identifer for the request
            // Contract    - The security being retrieved
            // endDateTime - The ending date and time for the request
            // durationStr - The duration of dates/time for the request
            // barSize     - The size of each data bar
            // WhatToShow  - Te data type such as TRADES
            // useRTH      - 1 = Use Real Time history
            // formatDate  - 3 = Date format YYYYMMDD
            // historicalDataOptions 
            ibClient.ClientSocket.reqHistoricalData(1, contract, strEndDate, strDuration,
                                                    strBarSize, strWhatToShow, 1, 1, true,
                                                    historicalDataOptions);
            // Pause to review data
            Console.ReadKey();
            
            // Disconnect from TWS
            ibClient.ClientSocket.eDisconnect();
        }
    }
}
