using System;
namespace PMS.Infrastructure.Domain.Entities
{
    public class MetroInfo
    {
        public Guid MetroId { get; set; }

        public string MetroName { get; set; }

        public int MetroLineId { get; set; }

        public string MetroLineName { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
}
}
