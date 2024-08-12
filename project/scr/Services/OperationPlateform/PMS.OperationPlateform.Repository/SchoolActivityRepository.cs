using PMS.OperationPlateform.Domain.DTOs;
using PMS.OperationPlateform.Domain.Entitys;
using PMS.OperationPlateform.Domain.Enums;
using PMS.OperationPlateform.Domain.IRespositories;
using ProductManagement.Framework.MSSQLAccessor.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductManagement.Framework.MSSQLAccessor;
namespace PMS.OperationPlateform.Repository
{
    public class SchoolActivityRepository : Repository<SchoolActivity, OperationCommandDBContext>, ISchoolActivityRepository
    {

        private readonly OperationCommandDBContext _db;

        public SchoolActivityRepository(OperationCommandDBContext db) : base(db)
        {
            _db = db;
        }

        public int GetVersion(Guid activityId)
        {
            var sql = " select max(Version) from SchoolActivityRegisteExtension where ActivityId = @activityId and IsDeleted = 0 ";
            return _db.Query<int>(sql, new { activityId }).FirstOrDefault();
        }

        public SchoolActivity Get(Guid id)
        {
            string sql = $@"select top 1 sla.[Id], [Title], [Subtitle], [ExtId], [Type], [Category], [Name],
            [IsConnectSchool], [StartTime], [EndTime], [Status], [IsDeleted],
            (select STRING_AGG(ISNULL(pr.papaId, ''), ',')
                from dbo.SchoolActivityPapaRelation pr
                where pr.ActivityId = sla.id
            )as Customer,
            [IsCover], [QRcodeUrl], [QRcodeTitle], [RegisteSuccessNote], [WechatShareTitle],
            [WechatShareIntro], [CreateTime], [CreatorId], [LastModifyTime], [ModifierId], [Note], [IsUnique],[Description]
            from [dbo].[SchoolActivity] sla where sla.Id = @id and sla.IsDeleted = 0 ;";

            //不管其他状态, 是否过期, 只要不删除, 就能显示, 但是不能报名成功
            return _db.Query<SchoolActivity>(sql, new { id }).FirstOrDefault();
        }

        public SchoolActivity GetCommon()
        {
            //类别(单一或统一)
            var category = SchoolActivityCategory.Common;
            var enableStatus = SchoolActivityStatus.Enable;

            //不管其他状态, 是否过期, 只要不删除, 就能报名成功
            var sql = $@" select top 1 * from SchoolActivity where IsDeleted = 0 and Category = @category and Status = @enableStatus order by LastModifyTime desc ";
            return _db.Query<SchoolActivity>(sql, new { category, enableStatus }).FirstOrDefault();
        }

        public SchoolActivity GetBySchoolExtId(Guid extId, bool? isEnable, bool? isCover)
        {
            #region 可用的活动
            var enableStatus = SchoolActivityStatus.Enable;
            var limitType = SchoolActivityType.Limit;
            var enableSql = true == isEnable ? $@" 
and (
     Status = @enableStatus
     and(
        Type != @limitType
        or(
            Type = @limitType 
            and StartTime <= getdate() 
            and EndTime >= getdate()
        )
    )
)
" : "";
            #endregion

            var sql = $@" 
select top 1 * from SchoolActivity 
where 
    ExtId = @extId
    and IsDeleted = 0
    and (@isCover is null or IsCover = @isCover)
    {enableSql}
order by 
    LastModifyTime desc
";
            return _db.Query<SchoolActivity>(sql, new { extId, isCover, limitType, enableStatus }).FirstOrDefault();
        }

        public IEnumerable<SchoolActivityExtension> GetImages(Guid activityId)
        {
            var name = "image";
            var sql = $@" select * from SchoolActivityExtension where ActivityId = @activityId and Name = @name and IsDeleted = 0 ";
            return _db.Query<SchoolActivityExtension>(sql, new { activityId, name });
        }

        public IEnumerable<SchoolActivityRegisteExtensionDto> GetExtensions(Guid activityId, int version)
        {
            var sql = $@" select * from SchoolActivityRegisteExtension where ActivityId = @activityId and Version = @version and IsDeleted = 0 order by Sort asc ";
            return _db.Query<SchoolActivityRegisteExtensionDto>(sql, new { activityId, version });
        }

