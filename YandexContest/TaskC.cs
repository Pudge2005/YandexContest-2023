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
            private readonly struct TradesMergingHelper
            {
                private readonly LinkedListNode<BuySellTrade> _node;


                public TradesMergingHelper(LinkedListNode<BuySellTrade> node)
                {
                    _node = node;
                }


                public readonly bool CanMergeLeft => _node.Previous != null;
                public readonly bool CanMergeRight => _node.Next != null;


                public readonly int MergeLeftProfit
                {
                    get
                    {
                        var leftNode = _node.Previous;

                        if (leftNode == null)
                            return int.MinValue;

                        var leftTrade = leftNode.Value;
                        var mergedProfit = _node.Value.Sell.Price - leftTrade.Buy.Price;
                        return mergedProfit - leftTrade.Profit;
                    }
                }

                public readonly int MergeRightProfit
                {
                    get
                    {
                        var rightNode = _node.Next;

                        if (rightNode == null)
                            return int.MinValue;

                        var rightTrade = rightNode.Value;
                        var mergedProfit = rightTrade.Sell.Price - _node.Value.Buy.Price;
                        return mergedProfit - rightTrade.Profit;
                    }
                }


                public readonly void MergeLeft()
                {
                    var leftNode = _node.Previous;
                    leftNode!.Value = BuySellTrade.Merge(leftNode.Value, _node.Value);
                    Destroy();
                }

                public readonly void MergeRight()
                {
                    var rightNode = _node.Next;
                    rightNode!.Value = BuySellTrade.Merge(_node.Value, rightNode.Value);
                    Destroy();
                }

                public readonly void Destroy()
                {
                    _node.List!.Remove(_node);

                }

                public readonly void Merge()
                {
                    int leftProfit = MergeLeftProfit;
                    int rightProfit = MergeRightProfit;

                    bool canMergeLeft = leftProfit > 0;
                    bool shouldMergeRight = rightProfit > 0 && rightProfit > leftProfit;

                    if (shouldMergeRight)
                    {
                        MergeRight();
                    }
                    else if (canMergeLeft)
                    {
                        MergeLeft();
                    }
                    else
                    {
                        Destroy();
                    }
                }
            }


            private static readonly IPool<List<LinkedListNode<BuySellTrade>>> _tradeNodesPool =
                ListsPool<LinkedListNode<BuySellTrade>>.Create();

            private readonly LinkedList<BuySellTrade> _trades;
            private readonly int _maxTradePairsCount;


            public TradesMerger(LinkedList<BuySellTrade> trades, int maxTradePairsCount)
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
                var minTradeNodes = _tradeNodesPool.Rent();
                FindMinTradeNodes(minTradeNodes);
                Merge(minTradeNodes);
                _tradeNodesPool.Return(minTradeNodes);
            }

            private void FindMinTradeNodes(List<LinkedListNode<BuySellTrade>> minTradeNodes)
            {
                var minTrade = _trades.First;
                int minProfit = minTrade!.Value.Profit;
                minTradeNodes.Add(minTrade);

                var curNode = minTrade.Next;

                while (curNode != null)
                {
                    var profit = curNode.Value.Profit;

                    if (profit == minProfit)
                    {
                        minTradeNodes.Add(curNode);
                    }
                    else if (profit < minProfit)
                    {
                        minTradeNodes.Clear();
                        minTradeNodes.Add(curNode);
                        minProfit = profit;
                    }

                    curNode = curNode.Next;
                }
            }

            private static void Merge(IReadOnlyList<LinkedListNode<BuySellTrade>> minTradeNodes)
            {
                int minMaxProfitIndex = 0;
                int minMaxProfit = GetMaxProfit(0);

                for (int i = 1; i < minTradeNodes.Count; i++)
                {
                    var maxProfit = GetMaxProfit(i);

                    if (maxProfit < minMaxProfit)
                    {
                        minMaxProfit = maxProfit;
                        minMaxProfitIndex = i;
                    }
                }

                var helper = new TradesMergingHelper(minTradeNodes[minMaxProfitIndex]);
                helper.Merge();


                int GetMaxProfit(int index)
                {
                    var helper = new TradesMergingHelper(minTradeNodes[index]);
                    return Math.Max(helper.MergeLeftProfit, helper.MergeRightProfit);
                }
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

        private static void MergeTradesUpTo(LinkedList<BuySellTrade> trades, int maxPairsCount)
        {
            var merger = new TradesMerger(trades, maxPairsCount);
            merger.StartMerging();
        }

        private static LinkedList<BuySellTrade> GetTrades()
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

        private static LinkedList<BuySellTrade> GetTrades(int[] prices)
        {
            var len = prices.Length;
            var trades = new LinkedList<BuySellTrade>();

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

                        trades.AddLast(new BuySellTrade(buy, new(i, prevPrice)));
                        wannaBuy = true;
                    }
                }

                prevPrice = price;
            }

            if (!wannaBuy)
            {
                if (prevPrice > buy.Price)
                {
                    trades.AddLast(new BuySellTrade(buy, new(len, prevPrice)));
                }
            }

            return trades;
        }
    }
}
