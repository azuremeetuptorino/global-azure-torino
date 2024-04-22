using FunctionCalling.Controllers.Dtos;
using System;

namespace FunctionCalling
{
    public static class DemoHelpers
    {

        private static readonly List<string> CreatedBy = new List<string>
            {"enrico.sabbadin@msc.com"/*, "marco@msc.com", "giorgio@msc.com", "franco@msc.com"*/};

        public static string RandomEmail()
        {
            return CreatedBy[Random.Next(0, CreatedBy.Count)];
        }

        public static int RandomNumber(int from, int to)
        {
            return Random.Next(from, to);

        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static ContainerType RandomContainerType()
        {
            var values = Enum.GetValues<ContainerType>();
            return values[Random.Next(0, values.Length)];
        }

        private static readonly Random Random = new Random();

        public static QuoteStatus RandomQuoteStatus()
        {
            var values = Enum.GetValues<QuoteStatus>();
            return values[Random.Next(0, values.Length)];
        }

        public static WeightUnit RandomWeightUnit()
        {
            var values = Enum.GetValues<WeightUnit>();
            return values[Random.Next(0, values.Length)];
        }
    }
}

