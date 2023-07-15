using DevourDev.SheetCodeUtility;

namespace YandexContest
{
    internal static class TaskC2
    {
        private struct Trade
        {
            public int BuyDay;
            public int SellDay;


            public Trade(int buyDay, int sellDay)
            {
                BuyDay = buyDay;
                SellDay = sellDay;
            }
        }


        public static void Solve()
        {
            var prices = ReadInput();
            var output = FindOptimalTrades(prices.Length, prices);

            Console.WriteLine(output.Count);

            foreach (var trade in output)
            {
                Console.WriteLine($"{trade.BuyDay + 1} {trade.SellDay + 1}");
            }
        }

        private static int[] ReadInput()
        {
            int n = InOutUtils.Read.Int();
            int[] prices = InOutUtils.Read.ArrayOf.Ints();
            return prices;
        }

        private static List<Trade> FindOptimalTrades(int n, int[] pricesArray)
        {
            int buyDay = -1;
            int sellDay = -1;
            List<Trade> trades = new List<Trade>();

            for (int i = 0; i < n - 1; i++)
            {
                if (pricesArray[i] < pricesArray[i + 1])
                {
                    if (buyDay == -1)
                    {
                        buyDay = i;
                    }
                }
                else if (pricesArray[i] > pricesArray[i + 1])
                {
                    if (buyDay != -1)
                    {
                        sellDay = i;
                        trades.Add(new Trade(buyDay, sellDay));
                        buyDay = -1;
                        sellDay = -1;
                    }
                }
            }

            // Check if there is an open trade
            if (buyDay != -1 && sellDay == -1)
            {
                sellDay = n - 1;
                trades.Add(new Trade(buyDay, sellDay));
            }

            return trades;
        }
    }
}
