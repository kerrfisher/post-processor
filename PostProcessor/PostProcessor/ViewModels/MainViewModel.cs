﻿using PostProcessor.Models;
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
        public string Filename { get; set; }

        public ObservableCollection<TankData> TankData = GetTankData();
        public ObservableCollection<MotionsData> MotionsData = GetMotionsData();
        public ObservableCollection<GeneralData> GeneralData = GetGeneralData();

        public ObservableCollection<TankData> Tanks = new ObservableCollection<TankData>();

        private static ObservableCollection<GeneralData> GetGeneralData()
        {
            GeneralData row1 = new GeneralData(0.1, 0.2, 0.3, 0.4, 15, 0.5);
            GeneralData row2 = new GeneralData(0.1, 0.2, 0.3, 0.4, 15, 0.5);

            return new ObservableCollection<GeneralData> { row1, row2 };
        }

        private static ObservableCollection<MotionsData> GetMotionsData()
        {
            MotionsData row1 = new MotionsData(new LinearData(0.2, 0.4, "7.8,9"), new LinearData(0.3, 0.8, "3.9,4.5"));
            MotionsData row2 = new MotionsData(new LinearData(1.2, 1.4, "17.8,19"), new LinearData(1.3, 1.8, "13.9,14.5"));

            ObservableCollection<MotionsData> motionsData = new ObservableCollection<MotionsData>();
            motionsData.Add(row1);
            motionsData.Add(row2);

            return motionsData;
        }

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

                for(int i = 0; i < 1064; i++)
                {
                    reader.ReadLine();
                }

                string line = reader.ReadLine();
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
                string meanHeelAngle = line.Substring(line.IndexOf(':')).Split(',')[1];
                string meanPitchAngle = line.Substring(line.IndexOf(':')).Split(',')[1].Trim();

                reader.Close();
            }
        }

        private void GetFileDate(string line)
        {
            FileDate = line;
        }

        
    }
}