        public bool ExistCustomerChannelPhone(List<int> customers, string channel, string phone)
        {
            var sql = $@"
select 1 from SchoolActivityRegiste sar
INNER JOIN SchoolActivityPapaRelation sapr on sar.ActivityId = sapr.ActivityId and sapr.PapaId in  @customers
INNER JOIN SchoolActivity a on a.id = sar.ActivityId and a.IsDeleted = 0  
 where   sar.Channel =  @channel and sar.Phone = @phone and sar.IsDeleted = 0;";
            return _db.Query<int>(sql, new { customers, channel, phone }).FirstOrDefault() > 0;
        }


        public bool ExistChannelPhone(Guid activityId, string channel, string phone)
        {
            var sql = $@" 
select 1 from SchoolActivityRegiste 
where
    ActivityId = @activityId and Channel = @channel and Phone = @phone and IsDeleted = 0
";
            return _db.Query<int>(sql, new { activityId, channel, phone }).FirstOrDefault() > 0;
        }

        public bool ExistPhone(Guid activityId, string phone)
        {
            var sql = $@" 
select 1 from SchoolActivityRegiste 
where
    ActivityId = @activityId and Phone = @phone and IsDeleted = 0
";
            return _db.Query<int>(sql, new { activityId, phone }).FirstOrDefault() > 0;
        }


        public bool Register(SchoolActivityRegiste schoolActivityRegiste)
        {
            var sql = $@"
insert into SchoolActivityRegiste
(
	Id, ActivityId, ExtId, Field0, Field1,
	Field2, Field3, Field4, Field5, Field6,
	Field7, Field8, Field9, Version, Status,
	IsDeleted, Note, CreateTime, CreatorId, LastModifyTime,
	ModifierId, Channel, Phone,SignInType
)
values(
	@Id, @ActivityId, @ExtId, @Field0, @Field1,
	@Field2, @Field3, @Field4, @Field5, @Field6,
	@Field7, @Field8, @Field9, @Version, @Status,
	@IsDeleted, @Note, @CreateTime, @CreatorId, @LastModifyTime,
	@ModifierId, @Channel, @Phone,@SignInType
)
";
            return _db.ExecuteUow(sql, schoolActivityRegiste) > 0;
        }

        public async Task<bool> SignIn(Guid activityId, string phone, SignType signType)
        {
            string sql = @"update SchoolActivityRegiste set SignInType = @signType
where ActivityId=@activityId and Phone=@phone and SignInType=0";
            return (await _db.ExecuteAsync(sql, new { activityId, phone, signType })) > 0;
        }

        public async Task<bool> HasSignIn(Guid activityId, string phone)
        {
            string sql = @"SELECT 1 FROM SchoolActivityRegiste
WHERE ActivityId=@activityId and Phone=@phone and SignInType!=0";
            return (await _db.ExecuteScalarAsync<int>(sql, new { activityId, phone })) == 1;
        }

        public async Task<IEnumerable<SchoolActivityRegiste>> GetRegiste(Guid activityId)
        {
           string sql = @"SELECT * FROM [dbo].[SchoolActivityRegiste]
WHERE ActivityId = @activityId AND IsDeleted = 0";
           return await _db.QueryAsync<SchoolActivityRegiste>(sql, new { activityId });
        }

        public async Task<IEnumerable<SchoolActivityRegisteExtension>> GetRegisteFields(Guid activityId)
        {
            string sql = @"SELECT * FROM SchoolActivityRegisteExtension WHERE ActivityId=@activityId AND IsDeleted=0";
            return await _db.QueryAsync<SchoolActivityRegisteExtension>(sql, new { activityId });
        }

        public async Task<IEnumerable<SchoolActivityProcess>> GetProcesses(Guid activityId)
        {
            string sql = "SELECT * FROM SchoolActivityProcess WHERE ActivityId=@activityId ORDER BY SchoolActivityProcess.Sort";
            return await _db.QueryAsync<SchoolActivityProcess>(sql, new { activityId });
        }
    }
}
