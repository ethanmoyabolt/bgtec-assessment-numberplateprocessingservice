using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberPlateProcessingService
{
    public class PlateReadDbContext : DbContext
    {
        // Context which allows the service to manage and track instances of file reads and add them to the database

        public DbSet<PlateRead> PlateReads { get; set; }

        public PlateReadDbContext(DbContextOptions<PlateReadDbContext> options) : base(options) { }
    }
}
