using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PMS.UserManage.Domain.Entities
{
    public class TalentDetail : TalentEntity
    {
        public string nickname { get; set; }

        public string headImgUrl { get; set; }

        public string staffname { get; set; }
    }
}
