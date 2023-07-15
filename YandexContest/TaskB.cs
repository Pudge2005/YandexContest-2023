using DevourDev.SheetCodeUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YandexContest
{
    internal static class TaskB
    {
        private readonly struct InputData
        {
            public readonly int Count;
            public readonly int PerfectWeight;
            public readonly long TimeLeft;

            public readonly int[] Weights;


            public InputData(int count, int perfectWeight, long timeLeft, int[] weights)
            {
                Count = count;
                PerfectWeight = perfectWeight;
                TimeLeft = timeLeft;
                Weights = weights;
            }

            public static InputData Read(FileStream fs)
            {
                var sr = new StreamReader(fs);
                var firstLineArgs = InOutUtils.Parse.ArrayOf.Longs(sr.ReadLine()!);
                int count = (int)firstLineArgs[0];
                int perfectWeight = (int)firstLineArgs[1];
                long timeLeft = firstLineArgs[2];
                int[] weights = InOutUtils.Parse.ArrayOf.Ints(sr.ReadLine()!);
                return new(count, perfectWeight, timeLeft, weights);
            }
        }


        private readonly struct OutputData
        {
            public readonly int MaxCount;
            public readonly List<int> Perfects;


            public OutputData(int maxCount, List<int> perfects)
            {
                MaxCount = maxCount;
                Perfects = perfects;
            }


            public void Log()
            {
                Console.WriteLine(MaxCount);

                if (Perfects != null && Perfects.Count > 0)
                    Console.WriteLine(string.Join(' ', Perfects));
            }
        }


        private readonly struct FigurePerfectionData : IComparable<FigurePerfectionData>
        {
            public readonly int FigureId;
            public readonly int PerfectionTime;


            public FigurePerfectionData(int figureId, int perfectionTime)
            {
                FigureId = figureId;
                PerfectionTime = perfectionTime;
            }


            public int CompareTo(FigurePerfectionData other)
            {
                return PerfectionTime.CompareTo(other.PerfectionTime);
            }
        }


        public static void Solve()
        {
            var inputData = GetData();

            int count = inputData.Count;
            int perfectWeight = inputData.PerfectWeight;
            long timeLeftTotal = inputData.TimeLeft;
            int[] weights = inputData.Weights;

            FigurePerfectionData[] perfectionDatas = new FigurePerfectionData[count];

            for (int i = 0; i < count; i++)
            {
                var perfectionTime = GetPerfectionTime(weights[i], perfectWeight);

                if (perfectionTime > timeLeftTotal)
                {
                    perfectionTime = int.MaxValue;
                }

                perfectionDatas[i] = new FigurePerfectionData(i + 1, perfectionTime);
            }

            Array.Sort(perfectionDatas);
            List<int> perfectFigures = new();

            long timeLeft = timeLeftTotal;

            for (int i = 0; timeLeft > 0 && i < count; i++)
            {
                var perfectionData = perfectionDatas[i];

                if ((timeLeft -= perfectionData.PerfectionTime) <= 0)
                    break;

                perfectFigures.Add(perfectionData.FigureId);
            }

            var outputData = new OutputData(perfectFigures.Count, perfectFigures);
            outputData.Log();
        }

        private static int GetPerfectionTime(int weight, int perfectWeight)
        {
            return Math.Abs(weight - perfectWeight);
        }

        private static InputData GetData()
        {
            const string inputFileName = "input.txt";
            using var fs = File.OpenRead(inputFileName);
            return InputData.Read(fs);
        }
    }
}
