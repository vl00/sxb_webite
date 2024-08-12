using PMS.OperationPlateform.Domain.Entitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.OperationPlateform.Domain.DTOs
{
    public class ToolModuleInfoDto
    {
        public ToolTypes FirstModule { get; set; }

        public List<ToolTypes> SecondModule { get; set; }



    }


}
