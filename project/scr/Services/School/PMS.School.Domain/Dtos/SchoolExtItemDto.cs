using iSchool;
using PMS.OperationPlateform.Domain.DTOs;
using PMS.School.Domain.Common;
using PMS.School.Domain.Enum;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PMS.School.Domain.Dtos
{
    public class SchoolExtItemDto
    {

        public Guid Sid { get; set; }

        public Guid ExtId { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 学部简介
        /// </summary>
        public string ExtIntro { get; set; }

        /// <summary>
        /// 学部简介
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// 学费
        /// </summary>
        public double? Tuition { get; set; }
        /// <summary>
        /// 学生人数
        /// </summary>
        public int? StudentCount { get; set; }
        /// <summary>
        /// 教师人数
        /// </summary>
        public int? TeacherCount { get; set; }
        /// <summary>
        /// 建校时间
        /// </summary>
        public DateTimeOffset? CreationDate { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }


        public string Province { get; set; }
        public string City { get; set; }
        public string Area { get; set; }


        public int ProvinceCode { get; set; }
        public int CityCode { get; set; }
        public int AreaCode { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public double? Score { get; set; }

        /// <summary>
        /// 评分评级
        /// </summary>
        public string ScoreLevel => Score == null ? null : (Score >= 90 ? "A+" : (Score >= 80 && Score < 90 ? "A" : (Score >= 70 && Score < 80 ? "B" : (Score >= 60 && Score < 70 ? "C" : "D"))));

        /// <summary>
        /// 评论
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 评论条数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 评论的序号
        /// </summary>
        public string CommontNo { get; set; }
        /// <summary>
        /// 评论的id
        /// </summary>
        public Guid? CommontId { get; set; }
        /// <summary>
        /// taglist
        /// </summary>
        public List<string> Tags { get; set; }

        public int Grade { get; set; }
        public byte Type { get; set; }

        public bool Discount { get; set; }
        public bool Diglossia { get; set; }
        public bool Chinese { get; set; }

        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }

        public List<H5SchoolRankInfoDto> Ranks;
        /// <summary>
        /// 学校认证
        /// </summary>
        public string Authentication { get; set; }
        /// <summary>
        /// 特色项目
        /// </summary>
        public string Characteristic { get; set; }
        /// <summary>
        /// 课程设置
        /// </summary>
        public string Courses { get; set; }

        /// <summary>
        /// 分部是否有效
        /// </summary>
        public bool ExtValid { get; set; }

        /// <summary>
        /// 学校是否有效
        /// </summary>
        public bool SchoolValid { get; set; }
        /// <summary>
        /// 基础设施
        /// </summary>
        public string Hardware { get; set; }
        /// <summary>
        /// 学校状态
        /// </summary>
        public SchoolStatus Status { get; set; }

        private bool selected;
        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = false;
            }
        }

        /// <summary>
        /// 学校自编代码
        /// </summary>
        public int SchoolNo { get; set; }
        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
            }
        }
    }

    public class SchoolCollectionDto
    {
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }
        public byte Grade { get; set; }
        public byte Type { get; set; }
        public string SchoolName { get; set; }
        public string ExtName { get; set; }
        public double? Tuition { get; set; }
        /// <summary>
        /// 是否普惠
        /// </summary>
        public bool? Discount { get; set; }
        /// <summary>
        /// 是否双语
        /// </summary>
        public bool? Diglossia { get; set; }
        /// <summary>
        /// 是否是中国人学校
        /// </summary>
        public bool? Chinese { get; set; }
        public List<H5SchoolRankInfoDto> Ranks { get; set; }
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// 评论条数
        /// </summary>
        public int CommentCount { get; set; }
        /// <summary>
        /// 评论的id
        /// </summary>
        public Guid? CommontId { get; set; }
        /// <summary>
        /// 评论的序号
        /// </summary>
        public string CommontNo { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public double? Score { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        public List<string> Tags { get; set; }

        public string CityName { get; set; }
        public string AreaName { get; set; }
        public string TypeName { get; set; }
        public int? City { get; set; }
        public int? Area { get; set; }
        public int? Province { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }


    }

    public class SchoolExtConut
    {
        public int Count { get; set; }
    }

    public class SchoolExtRange
    {
        public string Range { get; set; }
    }

    public class SchoolExtListDto
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public bool IsRecommend { get; set; } = false;

        public List<SchoolExtItemDto> List { get; set; }

    }

    public class Tags
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public Guid DataId { get; set; }
    }


    public class SchoolExtSimpleDto
    {
        public string Logo { get; set; }
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }

        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }

        public string CityName { get; set; }
        public string AreaName { get; set; }

        public string Name { get; set; }
        public string EName { get; set; }
        public string SchoolName { get; set; }
        public string ExtName { get; set; }
        public byte Grade { get; set; }
        public byte Type { get; set; }
        /// <summary>
        /// 是否普惠
        /// </summary>
        public bool? Discount { get; set; }
        /// <summary>
        /// 是否双语
        /// </summary>
        public bool? Diglossia { get; set; }
        /// <summary>
        /// 是否是中国人学校
        /// </summary>
        public bool? Chinese { get; set; }

        public SchFType0 SchFType { get; set; }

        /// <summary>
        /// 学费
        /// </summary>
        public double? Tuition { get; set; }
        public double? Applicationfee { get; set; }
        public List<KeyValueDto<double>> Otherfee { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public int? ExtPoint { get; set; }
        /// <summary>
        /// 点评数量
        /// </summary>
        public int? Comment { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        /// <summary>
        /// 学生人数
        /// </summary>
        public int? Studentcount { get; set; }
        /// <summary>
        /// 教师人数
        /// </summary>
        public int? Teachercount { get; set; }
        /// <summary>
        /// 师生比例
        /// </summary>
        public float? TsPercent { get; set; }
        /// <summary>
        /// 外教比例
        /// </summary>
        public double? ForeignTea { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        public List<KeyValueDto<string>> Abroad { get; set; }
        /// <summary>
        /// 有无饭堂
        /// </summary>
        public bool? Canteen { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public string CounterPart { get; set; }
        /// <summary>
        /// 课后管理
        /// </summary>
        public string AfterClass { get; set; }
        /// <summary>
        /// 学部标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 升学成绩的年份
        /// </summary>
        public int? AchYear { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public List<KeyValueDto<Guid, string, double, int>> Achievement { get; set; }
        //排行榜
        public List<H5SchoolRankInfoDto> Ranks { get; set; }
        public byte? Age { get; set; }
        public byte? MaxAge { get; set; }
        /// <summary>
        /// 招生对象
        /// </summary>
        public List<KeyValueDto<string>> Target { get; set; }
        /// <summary>
        /// 招生比例
        /// </summary>
        public float? Proportion { get; set; }
        /// <summary>
        /// 录取分数线
        /// </summary>
        public List<KeyValueDto<string>> Point { get; set; }

        /// <summary>
        /// 招生人数
        /// </summary>
        public int? Count { get; set; }

        /// <summary>
        /// 课程设置
        /// </summary>
        public List<KeyValueDto<string>> Courses { get; set; }
        public List<KeyValueDto<string>> CourseAuthentication { get; set; }
        /// <summary>
        /// 课程特色
        /// </summary>
        public string CourseCharacteristic { get; set; }
        /// <summary>
        /// 学校专访视频
        /// </summary>
        public List<KeyValueDto<DateTime, string, byte>> InterviewVideos { get; set; }
        //精度
        public double? Longitude { get; set; }
        //维度
        public double? Latitude { get; set; }
        /// <summary>
        /// 划片范围
        /// </summary>
        public string Range { get; set; }

        public int SchoolNo { get; set; }
        public string ShortSchoolNo
        {
            get
            {
                return UrlShortIdUtil.Long2Base32(SchoolNo).ToLower();
            }
        }

        /// <summary>
        /// 学校图册
        /// </summary>
        public List<KeyValueDto<string, string, string>> SchoolPhotos { get; set; }


    }

    public class SchoolExtDto
    {
        /// <summary>
        /// 学校logo
        /// </summary>
        public string Logo { get; set; }
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }
        public long SchoolNo { get; set; }
        public string Intro { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public double? Distance { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 学校名字
        /// </summary>
        public string SchoolName { get; set; }
        /// <summary>
        /// 分部名字
        /// </summary>
        public string ExtName { get; set; }
        public string EName { get; set; }
        //enum SchoolGrade
        public byte Grade { get; set; }
        //enum SchoolType
        public byte Type { get; set; }

        public string TypeName
        {
            get
            {
                switch (Type)
                {
                    case 1: return "公办";
                    case 2: return "民办";
                    case 3: return "国际";
                    default:
                        return "未知";
                }
            }
        }
        public bool? Discount { get; set; }
        public bool? Diglossia { get; set; }
        public bool? Chinese { get; set; }

        public SchFType0 SchFType { get; set; }
        /// <summary>
        /// 学生人数
        /// </summary>
        public int? Studentcount { get; set; }
        /// <summary>
        /// 教师人数
        /// </summary>
        public int? Teachercount { get; set; }
        /// <summary>
        /// 师生比例
        /// </summary>
        public float? TsPercent { get; set; }


        public int Province { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }

        public object Select(Func<object, SchoolExtDto> p)
        {
            throw new NotImplementedException();
        }

        public double? Tuition { get; set; }
        public double? Applicationfee { get; set; }
        public string Otherfee { get; set; }
        public string Tel { get; set; }
        public string WebSite { get; set; }
        public string Address { get; set; }
        /// <summary>
        /// 是否寄宿
        /// </summary>
        public bool? Lodging { get; set; }
        /// <summary>
        /// 是否走读
        /// </summary>
        public bool? Sdextern { get; set; }
        /// <summary>
        /// 有无饭堂
        /// </summary>
        public bool? Canteen { get; set; }
        /// <summary>
        ///伙食情况
        /// </summary>
        public string Meal { get; set; }
        /// <summary>
        /// 特色课程
        /// </summary>
        public string Characteristic { get; set; }
        /// <summary>
        /// 学校认证
        /// </summary>
        public string Authentication { get; set; }
        /// <summary>
        /// 外教比例
        /// </summary>
        public float? ForeignTea { get; set; }
        /// <summary>
        /// 出国方向
        /// </summary>
        public string Abroad { get; set; }
        /// <summary>
        /// 开放日
        /// </summary>
        public string OpenHours { get; set; }
        /// <summary>
        /// 行事日历
        /// </summary>
        public string Calendar { get; set; }
        /// <summary>
        /// 划片区域
        /// </summary>
        public string Range { get; set; }
        /// <summary>
        /// 课后管理
        /// </summary>
        public string AfterClass { get; set; }
        /// <summary>
        /// 对口学校
        /// </summary>
        public string CounterPart { get; set; }

        /// <summary>
        /// 总评分
        /// </summary>
        public int? ExtPoint { get; set; }
        /// <summary>
        /// 点评数量
        /// </summary>
        public int? Comment { get; set; }

        /// <summary>
        /// taglist
        /// </summary>
        public List<string> Tags { get; set; }


        /// <summary>
        /// 升学成绩的年份
        /// </summary>
        public int? AchYear { get; set; }
        /// <summary>
        /// 升学成绩
        /// </summary>
        public List<KeyValueDto<Guid, string, double, int>> Achievement { get; set; }

        public byte? Age { get; set; }
        public byte? MaxAge { get; set; }
        /// <summary>
        /// 招生对象
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 招生比例
        /// </summary>
        public float? Proportion { get; set; }
        /// <summary>
        /// 录取分数线
        /// </summary>
        public string Point { get; set; }
        /// <summary>
        /// 招生人数
        /// </summary>
        public int? Count { get; set; }
        /// <summary>
        /// 课程设置
        /// </summary>
        public string Courses { get; set; }
        public string CourseAuthentication { get; set; }
        /// <summary>
        /// 课程特色
        /// </summary>
        public string CourseCharacteristic { get; set; }

        /// <summary>
        /// 线上体验课程
        /// </summary>
        public List<KeyValueDto<DateTime, string, byte>> ExperienceVideos { get; set; }
        /// <summary>
        /// 学校专访课程
        /// </summary>
        public List<KeyValueDto<DateTime, string, byte>> InterviewVideos { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Code { get; set; }
        /// <summary>
        /// 寄宿类型
        /// </summary>
        public LodgingEnum LodgingType => LodgingUtil.Reason(Lodging, Sdextern);
        //学部信息 
        //public List<>

        /// <summary>
        /// 是否有校车
        /// </summary>
        public bool HasSchoolBus { get; set; }
        /// <summary>
        /// 学校学制
        /// </summary>
        public EduSysType? EduSysType { get; set; }

        public string EduSysTypeName
        {
            get
            {

                return EduSysType?.GetDescription();
            }
        }

        public string GradeName
        {
            get
            {
                if (Grade == 1) return "幼儿园";
                if (Grade == 2) return "小学";
                if (Grade == 3) return "初中";
                if (Grade == 4) return "高中";
                return "";
            }
        }
    }

    public class SchoolExtAnyDto : SchoolExtDto
    {

        public SchoolStatus Status { get; set; }

        public bool IsValid { get; set; }
    }
    public class SchoolExtensionDto : SchoolExtDto
    {
        /// <summary>
        /// 学校图片列表
        /// </summary>
        public List<SchoolImageDto> SchoolImages { get; set; }

        public List<SchoolImageDto> PrincipalImages => SchoolImages.Where(s => s.Type == SchoolImageType.Principal).ToList();
        public List<SchoolImageDto> TeacherImages => SchoolImages.Where(s => s.Type == SchoolImageType.Teacher).ToList();
        public List<SchoolImageDto> HardwareImages => SchoolImages.Where(s => s.Type == SchoolImageType.Hardware).ToList();
        public List<SchoolImageDto> StudentImages => SchoolImages.Where(s => s.Type == SchoolImageType.CommunityActivities).ToList();

        /// <summary>
        /// 学校首张图片
        /// </summary>
        public string SchoolImageUrl { get; set; }

        /// <summary>
        /// 考试科目
        /// </summary>
        public string Subjects { get; set; }

        /// <summary>
        /// 学费/年
        /// </summary>
        public string TuitionPerYearFee { get; set; }

        /// <summary>
        /// 考试科目 key列表
        /// </summary>
        public List<KeyValuePair<string, string>> SubjectsList => CommonHelper.TryJsonDeserializeObject(Subjects, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 课程设置 key列表  ["幼儿启蒙教育", "幼儿启蒙教育"]
        /// [{"Key":"幼儿启蒙教育","Value":"14acb08d-575e-4171-ac00-8db70bbe59b4"}]
        /// </summary>
        public List<KeyValuePair<string, string>> CoursesList => CommonHelper.TryJsonDeserializeObject(Courses, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 学校认证 key列表  ["省级示范性", "省级示范性"]
        /// [{"Key":"省级示范性","Value":"7140698c-18b7-4320-9ed8-529c0e18db0f"}]
        /// </summary>
        public List<KeyValuePair<string, string>> AuthenticationList => CommonHelper.TryJsonDeserializeObject(Authentication, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 招生对象 key列表  ["本市学籍", "本市居住证"]
        /// [{"Key":"本市学籍","Value":"fe60cbd6-b3cb-4d43-a668-ba9602bee321"},{"Key":"本市户籍","Value":"6e6b2cde-802b-4e2e-be03-a2333269fe84"},{ "Key":"本市居住证","Value":"bdfba0f8-0abe-4fcd-93e7-5a9f1acee6fe"}]
        /// </summary>
        public List<KeyValuePair<string, string>> TargetList => CommonHelper.TryJsonDeserializeObject(Target, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 出国方向 key列表
        /// </summary>
        public List<KeyValuePair<string, string>> AbroadList => CommonHelper.TryJsonDeserializeObject(Abroad, new List<KeyValuePair<string, string>>());

        /// <summary>
        /// 招生方式[Json 字符串数组结构]
        /// </summary>
        public string RecruitWay { get; set; }


        /// <summary>
        /// 对RecruitWay中的Json串解析为列表
        /// </summary>
        public List<string> RecruitWays
        {
            get
            {
                List<string> defatultRecruitWays = new List<string>();
                if (string.IsNullOrEmpty(RecruitWay))
                {
                    return defatultRecruitWays;
                }
                try
                {
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(this.RecruitWay);
                }
                catch {
                    return defatultRecruitWays;
                }

            }
        }

        /// <summary>
        /// 学校公众号名称
        /// </summary>
        public string OAName { get; set; }

        /// <summary>
        /// 学校公众号AppID
        /// </summary>
        public string OAAppID { get; set; }

        /// <summary>
        /// 学校公众号账号
        /// </summary>
        public string OAAccount { get; set; }
        /// <summary>
        /// 学校小程序名称
        /// </summary>
        public string MPName { get; set; }

        /// <summary>
        /// 学校小程序AppID
        /// </summary>
        public string MPAppID { get; set; }

        /// <summary>
        /// 学校小程序账号
        /// </summary>
        public string MPAccount { get; set; }
        /// <summary>
        /// 学校视频号名称
        /// </summary>
        public string VAName { get; set; }

        /// <summary>
        /// 学校视频号AppID
        /// </summary>
        public string VAAppID { get; set; }

        /// <summary>
        /// 学校视频号账号
        /// </summary>
        public string VAAccount { get; set; }

    }

    public class SchoolExtensionAnyDto : SchoolExtensionDto {
        public SchoolStatus Status { get; set; }

        public bool IsValid { get; set; }
    }
    public class SchoolPKDto
    {
        public SchoolExtListDto recommendSchools { get; set; }
        public HistoryPKShoolsDto historySchools { get; set; }
        public CollectPKSchoolsDto CollectSchools { get; set; }
    }

    public class HistoryPKShoolsDto
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }
        public List<SchoolExtSimpleDto> historySchoolList { get; set; }
    }

    public class CollectPKSchoolsDto
    {
        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }
        public List<SchoolExtDto> CollectSchoolList { get; set; }
    }

    public class PKSchools
    {
        public List<SchoolExtDto> PKSchoolList { get; set; }
    }

    public class PKSchool
    {
        public Guid Sid { get; set; }
        public Guid ExtId { get; set; }
    }

    /// <summary>
    /// 当前位置与学校差距
    /// </summary>
    public class DisparityDto
    {
        public Guid ExtId { get; set; }
        public double? Distance { get; set; }
    }

    /// <summary>
    /// 学校相关图片
    /// </summary>
    public class SchoolImageDto
    {
        public string Url { get; set; }
        public string SUrl { get; set; }
        public string ImageDesc { get; set; }
        public SchoolImageType Type { get; set; }
        public int Sort { get; set; }
    }
}

