using DevourDev.SheetCodeUtility.Testing;
using YandexContest;

internal class Program
{
    private static void Main()
    {
        var tester = PrepareTester();
        AddTests(tester);

        System.Action method = TaskC.Solve;

        var results = tester.StartTests(method, true, true);

        foreach (var result in results)
        {
            Console.WriteLine(result);
        }

        string input = Console.ReadLine()!;
        Console.WriteLine(input);
    }

    private static InOutTester PrepareTester()
    {
        var tester = new InOutTester();

        tester.GetTestBuilder()
            .WriteLine("6")
            .WriteLine("1 4 2 3 3 5")
            .BeginExpectedOutput()
            .WriteLine("2")
            .WriteLine("1 2")
            .WriteLine("3 6")
            .AddTest()

            .WriteLine("5")
            .WriteLine("10 5 5 7 6")
            .BeginExpectedOutput()
            .WriteLine("1")
            .WriteLine("3 4")
            .AddTest()

            .WriteLine("3")
            .WriteLine("3 2 2")
            .BeginExpectedOutput()
            .WriteLine("0")
            .AddTest();

        return tester;
    }

    private static void AddTests(InOutTester tester)
    {
        AddTest(tester, new int[] { 100, 99, 98, 98, 90, 90, 90, 10, 9, 8, 6, 4, 3 }, Array.Empty<int>());

        AddTest(tester, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 500, 100, 150, 0, 100, 55, 22 },
            new int[] { 1, 12, 15, 16 });

        AddTest(tester, new int[] { 839, 717, 138, 223, 145, 807, 63, 826, 185, 349, 267, 470, 372,
            88, 576, 421, 704, 355, 849, 597, 112, 785, 525, 683, 46, 172, 960, 958, 523, 703, 360,
            510, 981, 704, 256, 644, 894, 762, 389, 417 }, new int[] { 7, 19, 25, 33 });
    }

    private static void AddTest(InOutTester tester, int[] days, int[] buySellOperations)
    {
        int opPairsCount = buySellOperations.Length / 2;

        var builder = tester.GetTestBuilder()
             .WriteLine(days.Length.ToString())
             .WriteLine(string.Join(' ', days))
             .BeginExpectedOutput()
             .WriteLine(opPairsCount.ToString());

        for (int i = 0; i < opPairsCount * 2; i += 2)
        {
            builder.WriteLine($"{buySellOperations[i]} {buySellOperations[i + 1]}");
        }

        builder.AddTest();
    }


    //private static void Main2()
    //{
    //    GenerateFile();

    //    //TaskB.Solve();

    //    Prewarm();

    //    var line = File.ReadAllLines("input.txt")[1];

    //    ParseGpt(line);
    //    ParseUtility(line);
    //    ParseRustLib(line);
    //    ParseCppWinLib(line);
    //}

    //private static void ParseCppWinLib(string line)
    //{
    //    Console.WriteLine(nameof(ParseCppWinLib));

    //    ParseStringToLongs(line, out var ptr, out var countPtr);
    //    var count = countPtr.ToInt32();

    //    unsafe
    //    {
    //        long* numbers = (long*)ptr;
    //        for (int i = 0; i < count; i++)
    //        {
    //            Console.Write(numbers[i]);
    //            Console.Write(' ');
    //        }
    //    }

    //    FreeMemory(ptr);
    //}


    //private static void ParseRustLib(string line)
    //{
    //    Console.WriteLine(nameof(ParseRustLib));

    //    IntPtr numbersPtr = parse_number_string(line, out int size);

    //    unsafe
    //    {
    //        long* numbers = (long*)numbersPtr;
    //        for (int i = 0; i < size; i++)
    //        {
    //            Console.Write(numbers[i]);
    //            Console.Write(' ');
    //        }
    //    }

    //    free_numbers(numbersPtr);
    //}

    //private static void ParseUtility(string line)
    //{
    //}

    //private static void ParseGpt(string line)
    //{

    //}

    //private static void Prewarm()
    //{
    //}

    //private static void ThrowIfIntOverflow(long v)
    //{
    //    if (v != (int)v)
    //        throw new Exception($"{v}, {(int)v}");
    //}

    //private static void GenerateFile()
    //{
    //    var fs = File.Create("input.txt");
    //    using var sw = new StreamWriter(fs);

    //    long n = 2 * (long)Math.Pow(10, 5);
    //    long x = 10 * (long)Math.Pow(10, 9);
    //    long t = 3 * (long)Math.Pow(10, 14);

    //    //ThrowIfIntOverflow(n);
    //    //ThrowIfIntOverflow(x);
    //    //ThrowIfIntOverflow(t);


    //    sw.WriteLine($"{n} {x} {t}");
    //    var rng = new Random(10);
    //    var maxNumToWrite = (int)Math.Pow(10, 9);

    //    for (int i = 0; i < n; i++)
    //    {
    //        if (i != 0)
    //            sw.Write(' ');

    //        int numToWrite;

    //        if (i == 100)
    //        {
    //            numToWrite = 0;
    //        }
    //        else if (i == 1000)
    //        {
    //            numToWrite = maxNumToWrite;
    //        }
    //        else
    //        {
    //            numToWrite = rng.Next(0, maxNumToWrite + 1);
    //        }

    //        sw.Write(numToWrite);

    //    }
    //}
}
