using System;
using System.Collections.Generic;
using System.Text;

namespace WeChat.Model
{
    public class UpsertPreUniversityBasicRequest
    {
        /// <summary>
        /// 是否测试环境
        /// </summary>
        public int is_test { get; set; }
        /// <summary>
        /// 学校List
        /// </summary>
        public Pre_University[] pre_university_list { get; set; }

        public class Pre_University
        {
            public Pre_University()
            {
                this.pos_info = new Pos_Info();
                //this.aliases = new List<string>();
                this.hints = new List<string>();
                this.contact_info = new Contact_Info()
                {
                    phones = new List<string>()
                };
                this.recruit_info = new Recruit_Info()
                {
                    recruit_items = new List<dynamic>()
                };
                this.service_info = new Service_Info()
                {
                    service_items = new List<Service_Item>()
                };
                this.tags = new List<string>();
                this.relate_accts = new List<string>();
            }

            public int school_grade { get; set; }
            public string wx_school_id { get; set; }
            public string school_name { get; set; }
            public string official_website { get; set; }
            public List<string> tags { get; set; }
            public Bg_Img_Info bg_img_info { get; set; }
            public string logo_url { get; set; }
            public List<string> hints { get; set; }
            public List<string> aliases { get; set; }
            public List<string> relate_accts { get; set; }
            public Pos_Info pos_info { get; set; }
            public Contact_Info contact_info { get; set; }
            public int status { get; set; }
            public Recruit_Info recruit_info { get; set; }
            public Service_Info service_info { get; set; }
            /// <summary>
            /// 修改原因
            /// </summary>
            public string reason { get; set; }
        }

        public class Bg_Img_Info
        {
            public string img_url { get; set; }
            public int total_cnt { get; set; }
            public string jump_url { get; set; }
            public int jump_type { get; set; }
            public string appid { get; set; }
        }

        public class Pos_Info
        {
            public int[] area_codes { get; set; }
            public string address { get; set; }
            public double longitude { get; set; }
            public double latitude { get; set; }
        }

        public class Contact_Info
        {
            public List<string> phones { get; set; }
        }

        public class Recruit_Info
        {
            public List<dynamic> recruit_items { get; set; }
        }

        public class Recurit_Items
        {
            public RecruitType recruit_type { get; set; }
            public List<string> descs { get; set; }
            public int can_jump { get; set; }
            public string jump_url { get; set; }
            public int jump_type { get; set; }
            public string appid { get; set; }
        }

        public class Service_Info
        {
            public List<Service_Item> service_items { get; set; }
        }

        public class Service_Item
        {
            public SvrType svr_type { get; set; }
            public int jump_type { get; set; }
            public string appid { get; set; }
            public string service_url { get; set; }
        }

        public enum RecruitType
        {

            本区政策 = 1,
            招生计划 = 2,
            招生对象 = 3,
            招生方式 = 4,
        }


        ///
        public enum SvrType
        {
            [SvrTypePermission( City = 0, SchoolGrade = 0)]
            [SvrTypeServiceUrl( SvrTypeParams = "_type=1_ref=1")]
            学校概况 = 1,
            [SvrTypePermission(City = 0, SchoolGrade = 0)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=17")]
            招生简章 = 2,
            [SvrTypePermission(City = 440100, SchoolGrade = 1)]
            [SvrTypePermission(City = 440100, SchoolGrade = 2)]
            [SvrTypePermission(City = 440100, SchoolGrade = 3)]
            [SvrTypePermission(City = 440300, SchoolGrade = 1)]
            [SvrTypePermission(City = 440300, SchoolGrade = 2)]
            [SvrTypePermission(City = 440300, SchoolGrade = 3)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=19")]
            招生范围 = 3,
            [SvrTypePermission(City = 510100, SchoolGrade = 1)]
            [SvrTypePermission(City = 500100, SchoolGrade = 1)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=21")]
            片区范围 = 4,
            [SvrTypePermission(City = 440100, SchoolGrade = 1)]
            [SvrTypePermission(City = 440100, SchoolGrade = 2)]
            [SvrTypePermission(City = 440300, SchoolGrade = 1)]
            [SvrTypePermission(City = 440300, SchoolGrade = 2)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=28")]
            对口地段 = 5,
            [SvrTypePermission(City = 0, SchoolGrade = 1)]
            [SvrTypePermission(City = 0, SchoolGrade = 2)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=24")]
            招生报名时间 = 6,
            [SvrTypePermission(City = 0, SchoolGrade = 1)]
            [SvrTypePermission(City = 0, SchoolGrade = 2)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=23")]
            招生报名材料清单 = 7,
            [SvrTypePermission(City = 500100, SchoolGrade = 2)]
            [SvrTypePermission(City = 500100, SchoolGrade = 3)]
            [SvrTypePermission(City = 510100, SchoolGrade = 2)]
            [SvrTypePermission(City = 510100, SchoolGrade = 3)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=21")]
            划片范围 = 8,
            [SvrTypePermission(City = 0, SchoolGrade = 2)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=3_ref=10")]
            对口初中 = 9,
            [SvrTypePermission(City = 440100, SchoolGrade = 2)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=3_ref=10")]
            小升初派位安排 = 10,
            [SvrTypePermission(City = 0, SchoolGrade = 2)]
            [SvrTypePermission(City = 0, SchoolGrade = 3)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=6")]
            积分入学 = 11,
            [SvrTypePermission(City = 440100, SchoolGrade = 3)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=27")]
            对口小学 = 12,
            [SvrTypePermission(City = 500100, SchoolGrade = 3)]
            [SvrTypePermission(City = 500100, SchoolGrade = 4)]
            [SvrTypePermission(City = 510100, SchoolGrade = 3)]
            [SvrTypePermission(City = 510100, SchoolGrade = 4)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=4_ref=11")]
            指标到校 = 13,
            [SvrTypePermission(City = 440100, SchoolGrade = 3)]
            [SvrTypePermission(City = 440100, SchoolGrade = 4)]
            [SvrTypePermission(City = 440300, SchoolGrade = 3)]
            [SvrTypePermission(City = 440300, SchoolGrade = 4)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=4_ref=11")]
            指标分配 = 14,
            [SvrTypePermission(City = 0, SchoolGrade = 4)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=2_ref=9")]
            自主招生 = 15,
            [SvrTypePermission(City = 0, SchoolGrade = 4)]
            [SvrTypeServiceUrl(SvrTypeParams = "_type=5_ref=15")]
            中考录取分数线 = 16,
            招生计划 = 17,
            开设专业 = 18,
            学院与校区分布 = 19,
            毕业去向 = 20,
            志愿填报分析 = 21,
            招办答疑 = 22
               



        }


        [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
        public class SvrTypePermissionAttribute : Attribute {

            /// <summary>
            /// 440100 广州
            /// 440300 深圳
            /// 510100 四川
            /// 500100 重庆
            /// </summary>
            public int City { get; set; }

            public int SchoolGrade { get; set; }
        }

        [System.AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public class SvrTypeServiceUrlAttribute : Attribute {

            
            public string GetServiceUrl(string nostr )
            {
                return $"https://m.sxkid.com/school/wechat/detail/{nostr}{this.SvrTypeParams}";
            }

            public string SvrTypeParams { get; set; }
        }
    }
}
