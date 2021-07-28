using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Helpers
{
    public class SemisubDraught
    {
        private double[,] aa = new double[11, 11];
        private double[] cc = new double[11];

        private double[,] coordinates = new double[5, 3];

        public double draught;

        public void SetCoordinates(int num)
        {
            if (num == 1)
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
                StreamReader file = new StreamReader(files.First(x => x.Contains("Gauge")));
                string line;
                file.ReadLine();
                int i = 0;
                while ((line = file.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    coordinates[i + 1, 0] = Double.Parse(values[1]);
                    coordinates[i + 1, 1] = Double.Parse(values[2]);
                    i++;
                }
                file.Close();
            }
            else if (num == 0)
            {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
                StreamReader file = new StreamReader(files.First(x => x.Contains("Mark")));
                string line;
                file.ReadLine();
                int i = 0;
                while ((line = file.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    coordinates[i + 1, 0] = Double.Parse(values[1]);
                    coordinates[i + 1, 1] = Double.Parse(values[2]);
                    i++;
                }
                file.Close();
            }
        }

        public void ComputePlane(double[] draughts)
        {
            bool check;

            for (int i = 1; i <= 3; i++)
            {
                cc[i] = 0;
                for (int j = 1; j <= 3; j++)
                {
                    aa[i, j] = 0;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                SumTerms(draughts[i], GetRow(coordinates, i + 1));
            }

            check = true;

            InvertedMatrix(3, aa, ref check);

            if (!check)
            {
                draught = 9999;
            }
            else
            {
                MatVec(3, cc);

                draught = cc[3];
            }
        }

        public void SumTerms(double colDraught, double[] column)
        {
            aa[1, 1] += column[0] * column[0];
            aa[1, 2] += column[0] * column[1];
            aa[1, 3] += column[0];

            aa[2, 1] = aa[1, 2];
            aa[2, 2] += column[1] * column[1];
            aa[2, 3] += column[1];

            aa[3, 1] = aa[1, 3];
            aa[3, 2] = aa[2, 3];
            aa[3, 3]++;

            cc[1] += column[0] * colDraught;
            cc[2] += column[1] * colDraught;
            cc[3] += colDraught;
        }

        // Method to retrieve a row from a dmatrix (multidimensional array)
        public double[] GetRow(double[,] matrix, int rowNum)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNum, x])
                .ToArray();
        }

        public void InvertedMatrix(int n, double[,] a, ref bool outcome)
        {
            double[,] c = new double[11, 11];

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    c[i, j] = 0;
                }
            }

            aa = c;

            for (int i = 1; i <= n; i++)
            {
                c[i, i] = 1;
            }

            bool notSingular = true;

            for (int d = 1; d <= n; d++)
            {
                if (notSingular)
                {
                    int e = d;
                    while (a[e, d] == 0 && notSingular)
                    {
                        e++;
                        if (e > n)
                        {
                            notSingular = false;
                        }
                    }

                    if (notSingular)
                    {
                        // swap row e with d if a[d,d] was zero
                        if (d != e)
                        {
                            for (int i = 1; i <= n; i++)
                            {
                                Swap(ref a[d, i], ref a[e, i]);
                                Swap(ref c[d, i], ref c[e, i]);
                            }
                        }

                        // normalise the row with the diagonal term a[d,d]
                        double factor = a[d, d];
                        for (int i = 1; i <= n; i++)
                        {
                            a[d, i] /= factor;
                            c[d, i] /= factor;
                        }

                        for (int i = 1; i <= n; i++)
                        {
                            if (i != d)
                            {
                                factor = a[i, d];
                                for (int j = 1; j <= n; j++)
                                {
                                    a[i, j] -= a[d, j] * factor;
                                    c[i, j] -= -c[d, j] * factor;
                                }
                            }
                        }
                    }
                }
            }

            if (notSingular)
            {
                aa = c;
                outcome = true;
            }
            else
            {
                outcome = false;
            }
        }

        public static void Swap(ref double lhs, ref double rhs)
        {
            double temp = lhs;
            lhs = rhs;
            rhs = temp;
        }


        public void MatVec(int n, double[] u)
        {
            for (int i = 1; i <= n; i++)
            {
                double sum = 0;
                for (int j = 1; j <= n; j++)
                {
                    sum += aa[i, j] * u[j];
                }
                cc[i] = sum;
            }
        }
    }
}
