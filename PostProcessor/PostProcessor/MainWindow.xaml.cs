using Microsoft.Win32;
using PostProcessor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PostProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            this.mainViewModel = mainViewModel;

            dataGrid1.DataContext = mainViewModel.MotionsData;
            dataGrid2.DataContext = mainViewModel.GeneralData;
            dataGrid3.DataContext = mainViewModel.TankFromData;
            dataGrid4.DataContext = mainViewModel.TankToData;
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            // Create open file dialog
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Set filename label
                Uri filepathUri = new Uri(dlg.FileName);
                if (filepathUri.IsFile)
                {
                    lblFilename.Content = System.IO.Path.GetFileName(filepathUri.LocalPath);
                    mainViewModel.ReadFileContents(dlg.FileName);
                    mainViewModel.CheckConsistencies();
                }
            }
        }
    }
}
