using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Helpers
{
    public class TestAnalysisHelper
    {
        private List<double> _motions;
        private double _timeLapsed;
        private int _numRecords;

        public TestAnalysisHelper(List<double> motions, double timeLapsed, int numRecords)
        {
            this._motions = motions;
            this._timeLapsed = timeLapsed;
            this._numRecords = numRecords;
        }

        private int NearestAbsoluteValue(double thirtySecChunk, int records)
        {
            // Convert chunk to whole number
            thirtySecChunk = Math.Round(thirtySecChunk);

            if (records % thirtySecChunk == 0)
            {
                return (int)thirtySecChunk;
            }
            else
            {
                return NearestAbsoluteValue(thirtySecChunk + 1, records);
            }
        }

        public double Period(double timeLapsed, int numRecords)
        {
            return (timeLapsed * 60) / numRecords;
        }

        public double Slope()
        {
            double period = Period(_timeLapsed, _numRecords);
            double thirtySecChunk = 30 / period;

            int closestAbsoluteValue = NearestAbsoluteValue(thirtySecChunk, _numRecords);

            double chunksInSecs = closestAbsoluteValue * period;

            // How many times to loop
            int chunks = _numRecords / closestAbsoluteValue;

            double slopeSum = 0;

            for (int i = 0; i < chunks; i++)
            {
                /// Get range starting from which chunk 1..64 then count from there + 64,
                /// if chunks = 64.
                /// Also supply x, which will be i * chunks. E.g. 0, 64, 128 
                Sigma sigma = GetSigmas(_motions.GetRange(i * closestAbsoluteValue, closestAbsoluteValue), (i * closestAbsoluteValue) + 1);

                slopeSum += (_numRecords * sigma.XY - sigma.X * sigma.Y) / (_numRecords * sigma.XSquared - Math.Pow(sigma.X, 2));
            }

            // Return average slope
            return slopeSum / chunks;
        }

        private Sigma GetSigmas(List<double> motions, int x)
        {
            Sigma sigma = new Sigma();

            foreach (double heel in motions)
            {
                sigma.X += x;
                sigma.Y += heel / 60;
                sigma.XSquared += Math.Pow(x, 2);
                sigma.XY += x * (heel / 60);

                x++;
            }

            return sigma;
        }

        public double StandardDeviation(IEnumerable<double> values)
        {
            double ret = 0;
            int count = values.Count();
            if (count > 1)
            {
                // Compute the average
                double avg = values.Average();

                // Perform the sum of (value - avg)^2
                double sum = values.Sum(d => (d - avg) * (d - avg));

                // Put it all together
                ret = Math.Sqrt(sum / count);
            }

            return ret;
        }
    }

    public class Sigma
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double XSquared { get; set; }
        public double XY { get; set; }
    }
}