using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberPlateProcessingService
{
    public class PlateRead
    {
        // PlateRead model to allow new plate reads to be added to the database.

        public Guid Id { get; set; }
        public string CountryOfVehicle { get; set; }
        public string RegNumber { get; set; }
        public int ConfidenceLevel { get; set; }
        public string CameraName { get; set; }
        public DateTime CaptureDate { get; set; }
        public TimeSpan CaptureTime { get; set; }
        public string ImageFilename { get; set; }
    }
}
