using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using PMS.OperationPlateform.Domain.Enums;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public partial class article_cover
    {
        public override string ToString()
        {
            return !string.IsNullOrWhiteSpace(this.ImgUrl) ? this.ImgUrl : $"https://cdn.sxkid.com/images/article/{this.articleID}/{this.photoID}.{((FileExtension)this.ext).ToString()}";
        }
    }
}