using System;
using System.IO;

namespace Periodic
{
    public class TsParser
    {
        public static Timeseries ParseTimeseries(string tabSeparatedTimeseries)
        {
            var reader = new StringReader(tabSeparatedTimeseries);
            var result = new Timeseries();

            var line = reader.ReadLine();
            while (line != null)
            {
                var tvq = ParseLine(line);
                result.Add(tvq);
                line = reader.ReadLine();
            }

            return result;
        }

        private static Tvq ParseLine(string line)
        {
            var parts = line.Split('\t');
            var t = DateTime.Parse(parts[0]);
            var v = int.Parse(parts[1]);
            return new Tvq(t, v, Quality.Ok);
        }
    }
}