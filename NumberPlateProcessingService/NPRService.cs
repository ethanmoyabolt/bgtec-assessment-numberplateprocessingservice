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
            ProcessFile(e.FullPath);
        }

        protected override void OnStop()
        {
            _watcher.Dispose();
        }

        private void ProcessFile(string filePath)
        {
            // Clean up data for processing
            string[] fields = Regex.Split(filePath, @"\\");
            if (fields.Length != 6)
            {
                LogActions("Failed to log file in database, file was not in required format");
            }
            else
            {
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
                // We check to see that there are no cameras with the same filename and camera
                if (!context.PlateReads.Any(pr => pr.ImageFilename == plateRead.ImageFilename)
                    && !context.PlateReads.Any(pr => pr.CameraName == plateRead.CameraName))
                {
                    context.PlateReads.Add(plateRead);
                    context.SaveChanges();
                    LogActions($"File {filePath} has been logged in the database");
                }
                else
                {
                    LogActions($"File {filePath} is already present in the database");
                }
            }

        }

        // Log Actions in log file
        private void LogActions(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date;
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
        }
    }
}
