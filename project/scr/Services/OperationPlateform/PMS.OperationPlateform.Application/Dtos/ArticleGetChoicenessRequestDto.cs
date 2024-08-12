using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Application.Dtos
{
    public class ArticleGetChoicenessRequestDto
    {
        public Guid UserId { get; set; }

        public string UUID { get; set; }

        public int CityCode { get; set; }
    }
}
