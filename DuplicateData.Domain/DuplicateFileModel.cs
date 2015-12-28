using System;
using System.IO;

namespace DuplicateData.Domain
{
    public class DuplicateFileModel
    {
        #region Private Fields
        private static volatile DuplicateFileModel _instance;
        private static object _syncRoot = new Object();
        private const string InputFileFolderName = "Input";
        private const string OutputFileFolderName = "Output"; 
        #endregion

        #region .Ctor
        private DuplicateFileModel()
        {
            CreateOrUpdateDirectories();
        } 
        #endregion

        #region  Private Methods
        private void CreateOrUpdateDirectories()
        {
            string path = Directory.GetCurrentDirectory();
            string inputDirectory = Path.Combine(path, InputFileFolderName);
            if (!Directory.Exists(inputDirectory))
            {
                Directory.CreateDirectory(inputDirectory);
            }

            string outputDirectory = Path.Combine(path, OutputFileFolderName);
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }


        } 
        #endregion

        #region Public Properties
        public static DuplicateFileModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new DuplicateFileModel();
                    }
                }

                return _instance;
            }
        }

        public string InputDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), InputFileFolderName);
            }
        }

        public string OutputDirectory
        {
            get
            {
                return Path.Combine(Directory.GetCurrentDirectory(), OutputFileFolderName);
            }
        } 
        #endregion
    }
}
