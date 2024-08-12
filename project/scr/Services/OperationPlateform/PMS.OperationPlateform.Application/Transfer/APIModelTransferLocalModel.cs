using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Application.Transfer
{
    using Domain.DTOs;

    /// <summary>
    /// API模型接口转换为本地模型
    /// </summary>
    public class APIModelTransferLocalModel
    {




        /// <summary>
        /// 将iSchool.Data的Tag模型转换为本地的Tag模型
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static TagDto iSchoolDataTagToLocalTag(iSchool.Data.API.Model.Tag tag)
        {
            if (tag == null)
            {
                return null;
            }
            else
            {
                return new TagDto()
                {
                    Id = tag.Id.ToString(),
                    TagName = tag.Name

                };
            }


        }
    }

    }
