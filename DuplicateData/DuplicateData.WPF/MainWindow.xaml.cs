using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;
using DuplicateData.Domain;

namespace DuplicateData.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Fields
        private Microsoft.Win32.OpenFileDialog _dlg;
        private int _accuracy; 
        #endregion

        #region .Ctor
        public MainWindow()
        {
            InitializeComponent();
        } 
        #endregion

        #region Private Events
        private void btnBrowsFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            _dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            _dlg.DefaultExt = ".txt";
            _dlg.Filter = "Text files (*.txt)|*.txt";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = _dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = _dlg.FileName;
                lblFileNmae.Content = filename;
            }

            UpdatebtnRunProcess();
        }

        private void btnRunProcess_Click(object sender, RoutedEventArgs e)
        {
            //UI manipulations
            stkProgress.Visibility = Visibility.Visible;
            grpOutput.Visibility = Visibility.Visible;
            btnRunProcess.IsEnabled = false;
            grpOutput.IsEnabled = false;
            grpInput.IsEnabled = false;

            //Run the "dublicate finding" proceess as BackgroundWorker/NewThread, 
            //otherwise the "dublicate finding" process  will lock the main thead and stuck the Form/UI
            BackgroundWorker worker = new BackgroundWorker { WorkerReportsProgress = true };
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            //open output file
            Process.Start(lblOutputFile.Content.ToString());
        }

        private void cmbAccuracy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get accuracy number from drobdown 
            _accuracy = int.Parse((cmbAccuracy.SelectedItem as ComboBoxItem).Content.ToString());

            UpdatebtnRunProcess();
        }
        
        private void UpdatebtnRunProcess()
        {
            btnRunProcess.IsEnabled = false;
            //if file and Accuracy have been selected then enable RunProgress button 
            if (_dlg != null && !string.IsNullOrWhiteSpace(_dlg.FileName) && cmbAccuracy.SelectedIndex != -1)
            {
                btnRunProcess.IsEnabled = true;
            }
        }
        #endregion

        #region  Worker
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //get selected file name with extension
            string filename = System.IO.Path.GetFileName(_dlg.FileName);

            //instantiate DuplicateFileModel
            DuplicateFileModel dubFileModel = DuplicateFileModel.Instance;
            string inputfilename = System.IO.Path.Combine(dubFileModel.InputDirectory, filename);

            //If selected file name exist in InputFile Folder, then delete
            if (File.Exists(inputfilename))
            {
                File.Delete(inputfilename);
            }

            //Copy selected file into InputFile folder
            File.Copy(_dlg.FileName, inputfilename);


            string outputfilename = System.IO.Path.Combine(dubFileModel.OutputDirectory, string.Format("output_{0}", filename));

            //Update UI with new thread
            //because we are changing from another thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                lblOutputFile.Content = outputfilename;
            });


            //Create file process object 
            FileProcess proces = new FileProcess(inputfilename, outputfilename, _accuracy, (sender as BackgroundWorker));
            //Run the process
            proces.DuplicateProcess();

            //Update UI with new thread
            //because we are changing from another thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                grpOutput.IsEnabled = true;
                btnRunProcess.IsEnabled = true;
                grpInput.IsEnabled = true;
            });


        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //Update Progress Bar
            prgBar.Value = e.ProgressPercentage;
        } 
        #endregion

    }
}

