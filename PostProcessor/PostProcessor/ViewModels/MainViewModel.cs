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
            GeneralData row1 = new GeneralData(0.1, 0.2, 0.3, 0.4, 15, 0.5);
            GeneralData row2 = new GeneralData(0.1, 0.2, 0.3, 0.4, 15, 0.5);

            return new ObservableCollection<GeneralData> { row1, row2 };
        }

        // Dummy data for testing
        private static ObservableCollection<MotionsData> GetMotionsData()
        {
            MotionsData row1 = new MotionsData(new LinearData(0.2, 0.4, "7.8,9"), new LinearData(0.3, 0.8, "3.9,4.5"));
            MotionsData row2 = new MotionsData(new LinearData(1.2, 1.4, "17.8,19"), new LinearData(1.3, 1.8, "13.9,14.5"));

            ObservableCollection<MotionsData> motionsData = new ObservableCollection<MotionsData>();
            motionsData.Add(row1);
            motionsData.Add(row2);

            return motionsData;
        }

        // Dummy data for testing
        private static ObservableCollection<TankData> GetTankData()
        {
            TankData row1 = new TankData("SWB4P", 4.5, 3.2, 0.2, 45);
            TankData row2 = new TankData("SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row3 = new TankData("SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row4 = new TankData("SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row5 = new TankData("SWB4S", 6.7, 2.1, 0.4, 90);
            TankData row6= new TankData("SWB4S", 6.7, 2.1, 0.4, 90);

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
                GetFileDate(reader.ReadLine());

                for(int i = 0; i < 1063; i++)
                {
                    reader.ReadLine();
                }

                string line = reader.ReadLine();
                string numRecord = line.Substring(line.IndexOf(':') + 1).Trim();

                line = reader.ReadLine();
                string draught = line.Substring(line.IndexOf(':') + 1).Trim();

                line = reader.ReadLine();
                string displacement = line.Substring(line.IndexOf(':') + 1).Trim();

                reader.ReadLine();

                line = reader.ReadLine();
                string tankFrom = line.Substring(0, line.IndexOf(':'));

                reader.ReadLine();

                line = reader.ReadLine();
                string tankTo = line.Substring(0, line.IndexOf(':'));

                // 1067
                for (int i = 0; i < 1028; i++)
                {
                    reader.ReadLine();
                }

                line = reader.ReadLine();
                string meanHeelAngle = line.Substring(line.IndexOf(':') + 1).Split(',')[0].Trim();
                string meanPitchAngle = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

                string heelPlot = numRecord + "," + meanHeelAngle;
                string pitchPlot = numRecord + "," + meanPitchAngle;

                line = reader.ReadLine();
                string heelSlope = line.Substring(line.IndexOf(':') + 1).Split(',')[0].Trim();
                string pitchSlope = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

                line = reader.ReadLine();
                string heelStdDev = line.Substring(line.IndexOf(':') + 1).Split(',')[0].Trim();
                string pitchStdDev = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

                MotionsData.Add(new MotionsData(new LinearData(Convert.ToDouble(heelSlope), Convert.ToDouble(heelStdDev), heelPlot), new LinearData(Convert.ToDouble(pitchSlope), Convert.ToDouble(pitchStdDev), pitchPlot)));

                line = reader.ReadLine();
                string[] tankValues = line.Substring(line.IndexOf(':') + 1).Split(',');
                // string tankName, double lcg, double tcg, double level, double weight
                TankData stbdTank = new TankData(tankFrom, Convert.ToDouble(tankValues[0].Trim()), Convert.ToDouble(tankValues[1].Trim()), Convert.ToDouble(tankValues[2].Trim()), Convert.ToDouble(tankValues[3].Trim()));
                TankFromData.Add(stbdTank);

                line = reader.ReadLine();
                tankValues = line.Substring(line.IndexOf(':') + 1).Split(',');
                TankData portTank = new TankData(tankTo, Convert.ToDouble(tankValues[0].Trim()), Convert.ToDouble(tankValues[1].Trim()), Convert.ToDouble(tankValues[2].Trim()), Convert.ToDouble(tankValues[3].Trim()));
                TankToData.Add(portTank);

                double transMoment = new double[]{ stbdTank.Weight, portTank.Weight }.Average() * new double[] { stbdTank.TCG, portTank.TCG }.Average();
                double longMoment = new double[] { stbdTank.Weight, portTank.Weight }.Average() * new double[] { stbdTank.LCG, portTank.LCG }.Average();

                // double transversalMoment, double longitudinalMoment, double averageHeel, double averageTrim, double draught, double correspondingDisplacement
                GeneralData.Add(new GeneralData(transMoment, longMoment, Convert.ToDouble(meanHeelAngle), Convert.ToDouble(meanPitchAngle), Convert.ToDouble(draught), Convert.ToDouble(displacement)));

                reader.Close();
            }
        }

        private void GetFileDate(string line)
        {
            FileDate = line;
        }
    }
}
