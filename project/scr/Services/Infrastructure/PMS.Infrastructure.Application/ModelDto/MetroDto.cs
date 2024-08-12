using System;
using System.Collections.Generic;

namespace PMS.Infrastructure.Application.ModelDto
{
    public class MetroDto
    {
        public Guid MetroId { get; set; }
        public long MetroNo { get; set; }
        public string MetroName { get; set; }
        public List<MetroLine> MetroStations {get;set;}

        public class MetroLine
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public double Lat { get; set; }
            public double Lng { get; set; }
            public bool active { get; set; } = false;
        }
    }
}
