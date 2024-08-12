using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
    using Entitys;
    using Enums;
    public partial class Article_SubscribePreference
    {
        ///// <summary>
        ///// 学校类型
        ///// </summary>
        //[Write(false)]
        //public Article_SchoolTypes SchoolType { get; set; }
        public List<SchoolGrade> SchoolGradeEnums()
        {

                if (string.IsNullOrEmpty(Grades)) {
                    return null;
                }

                var splits = this.Grades?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<SchoolGrade> list = new List<SchoolGrade>();
                if (splits != null)
                {
                    foreach (var item in splits)
                    {
                        if (int.TryParse(item, out int sgInt))
                            list.Add((SchoolGrade)sgInt);
                    }
                }
                return list;

            
        }

        public List<Domain.Enums.SchoolType> SchoolTypeEnums()
        {

                if (string.IsNullOrEmpty(SchoolTypes))
                {
                    return null;
                }

                var splits = this.SchoolTypes?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<Domain.Enums.SchoolType> list = new List<Domain.Enums.SchoolType>();
                if (splits != null)
                {
                    foreach (var item in splits)
                    {
                        if (int.TryParse(item, out int stInt))
                            list.Add((Domain.Enums.SchoolType)stInt);
                    }
                }
                return list;

            
        }




        /// <summary>
        /// 省
        /// </summary>
        [Write(false)]
        public local_v2 Province { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [Write(false)]
        public local_v2 City { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [Write(false)]
        public local_v2 Area { get; set; }

        ///// <summary>
        ///// 学段
        ///// </summary>
        //[Write(false)]
        //public SchoolGrade SchoolStage { get; set; }
    }
}