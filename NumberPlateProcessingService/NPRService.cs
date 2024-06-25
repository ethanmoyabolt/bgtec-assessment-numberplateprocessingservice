using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NumberPlateProcessingService
{
    public partial class NPRService : ServiceBase
    {
        private FileSystemWatcher _watcher;
        private PlateReadDbContext context { get; set; }

        public NPRService(PlateReadDbContext ctx)
        {
            context = ctx;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // On start check the directory and process all files to check if any have been added while the service has been stopped

            string rootPath = @"C:\Users\ethan\OneDrive\Desktop\ACS Output Test";
            string[] dirs = Directory.GetFiles(rootPath);

            foreach(string filepath in dirs)
            {
                ProcessFile(filepath);
            }

            // Create a watcher to monitor when new files are added and to process them as soon as they are added.

            _watcher = new FileSystemWatcher
            {
                Path = rootPath
            };

            _watcher.Created += new FileSystemEventHandler(OnChanged);

            _watcher.EnableRaisingEvents = true;

        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            ProcessFile(@"NONE\r9112A\r77\rGIBEXIT2\20140827\1210/w27082014,12140198,9112A,77.jpg");
        }

        protected override void OnStop()
        {
            _watcher.Dispose();
        }

        private void ProcessFile(string filePath)
        {
            // Clean up data for processing
            string[] fields = Regex.Split(filePath, @"\\");
            string[] splitFinalTwo = fields[fields.Length - 1].Split('/');

            var plateRead = new PlateRead
            {
                CountryOfVehicle = fields[0],
                RegNumber = fields[1].Substring(1),
                ConfidenceLevel = int.Parse(fields[2].Replace("r", "")),
                CameraName = fields[3].Substring(1),
                CaptureDate = DateTime.ParseExact(fields[4], "yyyyMMdd", null),
                CaptureTime = TimeSpan.ParseExact(splitFinalTwo[0], "hhmm", null),
                ImageFilename = splitFinalTwo[1]
            };

            // Add any new files to the database.
            if (!context.PlateReads.Any(pr => pr.ImageFilename == plateRead.ImageFilename))
            {
                context.PlateReads.Add(plateRead);
                context.SaveChanges();
            }

        }
    }
}
