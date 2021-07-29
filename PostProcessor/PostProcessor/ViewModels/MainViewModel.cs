using PostProcessor.Helpers;
using PostProcessor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PostProcessor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // File name of current MOSIS output
        public string Filename { get; set; }

        // Observable objects for DataGrid's
        // Observable objects have notify property handling ingrained
        public ObservableCollection<MotionsData> MotionsData = new ObservableCollection<MotionsData>();
        public ObservableCollection<GeneralData> GeneralData = new ObservableCollection<GeneralData>();
        public ObservableCollection<TankData> TankFromData = new ObservableCollection<TankData>();
        public ObservableCollection<TankData> TankToData = new ObservableCollection<TankData>();

        // Holds data related to the calibration run
        Calibration calibration = new Calibration();

        // List of draughts, contains draughts at each shift
        private List<Draughts> AllDraughts = new List<Draughts>();

        private List<VesselCondition> vesselConditions = new List<VesselCondition>();

        // Determines whether draughts were read from draught marks or gauges
        bool isDraughtReadFromMarks;

        // Date when mosis test was undetaken
        private string _fileDate;
        public string FileDate
        {
            get
            {
                return _fileDate;
            }
            set
            {
                _fileDate = value;
                OnPropertyChanged(nameof(FileDate));
            }
        }

        // Central point for reading file contents
        public void ReadFileContents(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath))
            {
                // Read file date from file
                FileDate = reader.ReadLine();

                // Iterate over general information
                for (int i = 0; i < 27; i++)
                {
                    reader.ReadLine();
                }

                string[] draughts = reader.ReadLine().Split(',');
                AllDraughts.Add(new Draughts(Convert.ToDouble(draughts[0]), Convert.ToDouble(draughts[1]), Convert.ToDouble(draughts[2]), Convert.ToDouble(draughts[3])));

                string line = reader.ReadLine();
                VesselCondition vesselCondition = new VesselCondition();
                vesselCondition.Displacement = Convert.ToDouble(line.Split(',')[0]);

                for (int i = 0; i < 2; i++)
                {
                    reader.ReadLine();
                }

                ReadCalibrationRun(reader);

                // Line with test number
                line = reader.ReadLine();
                // Recursive method that will repeat if there are more tests
                ReadShift(reader, line);

                // Close stream
                reader.Close();
            }
        }

        private void ReadCalibrationRun(StreamReader reader)
        {
            List<Motions> calibrationRun = new List<Motions>();
            for (int i = 0; i < 1024; i++)
            {
                string[] line = reader.ReadLine().Split(',');
                calibrationRun.Add(new Motions(Convert.ToDouble(line[0]), Convert.ToDouble(line[1])));
            }

            calibration.Motions = calibrationRun;

            List<double> heels = new List<double>();
            List<double> pitchs = new List<double>();

            foreach (Motions motion in calibrationRun)
            {
                heels.Add(motion.Heel);
                pitchs.Add(motion.Pitch);
            }

            // Calculate calibrations slope and standard deviation
            TestAnalysisHelper calibrationHeelAnalysisHelper = new TestAnalysisHelper(heels, 512, 1024);
            calibration.Heel = new LinearData(calibrationHeelAnalysisHelper.Slope(), calibrationHeelAnalysisHelper.StandardDeviation(heels.AsEnumerable()), "");
            TestAnalysisHelper calibrationPitchAnalysisHelper = new TestAnalysisHelper(pitchs, 512, 1024);
            calibration.Pitch = new LinearData(calibrationPitchAnalysisHelper.Slope(), calibrationPitchAnalysisHelper.StandardDeviation(pitchs.AsEnumerable()), "");

            for (int i = 0; i < 8; i++)
            {
                reader.ReadLine();
            }
        }

        /// <summary>
        /// Recursively reads each shift and adds to tables.
        /// </summary>
        /// <param name="reader">The stream reading from the file.</param>
        /// <param name="line">The line which contains the test/shift number.</param>
        private void ReadShift(StreamReader reader, string line)
        {
            // Read shift number
            string numRecord = line.Substring(line.IndexOf(':') + 1).Trim();

            // Read draught at shift
            line = reader.ReadLine();
            string[] draughts = line.Substring(line.IndexOf(':') + 1).Split(',');
            AllDraughts.Add(new Draughts(Convert.ToDouble(draughts[0]), Convert.ToDouble(draughts[1]), Convert.ToDouble(draughts[2]), Convert.ToDouble(draughts[3])));
            double meanDraught = draughts.Average(x => Convert.ToDouble(x));

            // Read displacement at shift
            line = reader.ReadLine();
            VesselCondition vesselCondition = new VesselCondition();
            vesselCondition.Displacement = Convert.ToDouble(line.Split(',')[0].Substring(line.IndexOf(':') + 1).Trim());
            vesselCondition.TCP = Convert.ToDouble(line.Split(',')[1].Substring(line.Split(',')[1].IndexOf(':') + 1).Trim());
            vesselConditions.Add(vesselCondition);

            // Read which method was used to read draughts
            line = reader.ReadLine();
            isDraughtReadFromMarks = line.Substring(line.IndexOf(':')).Equals("0");

            reader.ReadLine();
            reader.ReadLine();

            // Read tank from name
            line = reader.ReadLine();
            string tankFrom = line.Substring(0, line.IndexOf(':'));

            reader.ReadLine();

            // Read tank to name
            line = reader.ReadLine();
            string tankTo = line.Substring(0, line.IndexOf(':'));

            // Iterate over heel and pitch data
            for (int i = 0; i < 1028; i++)
            {
                reader.ReadLine();
            }

            // Read average heel and pitch angles
            line = reader.ReadLine();
            string meanHeelAngle = line.Substring(line.IndexOf(':') + 1).Split(',')[0].Trim();
            string meanPitchAngle = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

            // Assign plot of shift no and average motion
            string heelPlot = numRecord + "," + meanHeelAngle;
            string pitchPlot = numRecord + "," + meanPitchAngle;

            // Read heel and pitch slope
            line = reader.ReadLine();
            string heelSlope = line.Substring(line.IndexOf(':') + 1).Split(',')[0].Trim();
            string pitchSlope = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

            // Read heel and pitch standard deviation
            line = reader.ReadLine();
            string heelStdDev = line.Substring(line.IndexOf(':') + 1).Split(',')[0].Trim();
            string pitchStdDev = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

            // Add motions data to DataGrid
            MotionsData.Add(new MotionsData(Convert.ToInt32(numRecord), new LinearData(Convert.ToDouble(heelSlope), Convert.ToDouble(heelStdDev), heelPlot), new LinearData(Convert.ToDouble(pitchSlope), Convert.ToDouble(pitchStdDev), pitchPlot)));

            // Split up and read tank values
            line = reader.ReadLine();
            string[] tankValues = line.Substring(line.IndexOf(':') + 1).Split(',');
            TankData stbdTank = new TankData(Convert.ToInt32(numRecord), tankFrom, Convert.ToDouble(tankValues[0].Trim()), Convert.ToDouble(tankValues[1].Trim()), Convert.ToDouble(tankValues[2].Trim()), Convert.ToDouble(tankValues[3].Trim()));
            // Add tank from data to DataGrid
            TankFromData.Add(stbdTank);

            // Split up and read tank values
            line = reader.ReadLine();
            tankValues = line.Substring(line.IndexOf(':') + 1).Split(',');
            TankData portTank = new TankData(Convert.ToInt32(numRecord), tankTo, Convert.ToDouble(tankValues[0].Trim()), Convert.ToDouble(tankValues[1].Trim()), Convert.ToDouble(tankValues[2].Trim()), Convert.ToDouble(tankValues[3].Trim()));
            // Add tank from data to DataGrid
            TankToData.Add(portTank);

            // Calculate transversal and longitudinal moment
            double transMoment = new double[] { stbdTank.Weight, portTank.Weight }.Average() * new double[] { stbdTank.TCG, portTank.TCG }.Average();
            double longMoment = new double[] { stbdTank.Weight, portTank.Weight }.Average() * new double[] { stbdTank.LCG, portTank.LCG }.Average();

            // Add general data to DataGrid
            GeneralData.Add(new GeneralData(Convert.ToInt32(numRecord), transMoment, longMoment, Convert.ToDouble(meanHeelAngle), Convert.ToDouble(meanPitchAngle), Convert.ToDouble(meanDraught), Convert.ToDouble(vesselCondition.Displacement)));

            line = reader.ReadLine();
            // Since this might be the end of the file, it might return null
            if (line != null)
            {
                // Check line starts with "Test"
                if (line.Substring(0, 4).Equals("Test"))
                {
                    // Recall method
                    ReadShift(reader, line);
                }
            }
        }

        public void CheckConsistencies()
        {
            List<bool> motions = new List<bool>();

            foreach (MotionsData data in MotionsData)
            {
                // Check heel and pitchs slope and stdDev
                if (data.Heel.Slope > OneOrderMagnitudeLarger(calibration.Heel.Slope) || data.Heel.StdDev > OneOrderMagnitudeLarger(calibration.Heel.StdDev) || data.Pitch.Slope > OneOrderMagnitudeLarger(calibration.Pitch.Slope) || data.Pitch.StdDev > OneOrderMagnitudeLarger(calibration.Pitch.StdDev))
                {
                    motions.Add(true);
                }
            }
        }

        // Caculates one magnitude larger of a
        private double OneOrderMagnitudeLarger(double a)
        {
            return a * Math.Pow(10, 1);
        }

        public void VerifyUnexpectedChangeTrim()
        {
            int numOfRecords = TankFromData.Count;

            for (int i = 0; i < numOfRecords; i++)
            {
                // Shift is only transversal
                if ((TankToData[i].LCG - TankFromData[i].LCG) < 1)
                {
                    // If i == 0, compare with calibration
                    if (i == 0)
                    {
                        // Check against calibration's trim
                        if ((GeneralData[i].AverageTrim - calibration.Motions.Average(x => x.Pitch)) < 0.1)
                        {
                            MessageBox.Show($"Trim at {i} exceeds 0.1 deg");
                        }
                    }
                    else
                    {
                        if ((GeneralData[i].AverageTrim - GeneralData[i - 1].AverageTrim) < 0.1)
                        {
                            MessageBox.Show($"Trim at {i} exceeds 0.1 deg");
                        }
                    }
                }
            }
        }

        public void VerifyUnexpectedChangeHeel()
        {
            int numOfRecords = TankFromData.Count;

            for (int i = 0; i < numOfRecords; i++)
            {
                // Shift is only longitudinal
                if ((TankToData[i].TCG - TankFromData[i].TCG) < 1)
                {
                    // If i == 0, compare with calibration
                    if (i == 0)
                    {
                        // Check against calibration's trim
                        if ((GeneralData[i].AverageHeel - calibration.Motions.Average(x => x.Heel)) < 0.1)
                        {
                            MessageBox.Show($"Heel at {i} exceeds 0.1 deg");
                        }
                    }
                    else
                    {
                        if ((GeneralData[i].AverageHeel - GeneralData[i - 1].AverageHeel) < 0.1)
                        {
                            MessageBox.Show($"Heel at {i} exceeds 0.1 deg");
                        }
                    }
                }
            }
        }

        public void CheckDraughtConsistency()
        {
            SemisubDraught semisubDraught = new SemisubDraught();
            // Set the coordinates to be used when computing the plane
            semisubDraught.SetCoordinates(0);

            // Take first draught for now

            // Loop through all draughts, start from one to avoid draughts taken before calibration
            for (int i = 1; i < AllDraughts.Count; i++)
            {
                double[] draughts = new double[] { AllDraughts[i].SF, AllDraughts[i].PF, AllDraughts[i].SA, AllDraughts[i].PA };
                // Loop through each measured draught and check against the calculated draught
                for (int j = 0; j < 4; j++)
                {
                    // Create new array with draught at i removed
                    List<double> shortenedDraughts = draughts.ToList();
                    shortenedDraughts.RemoveAt(i);
                    semisubDraught.ComputePlane(shortenedDraughts.ToArray());
                    // Check against measured draught
                    if ((semisubDraught.draught - draughts[j]) > 0.05)
                    {
                        // Flag warning
                        MessageBox.Show($"At shift {i} the measured draught {semisubDraught.draught} minus the expected draught {draughts[j]} is greater than 0.05.");
                    }
                }
            }
        }

        public void CheckDisplacementChange()
        {
            // Loop through vessel conditions and check displacements
            for(int i = 1; i < vesselConditions.Count; i++)
            {
                if(Math.Abs((vesselConditions[i].Displacement - vesselConditions[i - 1].Displacement)) > vesselConditions[i].TCP)
                {
                    // Flag warning
                    MessageBox.Show($"At shift {i} the difference in displacements is greater than TCP");
                }
            }
        }
    }
}
