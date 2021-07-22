using PostProcessor.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostProcessor.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // File name of current MOSIS output
        public string Filename { get; set; }
        
        // Observable objects for DataGrid's
        // Observable objects have notify property handling ingrained
        //public ObservableCollection<MotionsData> MotionsData = GetMotionsData();
        //public ObservableCollection<GeneralData> GeneralData = GetGeneralData();
        //public ObservableCollection<TankData> TankFromData = GetTankData();
        public ObservableCollection<MotionsData> MotionsData = new ObservableCollection<MotionsData>();
        public ObservableCollection<GeneralData> GeneralData = new ObservableCollection<GeneralData>();
        public ObservableCollection<TankData> TankFromData = new ObservableCollection<TankData>();
        public ObservableCollection<TankData> TankToData = new ObservableCollection<TankData>();

        // Dummy data for testing
        private static ObservableCollection<GeneralData> GetGeneralData()
        {
            GeneralData row1 = new GeneralData(1, 0.1, 0.2, 0.3, 0.4, 15, 0.5);
            GeneralData row2 = new GeneralData(2, 0.1, 0.2, 0.3, 0.4, 15, 0.5);

            return new ObservableCollection<GeneralData> { row1, row2 };
        }

        // Dummy data for testing
        private static ObservableCollection<MotionsData> GetMotionsData()
        {
            MotionsData row1 = new MotionsData(1, new LinearData(0.2, 0.4, "7.8,9"), new LinearData(0.3, 0.8, "3.9,4.5"));
            MotionsData row2 = new MotionsData(2, new LinearData(1.2, 1.4, "17.8,19"), new LinearData(1.3, 1.8, "13.9,14.5"));

            ObservableCollection<MotionsData> motionsData = new ObservableCollection<MotionsData>();
            motionsData.Add(row1);
            motionsData.Add(row2);

            return motionsData;
        }

        // Dummy data for testing
        private static ObservableCollection<TankData> GetTankData()
        {
            TankData row1 = new TankData(1, "SWB4P", 4.5, 3.2, 0.2, 45);
            TankData row2 = new TankData(2, "SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row3 = new TankData(3, "SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row4 = new TankData(4, "SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row5 = new TankData(5, "SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row6= new TankData(6, "SWB4S", 6.7, 2.1, 0.4, 90);

            return new ObservableCollection<TankData> { row1, row2, row3, row4, row5, row6 };
        }

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

                // Iterate over calibration run
                for(int i = 0; i < 1063; i++)
                {
                    reader.ReadLine();
                }

                // Line with test number
                string line = reader.ReadLine();
                // Recursive method that will repeat if there are more tests
                ReadShift(reader, line);

                // Close stream
                reader.Close();
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
            string draught = line.Substring(line.IndexOf(':') + 1).Trim();

            // Read displacement at shift
            line = reader.ReadLine();
            string displacement = line.Substring(line.IndexOf(':') + 1).Trim();

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
            GeneralData.Add(new GeneralData(Convert.ToInt32(numRecord), transMoment, longMoment, Convert.ToDouble(meanHeelAngle), Convert.ToDouble(meanPitchAngle), Convert.ToDouble(draught), Convert.ToDouble(displacement)));

            line = reader.ReadLine();
            // Since this might be the end of the file, it might return null
            if(line != null)
            {
                // Check line starts with "Test"
                if (line.Substring(0, 4).Equals("Test"))
                {
                    // Recall method
                    ReadShift(reader, line);
                }
            }            
        }
    }
}
