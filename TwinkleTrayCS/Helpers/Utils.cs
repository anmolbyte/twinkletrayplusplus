using System;
using System.Collections.Generic;
using System.Linq;
using TwinkleTrayCS.Models;

namespace TwinkleTrayCS.Helpers
{
    public static class Utils
    {
        public static int ParseTime(string time)
        {
            try
            {
                var parts = time.Split(':');
                return (int.Parse(parts[0]) * 60) + int.Parse(parts[1]);
            }
            catch { return 0; }
        }

        public static double Lerp(double start, double end, double perc)
        {
            return start * (1 - perc) + end * perc;
        }

        public static int GetCalibratedValue(int value, List<CalibrationPoint> points, bool reverse = false)
        {
            if (points == null || points.Count == 0) return value;

            var sortedPoints = points.OrderBy(p => p.Input).ToList();
            if (!sortedPoints.Any(p => p.Input == 0)) sortedPoints.Insert(0, new CalibrationPoint { Input = 0, Output = 0 });
            if (!sortedPoints.Any(p => p.Input == 100)) sortedPoints.Add(new CalibrationPoint { Input = 100, Output = 100 });

            if (reverse)
            {
                for (int i = 0; i < sortedPoints.Count - 1; i++)
                {
                    var p1 = sortedPoints[i];
                    var p2 = sortedPoints[i + 1];
                    var minOut = Math.Min(p1.Output, p2.Output);
                    var maxOut = Math.Max(p1.Output, p2.Output);

                    if (value >= minOut && value <= maxOut)
                    {
                        if (p2.Output == p1.Output) return (p1.Input + p2.Input) / 2;
                        double ratio = (double)(value - p1.Output) / (p2.Output - p1.Output);
                        return (int)Math.Round(p1.Input + ratio * (p2.Input - p1.Input));
                    }
                }
            }
            else
            {
                for (int i = 0; i < sortedPoints.Count - 1; i++)
                {
                    var p1 = sortedPoints[i];
                    var p2 = sortedPoints[i + 1];

                    if (value >= p1.Input && value <= p2.Input)
                    {
                        double ratio = (double)(value - p1.Input) / (p2.Input - p1.Input);
                        return (int)Math.Round(p1.Output + ratio * (p2.Output - p1.Output));
                    }
                }
            }

            return value;
        }
    }
}
