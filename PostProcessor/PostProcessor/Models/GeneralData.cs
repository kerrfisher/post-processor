using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class GeneralData
    {
        public int Number { get; set; }
        public double TransversalMoment { get; set; }
        public double LongitudinalMoment { get; set; }
        public double AverageHeel { get; set; }
        public double AverageTrim { get; set; }
        public double Draught { get; set; }
        public double CorrespondingDisplacement { get; set; }

        public GeneralData(int number, double transversalMoment, double longitudinalMoment, double averageHeel, double averageTrim, double draught, double correspondingDisplacement)
        {
            Number = number;
            TransversalMoment = transversalMoment;
            LongitudinalMoment = longitudinalMoment;
            AverageHeel = averageHeel;
            AverageTrim = averageTrim;
            Draught = draught;
            CorrespondingDisplacement = correspondingDisplacement;
        }
    }
}
