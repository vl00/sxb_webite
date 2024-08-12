using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtInfoDto
    {
        public Guid Id { get; set; }
	    public Guid SchoolId { get; set; }
	    public int Grade { get; set; }
	    public int Type { get; set; }
	    public int Province { get; set; }
	    public int City { get; set; }
	    public int Area { get; set; }
	    public double Latitude { get; set; }
	    public double Longitude { get; set; }
    }
}
