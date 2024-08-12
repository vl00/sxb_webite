using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Reflection;
namespace PMS.OperationPlateform.Domain.Enums
{

    /// <summary>
    /// 活动签到形式
    /// </summary>
    public enum SignType { 
        [Description("普通签到")]
        BaseSign = 1,
        [Description("现场签到")]
        SceneSign = 2

    }

    /// <summary>
    /// 工具模块类型
    /// </summary>
    public enum ToolModuleType
    {
        [Description("常用")]
        ChangYong = 1,

        [Description("查询")]
        ChaXun = 2
    }

    public enum SendMsgType
    {
        [Description("全部")]
        [DefaultValue("")]
        All = 0,
        [Description("系统发送")]
        [DefaultValue("0")]
        SysSend = 1,
        [Description("用户回复")]
        [DefaultValue("1")]
        UserReply = 2

    }

    public enum Channels
    {

        [Description("上学帮服务号")]
        [DefaultValue("_sxbfwh")]
        SXBFWH = 1,
        [Description("广佛上学帮")]
        [DefaultValue("_gfsxb")]
        GFSXB = 2

    }


    public enum TagBindType
    {

        /// <summary>
        ///  0: v2学校/机构
        /// </summary>
        [Description("v2学校/机构")]
        [DefaultValue(0)]
        V2SchoolOrOrg = 0,

        /// <summary>
        /// 1: 文章
        /// </summary>
        [DefaultValue(1)]
        Article = 1,

        /// <summary>
        /// 2: v3学校/学部
        /// </summary>
        [DefaultValue(2)]
        V3SchoolOrDepartment = 2




    }

    /// <summary>
    /// 上学帮旗下接管的公众号
    /// </summary>
    public enum CityIdentity
    {
        [Description("服务号")]
        [DefaultValue("")]
        fwh = 1,
        [Description("广州上学帮")]
        [DefaultValue("_gz")]
        gz = 2,
        [Description("成都上学帮")]
        [DefaultValue("_cd")]
        cd = 3,
        [Description("深圳上学帮")]
        [DefaultValue("_sz")]
        sz = 4,


    }



    /// <summary>
    /// 学校榜单搜索类型
    /// </summary>
    public enum SchoolRankSearchType
    {
        /// <summary>
        /// 检索ID
        /// </summary>
        [Description("榜单ID")]
        Id = 1,
        /// <summary>
        /// 加索名称
        /// </summary>
        [Description("榜单名称")]
        Name = 2

    }

    /// <summary>
    /// 招生年级
    /// </summary>
    public enum SchoolGrade
    {
        /// <summary>
        /// 幼儿园
        /// </summary>
        [Description("幼儿园")]
        Kindergarten = 1,
        /// <summary>
        /// 小学
        /// </summary>
        [Description("小学")]
        PrimarySchool = 2,
        /// <summary>
        /// 初中
        /// </summary>
        [Description("初中")]
        JuniorMiddleSchool = 3,
        /// <summary>
        /// 高中
        /// </summary>
        [Description("高中")]
        SeniorMiddleSchool = 4,
    }

    /// <summary>
    /// 招生年级(拼音)
    /// </summary>
    public enum SchoolGradePY
    {
        [Description("全部")]
        Unknow = 0,
        /// <summary>
        /// 幼儿园
        /// </summary>
        [Description("幼儿园")]
        YouErYuan = 1,
        /// <summary>
        /// 小学
        /// </summary>
        [Description("小学")]
        XiaoXue = 2,
        /// <summary>
        /// 初中
        /// </summary>
        [Description("初中")]
        ChuZhong = 3,
        /// <summary>
        /// 高中
        /// </summary>
        [Description("高中")]
        GaoZhong = 4,
    }

    /// <summary>
    /// 学校类型 1公办  2民办  3国际 4外籍 80港澳台 99其它
    /// </summary>
    public enum SchoolType
    {
        /// <summary>
        /// 公办
        /// </summary>
        [Description("公办")]
        Public = 1,
        /// <summary>
        /// 民办
        /// </summary>
        [Description("民办")]
        Private = 2,
        /// <summary>
        /// 国际
        /// </summary>
        [Description("国际")]
        International = 3,
        /// <summary>
        /// 外籍
        /// </summary>
        [Description("外籍")]
        ForeignNationality = 4,
        /// <summary>
        /// 港澳台
        /// </summary>
        [Description("港澳台")]
        SAR = 80,
        /// <summary>
        /// 其它
        /// </summary>
        [Description("其它")]
        Other = 99

    }

    /// <summary>
    /// 文章类型 1 政策性文章 2 数据对比类 3 教育投资类 4 学校介绍类 5 育儿成长类 6 学科备考类 7 热点新闻类
    /// </summary> 
    public enum ArticleType
    {
        [Description("教育政策类")]
        Policy = 1,

        [Description("教育投资类")]
        EduInvestment = 3,

        [Description("学校介绍类")]
        SchoolIntroduction = 4,

        [Description("育儿成长类")]
        ParentingGrowth = 5,

        [Description("数据对比类")]
        Comparision = 2,

        [Description("学科备考类")]
        SubjectPreparation = 6,

        [Description("热点新闻类")]
        HotNews = 7
    }

    /// <summary>
    /// 文章作者类型
    /// </summary>
    public enum ArticleAuthorType
    {
        [Description("非达人")]
        General,

        [Description("达人")]
        Talent
    }

    /// <summary>
    /// 点评类型
    /// </summary>
    public enum CommentType
    {
        优质 = 1,
        普通 = 2
    }

