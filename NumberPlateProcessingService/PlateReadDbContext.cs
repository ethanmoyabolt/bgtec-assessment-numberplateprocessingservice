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
        public DbSet<PlateRead> PlateReads { get; set; }

        public PlateReadDbContext(DbContextOptions<PlateReadDbContext> options) : base(options) { }
    }
}
