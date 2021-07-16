using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.Models
{
    public class GeneralData
    {
        public double TransversalMoment { get; set; }
        public double LongitudinalMoment { get; set; }
        public double AverageHeel { get; set; }
        public double AverageTrim { get; set; }
        public double Draught { get; set; }
        public double CorrespondingDisplacement { get; set; }

        public GeneralData(double transversalMoment, double longitudinalMoment, double averageHeel, double averageTrim, double draught, double correspondingDisplacement)
        {
            TransversalMoment = transversalMoment;
            LongitudinalMoment = longitudinalMoment;
            AverageHeel = averageHeel;
            AverageTrim = averageTrim;
            Draught = draught;
            CorrespondingDisplacement = correspondingDisplacement;
        }
    }
}
