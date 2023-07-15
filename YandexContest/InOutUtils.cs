namespace DevourDev.SheetCodeUtility
{
    public static class InOutUtils
    {
        public static class Read
        {
            public static class ArrayOf
            {
                public static int[] Ints()
                {
                    return Parse.ArrayOf.Ints(Console.ReadLine()!, null!);
                }

                public static int[] Ints(char[] separators)
                {
                    return Parse.ArrayOf.Ints(Console.ReadLine()!, separators);
                }

                public static long[] Longs()
                {
                    return Parse.ArrayOf.Longs(Console.ReadLine()!, null!);
                }

                public static long[] Longs(char[] separators)
                {
                    return Parse.ArrayOf.Longs(Console.ReadLine()!, separators);
                }

                public static double[] Doubles()
                {
                    return Parse.ArrayOf.Doubles(Console.ReadLine()!, null!);
                }

                public static double[] Doubles(char[] separators)
                {
                    return Parse.ArrayOf.Doubles(Console.ReadLine()!, separators);
                }

                public static decimal[] Decimals()
                {
                    return Parse.ArrayOf.Decimals(Console.ReadLine()!, null!);
                }

                public static decimal[] Decimals(char[] separators)
                {
                    return Parse.ArrayOf.Decimals(Console.ReadLine()!, separators);
                }
            }


            public static int Int()
            {
                return int.Parse(Console.ReadLine()!);
            }

            public static long Long()
            {
                return long.Parse(Console.ReadLine()!);
            }

            public static double Double()
            {
                return double.Parse(Console.ReadLine()!);
            }

            public static decimal Decimal()
            {
                return decimal.Parse(Console.ReadLine()!);
            }
        }


        public static class Parse
        {
            public static class ArrayOf
            {
                public static int[] Ints(string s, params char[] separators)
                {
                    var slices = s.Split(separators, options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var len = slices.Length;
                    int[] ints = new int[len];

                    for (int i = 0; i < len; i++)
                    {
                        ints[i] = int.Parse(slices[i]);
                    }

                    return ints;
                }

                public static long[] Longs(string s, params char[] separators)
                {
                    var slices = s.Split(separators, options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var len = slices.Length;
                    long[] nums = new long[len];

                    for (int i = 0; i < len; i++)
                    {
                        nums[i] = long.Parse(slices[i]);
                    }

                    return nums;
                }

                public static double[] Doubles(string s, params char[] separators)
                {
                    var slices = s.Split(separators, options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var len = slices.Length;
                    double[] nums = new double[len];

                    for (int i = 0; i < len; i++)
                    {
                        nums[i] = double.Parse(slices[i]);
                    }

                    return nums;
                }

                public static decimal[] Decimals(string s, params char[] separators)
                {
                    var slices = s.Split(separators, options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var len = slices.Length;
                    decimal[] nums = new decimal[len];

                    for (int i = 0; i < len; i++)
                    {
                        nums[i] = decimal.Parse(slices[i]);
                    }

                    return nums;
                }
            }


            public static class Separators
            {
                public const char Comma = ',';
                public const char Semicolon = ';';

                public static readonly char[] Default = Array.Empty<char>();
            }
        }
    }
}
