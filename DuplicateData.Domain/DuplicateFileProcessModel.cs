using System.CodeDom;
using System.ComponentModel;
using System.IO;
using System.Text;


namespace DuplicateData.Domain
{
    /// <summary>
    /// This class is in chatrge processing the input file, 
    /// and generating output file with duplicates  
    /// </summary>
    public class FileProcess
    {
        #region Private Fields
        private string _inpuFile;
        private string _outputFile;
        private int _accuracy;
        private BackgroundWorker _worker;
        private string[] _fileTextList; 
        #endregion

        #region .Ctor
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="inpuFile">Input File Path </param>
        /// <param name="outputFile">Output File Path</param>
        /// <param name="accuracy">accuracy</param>
        /// <param name="worker">BackgroundWorker for Reporting Progress to UI</param>
        public FileProcess(string inpuFile, string outputFile, int accuracy, BackgroundWorker worker)
        {
            _inpuFile = inpuFile;
            _outputFile = outputFile;
            _accuracy = accuracy;
            _worker = worker;
        } 
        #endregion

        #region Private Methods
        /// <summary>
        /// finding duplicates in a List
        /// </summary>
        /// <param name="text">find duplicates for "text"</param>
        /// <param name="index">start computeing starting from index</param>
        /// <returns></returns>
        private string FindTextDuplicates(string text, int index)
        {

            StringBuilder sb = new StringBuilder();
            // Read the file and display it line by line.

            for (int i = index; i < _fileTextList.Length; ++i)
            {
                string line = _fileTextList[i];

                int distance = LevenshteinDistance.Compute(text, line);
                if (distance <= _accuracy)
                {
                    sb.Append(string.Format("\t{0}", line));
                }
            }

            if (sb.Length > 1)
            {
                return string.Format("{0}{1}", text, sb);
            }
            else
            {
                return string.Empty;
            }
        } 
        #endregion

        #region Public Methods
        /// <summary>
        /// Generate output file with duplicates
        /// </summary>
        public void DuplicateProcess()
        {
            if (!string.IsNullOrWhiteSpace(_inpuFile) && File.Exists(_inpuFile))
            {
                int index = 1;

                _fileTextList = File.ReadAllLines(_inpuFile);
                
                //calculate progres step
                double progressStep = 100.0 / _fileTextList.Length;
                
                
                using (StreamWriter writer = new StreamWriter(_outputFile))
                {
                    foreach (string text in _fileTextList)
                    {
                        //find duplicates for "text"
                        var result = FindTextDuplicates(text, index);
                        if (!string.IsNullOrEmpty(result))
                        {
                            //write duplicates
                            writer.WriteLine(result);
                        }

                        index++;

                        //update progress
                        _worker.ReportProgress((int)(index * progressStep));

                    }
                }
            }
        } 
        #endregion


    }
}
