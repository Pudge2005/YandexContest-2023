using DevourDev.SheetCodeUtility;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace YandexContest
{
    internal static class TaskC
    {
        private readonly struct DayPrice
        {
            private static readonly DayPrice _notInited = new(-1, -1);
            private static readonly DayPrice _min = new(-1, int.MinValue);
            private static readonly DayPrice _max = new(-1, int.MaxValue);

            public readonly int DayId;
            public readonly int Price;


            public DayPrice(int dayId, int price)
            {
                DayId = dayId;
                Price = price;
            }


            public static DayPrice NotInited => _notInited;
            public static DayPrice MinValue => _min;
            public static DayPrice MaxValue => _max;


            public bool IsInited => DayId >= 0;


            public bool Equals(DayPrice other)
            {
                return this == other;
            }

            public override int GetHashCode()
            {
                return DayId;
            }

            public static bool operator ==(DayPrice left, DayPrice right)
            {
                return left.DayId == right.DayId;
            }

            public static bool operator !=(DayPrice left, DayPrice right)
            {
                return left.DayId != right.DayId;
            }

            public override bool Equals(object? obj)
            {
                return obj is BuySellTrade && Equals((BuySellTrade)obj);
            }
        }


        private readonly struct BuySellTrade : IEquatable<BuySellTrade>, IComparable<BuySellTrade>
        {
            private static readonly BuySellTrade _notInited = new(DayPrice.NotInited, DayPrice.NotInited);
            private static readonly BuySellTrade _min = new(DayPrice.MaxValue, DayPrice.MinValue);
            private static readonly BuySellTrade _max = new(DayPrice.MinValue, DayPrice.MaxValue);

            public readonly DayPrice Buy;
            public readonly DayPrice Sell;
            public readonly int Profit;


            public BuySellTrade(DayPrice buy, DayPrice sell)
            {
                Buy = buy;
                Sell = sell;
                Profit = sell.Price - buy.Price;
            }


            public static BuySellTrade NotInited => _notInited;
            public static BuySellTrade Minvalue => _min;
            public static BuySellTrade MaxValue => _max;


            public bool IsInited => Buy.IsInited && Sell.IsInited;


            public static BuySellTrade Merge(BuySellTrade left, BuySellTrade right)
            {
                return new BuySellTrade(left.Buy, right.Sell);
            }

            public bool Equals(BuySellTrade other)
            {
                return this == other;
            }

            public override bool Equals(object? obj)
            {
                return obj is BuySellTrade && Equals((BuySellTrade)obj);
            }

            public int CompareTo(BuySellTrade other)
            {
                return Profit.CompareTo(other.Profit);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Buy, Sell);
            }

            public static bool operator ==(BuySellTrade left, BuySellTrade right)
            {
                return left.Buy == right.Buy
                   && left.Sell == right.Sell;
            }

            public static bool operator !=(BuySellTrade left, BuySellTrade right)
            {
                return left.Buy != right.Buy
                   && left.Sell != right.Sell;
            }

            public static bool operator >(BuySellTrade left, BuySellTrade right)
            {
                return left.Profit > right.Profit;
            }

            public static bool operator <(BuySellTrade left, BuySellTrade right)
            {
                return left.Profit < right.Profit;
            }
        }


        private sealed class TradesMerger
        {
            private readonly List<BuySellTrade> _trades;
            private readonly int _maxTradePairsCount;


            public TradesMerger(List<BuySellTrade> trades, int maxTradePairsCount)
            {
                _trades = trades;
                _maxTradePairsCount = maxTradePairsCount;
            }


            public void StartMerging()
            {
                while (_trades.Count > _maxTradePairsCount)
                {
                    Merge();
                }
            }

            private void Merge()
            {
                var minTradeIndex = FindMinIndex();
                _ = TryMerge(minTradeIndex);
            }

            private bool TryMerge(int index)
            {
                bool canMergeLeft = index > 0;
                bool canMergeRight = index < _trades.Count - 1;
                var minTrade = _trades[index];
                _trades.RemoveAt(index);

                var leftTrade = canMergeLeft
                    ? _trades[index - 1]
                    : default;

                var rightTrade = canMergeRight
                    ? _trades[index]
                    : default;

                BuySellTrade leftMergedTrade = canMergeLeft
                    ? BuySellTrade.Merge(leftTrade, minTrade)
                    : default;

                BuySellTrade rightMergedTrade = canMergeRight
                    ? BuySellTrade.Merge(minTrade, rightTrade)
                    : default;

                int leftMergeProfit = canMergeLeft
                    ? leftMergedTrade.Profit - leftTrade.Profit
                    : int.MinValue;

                int rightMergeProfit = canMergeRight
                    ? rightMergedTrade.Profit - rightTrade.Profit
                    : int.MinValue;

                canMergeLeft = leftMergeProfit > 0;
                canMergeRight = rightMergeProfit > 0 && rightMergeProfit > leftMergeProfit;

                if (canMergeRight)
                {
                    // Merging right.

                    _trades[index] = rightMergedTrade;
                    return true;
                }
                else if (canMergeLeft)
                {
                    // Mergint left.

                    _trades[index - 1] = leftMergedTrade;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private int FindMinIndex()
            {
#pragma warning disable IDE0018 // Объявление встроенной переменной
                int minProfit;
                int minIndex;
#pragma warning restore IDE0018 // Объявление встроенной переменной
                var span = CollectionsMarshal.AsSpan(_trades);
                SetAsMin(0, out minProfit, out minIndex, in span);
                var len = span.Length;

                for (int i = 0; i < len; i++)
                {
                    if (span[i].Profit < minProfit)
                        SetAsMin(in i, out minProfit, out minIndex, in span);
                }

                return minIndex;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void SetAsMin(in int index, out int minProfit, out int minIndex, in Span<BuySellTrade> span)
            {
                minProfit = span[index].Profit;
                minIndex = index;
            }
        }

        public static void Solve()
        {
            SolveImpl1();
        }

        private static void SolveImpl1()
        {
            var trades = GetTrades();

            MergeTradesUpTo(trades, 2);

            Console.WriteLine(trades.Count);

            foreach (var trade in trades)
            {
                Console.WriteLine($"{trade.Buy.DayId} {trade.Sell.DayId}");
            }
        }

        private static void MergeTradesUpTo(List<BuySellTrade> trades, int maxPairsCount)
        {
            var merger = new TradesMerger(trades, maxPairsCount);
            merger.StartMerging();
        }

        private static List<BuySellTrade> GetTrades()
        {
            var prices = GetPrices();
            var trades = GetTrades(prices);
            return trades;
        }

        private static int[] GetPrices()
        {
            _ = InOutUtils.Read.Int();
            return InOutUtils.Read.ArrayOf.Ints();
        }

        private static List<BuySellTrade> GetTrades(int[] prices)
        {
            var len = prices.Length;
            var trades = new List<BuySellTrade>(len);

            DayPrice buy = default;
            bool wannaBuy = true;
            int prevPrice = wannaBuy ? int.MaxValue : int.MinValue;

            for (int i = 0; i < len; i++)
            {
                var price = prices[i];

                if (wannaBuy)
                {
                    if (price > prevPrice)
                    {
                        // prevPrice is localMin.

                        // i is index, we should use 1-based numeration
                        // so we should i + 1, but localMin is prev index
                        // so we simplify (i - 1) + 1 to i.

                        buy = new(i, prevPrice);
                        wannaBuy = false;
                    }
                }
                else // wannaSell
                {
                    if (price < prevPrice)
                    {
                        // price is localMax.

                        trades.Add(new(buy, new(i, prevPrice)));
                        wannaBuy = true;
                    }
                }

                prevPrice = price;
            }

            if (!wannaBuy)
            {
                if (prevPrice > buy.Price)
                {
                    trades.Add(new(buy, new(len, prevPrice)));
                }
            }

            return trades;
        }
    }
}
