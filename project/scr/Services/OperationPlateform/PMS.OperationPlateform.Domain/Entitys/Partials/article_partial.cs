using System;
using System.Collections.Generic;
using System.Text;
using Dapper.Contrib.Extensions;

namespace PMS.OperationPlateform.Domain.Entitys
{
    public partial class article
    {
        /// <summary>
        /// �����ı���ͼƬ
        /// </summary>
        [Write(false)]
        public List<article_cover> Covers { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Write(false)]
        public int CommentCount { get; set; }

        [Write(false)]
        public Enums.ArticleType ArticleType
        {
            get
            {
                if (int.TryParse(this.type, out int type))
                {
                    return (Enums.ArticleType)type;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// ������Ķ���
        /// </summary>
        [Write(false)]
        public int VirualViewCount
        {
            get
            {
                return this.viewCount.GetValueOrDefault() + this.viewCount_r.GetValueOrDefault();
            }
        }
    }
}