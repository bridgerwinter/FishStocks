// See https://aka.ms/new-console-template for more information
using FishStocks.Importer.FishStocksDatasetTableAdapters;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using Tesseract;

namespace Fishstocks.Importer
{
    class Program
    {
        public static List<string> ImportedText = new List<string>();
        private static readonly string ConnectionString = @"Data Source=SUNBEAR\SQLEXPRESS;Initial Catalog = FishStocks; Integrated Security = True";

        public static void Main(string[] args)
        {
            var testImagePath = "C:\\Users\\bridg\\source\\repos\\FishStocks\\FishStocks.Importer\\unnamed.png";
            if (args.Length > 0)
            {
                testImagePath = args[0];
            }

            try
            {
                var logger = new FormattedConsoleLogger();
                var resultPrinter = new ResultPrinter(logger);
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (logger.Begin("Process image"))
                        {
                            var i = 1;
                            using (var page = engine.Process(img))
                            {
                                var text = page.GetText();
                               // logger.Log("Text: {0}", text);
                                logger.Log("Mean confidence: {0}", page.GetMeanConfidence());
                                if (text != string.Empty)
                                {
                                    ImportedText.Add(text);
                                }
                                using (var iter = page.GetIterator())
                                {
                                    iter.Begin();
                                    do
                                    {
                                        if (i % 2 == 0)
                                        {
                                            using (logger.Begin("Line {0}", i))
                                            {
                                                do
                                                {
                                                    using (logger.Begin("Word Iteration"))
                                                    {
                                                        if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                                        {
                                                            logger.Log("New block");
                                                        }
                                                        if (iter.IsAtBeginningOf(PageIteratorLevel.Para))
                                                        {
                                                            logger.Log("New paragraph");
                                                        }
                                                        if (iter.IsAtBeginningOf(PageIteratorLevel.TextLine))
                                                        {
                                                            logger.Log("New line");
                                                        }
                                                        logger.Log("word: " + iter.GetText(PageIteratorLevel.Word));
                                                        //ImportedText.Add(iter.GetText(PageIteratorLevel.Word));
                                                    }
                                                } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));
                                            }
                                        }
                                        i++;
                                    } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
            }
            CleanList();
            Console.Write("Press any key to continue . . . ");

            Console.ReadKey(true);
        }

        public static void CleanList()
        {
            var CleanList = new List<string>();
            string str = string.Empty;
            foreach (var item in ImportedText)
            {
                var splitList = item.Split('\n');
                foreach (var split in splitList)
                {
                    if (!String.IsNullOrWhiteSpace(split))
                    {
                        CleanList.Add(split);
                    }
                }
            }
            var splitIndex = CleanList.Count() / 2;
            var fishList = new List<string>();
            var priceList = new List<decimal>();
            var fishPriceIndex = new Dictionary<string, decimal>();
            for (int i = 0; i < splitIndex; i++)
            {
                if (CleanList[i] != " ")
                {
                    fishList.Add(CleanList[i]);
                }
            }
            for (int i = splitIndex; i < splitIndex * 2; i++)
            {
                var val = decimal.Parse(CleanList[i], NumberStyles.Currency);
                if (CleanList[i] != " ")
                {
                    priceList.Add(val);
                }
            }
            for (int i = 0; i < fishList.Count(); i++)
            {
                fishPriceIndex.Add(fishList[i], priceList[i]);
            }
            foreach (var item in fishPriceIndex)
            {
                Console.WriteLine("Key: {0} Value: {1}",item.Key, item.Value);
            }
            //AddFishToDatabase(fishList);
            //AddToDatabase(fishPriceIndex);
        }

        private class ResultPrinter
        {
            readonly FormattedConsoleLogger logger;

            public ResultPrinter(FormattedConsoleLogger logger)
            {
                this.logger = logger;
            }

            public void Print(ResultIterator iter)
            {
                logger.Log("Is beginning of block: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Block));
                logger.Log("Is beginning of para: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Para));
                logger.Log("Is beginning of text line: {0}", iter.IsAtBeginningOf(PageIteratorLevel.TextLine));
                logger.Log("Is beginning of word: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Word));
                logger.Log("Is beginning of symbol: {0}", iter.IsAtBeginningOf(PageIteratorLevel.Symbol));

                logger.Log("Block text: \"{0}\"", iter.GetText(PageIteratorLevel.Block));
                logger.Log("Para text: \"{0}\"", iter.GetText(PageIteratorLevel.Para));
                logger.Log("TextLine text: \"{0}\"", iter.GetText(PageIteratorLevel.TextLine));
                logger.Log("Word text: \"{0}\"", iter.GetText(PageIteratorLevel.Word));
                logger.Log("Symbol text: \"{0}\"", iter.GetText(PageIteratorLevel.Symbol));
            }
        }

        public static void AddToDatabase(Dictionary<string, decimal> pairs)
        {
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(ConnectionString);
            using (sqlConnection)
            {
                FishTransactionTableAdapter fishTransactionTableAdapter = new FishTransactionTableAdapter();
                foreach (var item in pairs)
                {
                    fishTransactionTableAdapter.Insert(item.Value, DateTime.Now, item.Key);
                }
            }
        }
        public static void AddFishToDatabase(List<string> fish)
        {
            SqlConnection sqlConnection;
            sqlConnection = new SqlConnection(ConnectionString);
            using (sqlConnection)
            {
                FishTableAdapter fishTableAdapter = new FishTableAdapter();
                foreach (var item in fish)
                {
                    fishTableAdapter.Insert(item);
                }
            }

        }

    }
}