    public enum SMSRecordType
    {
        系统发送,
        用户回复
    }


    public enum ArticleSchoolTypes
    {
        [Description("外国人高中")]
        waiguorengaozhong = 1

    }


    public enum ArticleEntraceStage
    {
        [Description("高考")]
        gaokao = 1,
        [Description("中考")]
        zhongkao = 2,
        [Description("小升初")]
        xiaoshengchu = 3,
        [Description("幼升小")]
        youshengxiao = 4
    }


    /// <summary>
    /// 学校点评人角色
    /// </summary>
    public enum SCMRole
    {
        [Description("校方")]
        SA = 1,
        [Description("普通")]
        GN = 2,


    }


    /// <summary>
    /// 文章列表展现的布局形式
    /// </summary>
    public enum ArticleLayout
    {

        /// <summary>
        /// 无图布局
        /// </summary>
        [Description("//cdn.sxkid.com/images/bg/article/layoutimg/0.png")]
        NoImage = 0,

        /// <summary>
        /// 单图布局
        /// </summary>
        [Description("//cdn.sxkid.com/images/bg/article/layoutimg/1.png")]
        SingleImage = 1,


        /// <summary>
        /// 单大图布局
        /// </summary>
        [Description("//cdn.sxkid.com/images/bg/article/layoutimg/2.png")]
        SingleBigImage = 2,


        /// <summary>
        /// 单视频布局
        /// </summary>
        [Description("//cdn.sxkid.com/images/bg/article/layoutimg/3.png")]
        SingleVideo = 3,

        /// <summary>
        /// 双图布局
        /// </summary>
        [Description("//cdn.sxkid.com/images/bg/article/layoutimg/4.png")]
        DoubleImage = 4,


        /// <summary>
        /// 三图布局
        /// </summary>
        [Description("//cdn.sxkid.com/images/bg/article/layoutimg/5.png")]
        ThreeImage = 5

    }


    public enum SchoolCommentRole
    {


        /// <summary>
        /// 普通
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 达人
        /// </summary>
        Expert = 1,

        /// <summary>
        /// 官方(校方)
        /// </summary>
        Official = 2


    }


    /// <summary>
    /// 文件扩展名称
    /// </summary>
    public enum FileExtension
    {
        jpg,
        png,
        gif,
        mp3,
        mp4,
        amr,
        avi,
        zip,
        rar,
        txt,
        doc,
        docx,
        xls,
        xlsx,
        ppt,
        pptx,
        pdf
    }

    /// <summary>
    /// 留资页用户留资类型
    /// </summary>
    public enum LSRFLeveInfoType
    {
        /// <summary>
        /// 快捷留资
        /// </summary>
        Quick = 1,

        /// <summary>
        /// 普通留资
        /// </summary>
        Ordinary = 2
    }

    /// <summary>
    /// 留资页学校推荐类型
    /// </summary>
    public enum LSRFSchoolType
    {
        /// <summary>
        /// 普通
        /// </summary>
        Ordinary = 1,

        /// <summary>
        /// 推荐
        /// </summary>
        Recommend = 2,

        /// <summary>
        /// 广告
        /// </summary>
        Advertise = 3
    }


    //字段类型：0、文本输入；1、手机号+验证码；2、下拉选择；3、单选；4、多选；5、多行文本；
    public enum SchoolActivityExtensionType
    {
        [Description("文本输入")]
        Text = 0,

        [Description("手机号+验证码")]
        Phone = 1,

        [Description("下拉选择")]
        DropdownList = 2,

        [Description("单选")]
        Radio = 3,

        [Description("多选")]
        CheckBox = 4,

        [Description("多行文本")]
        Textarea = 5
    }


    /// <summary>
    /// 类别0，统一   1;单一
    /// </summary>
    public enum SchoolActivityCategory
    {

        [Description("统一")]
        Common = 0,

        [Description("单一")]
        Single = 1
    }

    /// <summary>
    /// 留资类型，0：长期，1：限时，2：固定
    /// </summary>
    public enum SchoolActivityType
    {
        [Description("长期")]
        Long = 0,
        [Description("限时")]
        Limit = 1,

        //只有统一留资 限制固定
        [Description("固定")]
        Const = 2
    }

    /// <summary>
    /// 状态，0：启用，1：停用
    /// </summary>
    public enum SchoolActivityStatus
    {
        [Description("启用")]
        Enable = 0,

        [Description("停用")]
        Disable = 1
    }

    /// <summary>
    /// 类型 0 城市  1区
    /// </summary>
    public enum AreaType
    {
        City,
        District
    }
    /// <summary>
    /// 来源 0 userinfo表 1 浏览直播记录  2 浏览文章记录 3. 浏览反馈
    /// </summary>
    public enum UgcUserAreaFromType
    {
        /// <summary>
        /// userinfo表中的用户定位
        /// </summary>
        [Description("userinfo表")]
        UserTable = 0,

        /// <summary>
        /// 浏览直播记录中的直播定位
        /// </summary>
        [Description("浏览直播记录")]
        Live = 1,

        /// <summary>
        /// 浏览文章记录中的文章定位
        /// </summary>
        [Description("浏览文章记录")]
        Article = 2,

        /// <summary>
        /// userinfo表中的用户定位
        /// </summary>
        [Description("浏览反馈")]
        Feedback = 3,
    }

    /// <summary>
    /// 广告排期类型
    /// 广告位类型
    /// </summary>
    public enum LocationDataType
    {
        [Description("默认")]
        Default = 0,
        [Description("默认+学校")]
        School = 1,
        [Description("默认+文章")]
        Article = 2,
    }
}
