using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NumberPlateProcessingService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            // Database is connected and context is created and passed to the service on start up.
            DbContextOptionsBuilder<PlateReadDbContext> options = new DbContextOptionsBuilder<PlateReadDbContext>();
            options.UseSqlServer("Server = (localdb)\\mssqllocaldb;Database = ANPR; MultipleActiveResultSets = true;Trusted_Connection=True");
            PlateReadDbContext ctx = new PlateReadDbContext(options.Options);

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new NPRService(ctx)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
