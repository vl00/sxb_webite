using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.OperationPlateform.Repository
{
    using Dapper;
    using PMS.OperationPlateform.Domain.DTOs;
    using PMS.OperationPlateform.Domain.Entitys;
    using PMS.OperationPlateform.Domain.Enums;
    using PMS.OperationPlateform.Domain.IRespositories;
    using ProductManagement.Framework.MSSQLAccessor;
    using ProductManagement.Framework.MSSQLAccessor.DBContext;
    using System.Threading.Tasks;

    public class ArticleRepository : BaseQueryRepository<article>, IArticleRepository
    {
        public ArticleRepository(OperationQueryDBContext db) : base(db)
        {
        }

        public IEnumerable<article> SelectSubScribeArticles(
            Article_SubscribePreference preference, SchoolGrade[] grades, Domain.Enums.SchoolType[] schoolTypes, Guid[] schoolIds, int offset = 0, int limit = 20)
        {
            StringBuilder baseSql = new StringBuilder();
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("grades", grades == null ? null : grades.Select(g => (int)g));
            parameters.Add("schoolTypes", schoolTypes == null ? null : schoolTypes.Select(st => (int)st));

            baseSql.Append(@"SELECT [id]
                            ,[title]
                            ,[author]
                            ,[author_origin]
                            ,[time]
                            ,[url]
                            ,[type]
                            ,[url_origin]
                            ,[layout]
                            ,[img]
                            ,[overview]
                            ,[toTop]
                            ,[show]
                            ,[linkOnly]
                            ,[viewCount]
                            ,[viewCount_r]
                            ,[assistant]
                            ,[city]
                            ,[createTime]
                            ,[creator]
                            ,[updateTime]
                            ,[updator]
                            ,[No]
                            FROM article WHERE IsDeleted = 0 and show = 1 AND time<=GETDATE()");

            //地区筛选优先级从区域->市->省
            if (preference.Area != null)
            {
                //如果选择的是区域，那么如果文章投放目标仅为区域上层的市或仅为省的时候也应该要收到。
                baseSql.AppendFormat(" AND EXISTS( SELECT 1 FROM Article_Areas WHERE Article_Areas.ArticleId=article.id AND (AreaId=@areaId OR (AreaId IS NULL AND CityId=@cityID AND ProvinceId=@provinceId) OR (AreaId IS NULL AND CityId IS NULL AND ProvinceId=@provinceId)  OR (AreaId IS NULL AND CityId IS NULL AND ProvinceId IS NULL)  )) ");
                parameters.Add("areaId", preference.AreaId);
                parameters.Add("cityID", preference.CityId);
                parameters.Add("provinceId", preference.ProvinceId);
            }
            else if (preference.City != null)
            {
                //如果选择的是市，那么如果文章投放目标仅为省的时候也应该要收到。
                baseSql.AppendFormat(" AND EXISTS( SELECT 1 FROM Article_Areas WHERE Article_Areas.ArticleId=article.id AND (CityId=@cityID OR (AreaId IS NULL AND CityId IS NULL AND ProvinceId=@provinceId) OR (AreaId IS NULL AND CityId IS NULL AND ProvinceId IS NULL) )) ");
                parameters.Add("cityID", preference.CityId);
                parameters.Add("provinceId", preference.ProvinceId);
            }
            else if (preference.Province != null)
            {
                //如果选择的是省，那么如果文章投放目标为全国的也要展示出来
                baseSql.AppendFormat(" AND (EXISTS( SELECT 1 FROM Article_Areas WHERE Article_Areas.ArticleId=article.id AND (ProvinceId=@ProvinceId OR (AreaId IS NULL AND CityId IS NULL AND ProvinceId IS NULL) ))) ");
                parameters.Add("ProvinceId", preference.ProvinceId);
            }


            List<string> or_contact1 = new List<string>();

            if (grades != null && grades.Length > 0)
            {
                or_contact1.Add("EXISTS(SELECT  1 FROM Article_PriodBinds WHERE Article_PriodBinds.ArticleId=article.id AND PriodId IN @grades)");
            }

            if ((grades != null && grades.Length > 0) || (schoolTypes != null && schoolTypes.Length > 0))
            {
                StringBuilder temp = new StringBuilder();
                temp.AppendFormat(@"  EXISTS(
	                        SELECT 1 FROM Article_SchoolTypeBinds WHERE  Article_SchoolTypeBinds.ArticleId=article.id
	                        AND Article_SchoolTypeBinds.SchoolTypeId IN (SELECT id FROM Article_SchoolTypes WHERE  1=1 AND (");
                List<string> and_contact = new List<string>();
                if (grades != null && grades.Length > 0)
                {
                    and_contact.Add("SchoolGrade IN @grades");
                }
                if (schoolTypes != null && schoolTypes.Length > 0)
                {
                    and_contact.Add("SchoolType IN @schoolTypes");
                }
                string contact = string.Join(" AND ", and_contact);
                temp.Append(contact);
                temp.Append(" )))");
                or_contact1.Add(temp.ToString());
            }

            if (or_contact1.Any())
            {
                var contact = string.Join(" OR ", or_contact1);
                baseSql.AppendFormat(" AND ({0})", contact);
            }

            //联合收藏学校的关联文章
            if (schoolIds != null && schoolIds.Length > 0)
            {
                baseSql.Append(" UNION ");
                baseSql.AppendFormat(@"SELECT [id]
                            ,[title]
                            ,[author]
                            ,[author_origin]
                            ,[time]
                            ,[url]
                            ,[type]
                            ,[url_origin]
                            ,[layout]
                            ,[img]
                            ,[overview]
                            ,[toTop]
                            ,[show]
                            ,[linkOnly]
                            ,[viewCount]
                            ,[viewCount_r]
                            ,[assistant]
                            ,[city]
                            ,[createTime]
                            ,[creator]
                            ,[updateTime]
                            ,[updator]
                            ,[No] FROM article WHERE IsDeleted = 0 and show = 1 AND time<=GETDATE()");

                baseSql.Append(" AND EXISTS (SELECT 1 FROM Article_SchoolBind WHERE article.id = Article_SchoolBind.ArticleId  AND Article_SchoolBind.SchoolId IN @schoolIds)");
                parameters.Add("schoolIds", schoolIds);
            }

            baseSql.AppendFormat(@" ORDER BY ToTop desc,time DESC  OFFSET {0} ROWS
                                    FETCH NEXT {1} ROWS ONLY", offset, limit);
            baseSql.Append(";");

            return _db.Query<article>(baseSql.ToString(), parameters);
        }

        public article GetArticle(Guid id, bool isShow)
        {
            string sql = @"SELECT * FROM article WHERE IsDeleted = 0 and id=@Id AND show=@isShow";
            return _db.QuerySingle<article>(sql, new { Id = id, isShow = isShow });
        }

        public IEnumerable<article> GetArticleViewCounts(IEnumerable<Guid> aids)
        {
            string sql = @"SELECT id,viewCount,viewCount_r FROM article WHERE IsDeleted = 0 and id in @Ids";
            return _db.Query<article>(sql, new { Ids = aids });
        }

        public article GetArticleNoHtml(Guid id)
        {
            string sql = @"SELECT id,title,author,author_origin,time,url,url_origin,layout,img,overview,toTop,show,linkOnly,viewCount,viewCount_r,assistant,city,type,[No] FROM article WHERE IsDeleted = 0 and id=@Id";
            return _db.QuerySingle<article>(sql, new { Id = id });
        }

        public IEnumerable<Article_Areas> GetCorrelationAreas(Guid id)
        {
            string sql = @"SELECT * FROM Article_Areas WHERE ArticleId=@id";
            return _db.Query<Article_Areas>(sql, new { id = id });
        }

        public IEnumerable<article> GetCorrelationArticle(Guid id, int top = 10)
        {
            string sql1 = $@"
                     select
                     TOP {top}
                          id,title,author,author_origin,time,url,url_origin,layout,img,overview,toTop,show,linkOnly,viewCount,viewCount_r,assistant,city,type,no
                     	  from article
                           join(select  dataID, COUNT(1) matchRate from tag_bind where 
                          dataType = 1
                          and
                          tagID in (select tagID from tag_bind where dataID = @articleId)
                          and
                          dataID <> @articleId
                          group by dataID) tmp1 on article.id = tmp1.dataID
                     	 WHERE IsDeleted = 0 and show = 1 AND time<=GETDATE()
                     	 ORDER BY tmp1.matchRate DESC,article.time desc,NEWID()
                        ";
            var list = _db.Query<article>(sql1, new { articleId = id });
            return list;
        }

        public IEnumerable<GroupQRCode> GetCorrelationGQRCodes(Guid id)
        {
            string sql = @"select GroupQRCode.* from GroupQRCode
                        join  article_gqrcbinds on GroupQRCode.Id = article_gqrcbinds.gqrcid
                        where articleId=@articleId ";
            return _db.Query<GroupQRCode>(sql, new { articleId = id });
        }

        public IEnumerable<Article_SchoolBind> GetCorrelationSchool(Guid id)
        {
            string sql = @"SELECT top 10 * FROM Article_SchoolBind where ArticleId=@id";
            return _db.Query<Article_SchoolBind>(sql, new { id = id });
        }

        public IEnumerable<Article_SchoolTypes> GetCorrelationSchoolTypes(Guid id)
        {
            string sql = @"SELECT Article_SchoolTypes.* FROM Article_SchoolTypeBinds
                        JOIN Article_SchoolTypes ON Article_SchoolTypeBinds.SchoolTypeId = Article_SchoolTypes.Id
                        WHERE ArticleId=@Id";
            return _db.Query<Article_SchoolTypes>(sql, new { Id = id });
        }

        public IEnumerable<Article_SCMBinds> GetCorrelationSCMs(Guid id)
        {
            string sql = @"
                        SELECT * FROM Article_SCMBinds
                        WHERE ArticleId = @articleId ";
            return _db.Query<Article_SCMBinds>(sql, new { articleId = id });
        }

        public IEnumerable<tag_bind> GetCorrelationTags(Guid id, bool ms)
        {
            string sql = @"SELECT tagID FROM tag_bind WHERE dataType =1 AND dataID = @articleId AND ms=@ms";
            return _db.Query<tag_bind>(sql, new { articleId = id, ms });
        }

        //public IEnumerable<article> GetOgArticles(out int total, int pageIdx, int pageCount = 20)
        //{
        //    IEnumerable<article> _articles;
        //    int _total = 0;
        //    string sql = @"SELECT
        //            id,title,author,author_origin,time,url,url_origin,layout,img,overview,toTop,show,linkOnly,viewCount,viewCount_r,assistant,city,type
        //            ,(select COUNT(1) from comment where comment.forumID=article.id) commentCount
        //            FROM article WHERE show=1
        //            AND time<=GETDATE()
        //            Order by toTop desc, time desc
        //            OFFSET @skip ROWS
        //            FETCH NEXT @take ROWS ONLY;
        //            SELECT COUNT(1) FROM article WHERE show=1
        //            ";
        //    using (var multi = _db.QueryMultiple(sql, new { skip = (pageIdx - 1) * pageCount, take = pageCount }))
        //    {
        //        _articles = multi.Read<article>();
        //        _total = multi.Read<int>().Single();
        //    }
        //    total = _total;
        //    return _articles;
        //}

        public IEnumerable<Article_SchoolTypes> GetArticleSchoolTypes()
        {
            return _db.GetAll<Article_SchoolTypes>();
        }

        public Article_SubscribePreference GetArticleSubscribeInfoByUserId(Guid userId)
        {
            string sql = "SELECT* FROM Article_SubscribePreference WHERE UserId = @userId";
            return _db.QuerySingle<Article_SubscribePreference>(sql, new { userId = userId });
        }

        public IEnumerable<article> GetUserSubscribeArticles(Guid userId, int currentPage, int pageSize)
        {
            IEnumerable<article> _articles = _db.Query<article>(
                "usp_select_user_subscribe_articles",
               new { userId = userId, currentPage, pageSize },
               null,
               System.Data.CommandType.StoredProcedure);
            return _articles;
        }

        //public Article_SubscribePreference GetArticleSubscribeDetailInfoByUserId(Guid userId)
        //{
        //    //string sql = @"SELECT A1.*,A2.* FROM Article_SubscribePreference A1
        //    //    JOIN Article_SchoolTypes A2 ON A1.SchoolTypeId = A2.Id
        //    //    WHERE A1.UserId=@userId
        //    //    ";
        //    //return _db.Query<Article_SubscribePreference, Article_SchoolTypes, Article_SubscribePreference>(sql, (a1, a2) =>
        //    //{
        //    //    a1.SchoolType = a2;
        //    //    return a1;
        //    //}, new { userId }).FirstOrDefault();

        //}

        public (IEnumerable<article> articles, int total) SelectPolicyArticles(List<Domain.DTOs.SchoolType> schoolTypes, List<Local> locals, bool isPage, int offset = 0, int limit = 20)
        {
            DynamicParameters parameters = new DynamicParameters();

            StringBuilder sql = new StringBuilder(100);
            sql.AppendFormat(@"SELECT 1 FROM Policy_Articles A2 WHERE (1=1) ");
            StringBuilder where_schoolTypes = new StringBuilder();
            if (schoolTypes != null)
            {
                for (int i = 0; i < schoolTypes.Count; i++)
                {
                    string st = "@schoolType" + i;
                    string sg = "@schoolGrade" + i;
                    string cn = "@chinese" + i;
                    string dg = "@diglossia" + i;
                    string dc = "@discount" + i;
                    where_schoolTypes.AppendFormat("( schoolType={0} and schoolGrade={1} and chinese={2} and diglossia={3} and discount={4})"
                   , st, sg, cn, dg, dc);
                    if (i < (schoolTypes.Count - 1))
                    {
                        where_schoolTypes.Append(" or ");
                    }
                    parameters.Add(st, schoolTypes[i].Type);
                    parameters.Add(sg, schoolTypes[i].SchoolGrade);
                    parameters.Add(cn, schoolTypes[i].Chinese);
                    parameters.Add(dg, schoolTypes[i].Diglossia);
                    parameters.Add(dc, schoolTypes[i].Discount);
                }
            }
            if (where_schoolTypes.Length > 0)
            {
                sql.AppendFormat(" and ({0})", where_schoolTypes);
            }

            StringBuilder where_locals = new StringBuilder();
            if (locals != null)
            {
                for (int i = 0; i < locals.Count; i++)
                {
                    var currentItem = locals[i];
                    List<string> tmp = new List<string>();
                    if (currentItem.ProvinceId != null)
                    {
                        string pid = "@pid" + i;
                        tmp.Add(string.Format("provinceid={0}", pid));
                        parameters.Add(pid, currentItem.ProvinceId.GetValueOrDefault().ToString());
                    }
                    if (currentItem.CityId != null)
                    {
                        string cid = "@cid" + i;
                        tmp.Add(string.Format("cityId={0}", cid));
                        parameters.Add(cid, currentItem.CityId.GetValueOrDefault().ToString());
                    }
                    if (currentItem.AreaId != null)
                    {
                        string aid = "@aid" + i;
                        tmp.Add(string.Format("areaId={0}", aid));
                        parameters.Add(aid, currentItem.AreaId.GetValueOrDefault().ToString());
                    }
                    string end = string.Empty;  //最终拼接好的单元
                    if (tmp.Count > 0)
                    {
                        end = $"({string.Join(" and ", tmp)})";
                        if (i < (locals.Count - 1))
                        {
                            end += " or ";
                        }
                    }

                    where_locals.Append(end);//上车
                }
            }
            if (where_locals.Length > 0)
            {
                sql.AppendFormat(" and ({0})", where_locals);
            }

            return base.SelectPage(
                  where: string.Format(@"
                EXISTS( {0} and (article.id= A2.id) )
                and article.show = 1
                and article.IsDeleted = 0
                and article.time<GETDATE()", sql),
                  param: parameters,
                  order: "time desc",
                  fileds: new string[] {
                    "[id]",
                    "[title]",
                    "[author]",
                    "[author_origin]" ,
                    "[time]",
                    "[url]",
                    "[type]",
                    "[url_origin]",
                    "[layout]",
                    "[img]",
                    "[overview]",
                    "[toTop]",
                    "[show]",
                    "[linkOnly]",
                    "[viewCount]",
                    "[viewCount_r]",
                    "[assistant]",
                    "[city]",
                    "[No]"},
                  isPage: isPage,
                  offset: offset,
                  limit: limit
                  );
        }

        public (IEnumerable<article> articles, int total) SelectComparisionArticles(List<Guid> branchIds, bool isPage, int offset = 0, int limit = 20)
        {
            return base.SelectPage(
                  where: @"
                EXISTS(SELECT 1 FROM Comparision_Articles A2 WHERE article.id= A2.id AND SchoolId IN @sids )
                and article.show = 1
                and article.IsDeleted = 0
                and article.time<GETDATE() ",
                  param: new { sids = branchIds },
                  order: "time desc",
                  fileds: new string[] {
                    "[id]",
                    "[title]",
                    "[author]",
                    "[author_origin]" ,
                    "[time]",
                    "[url]",
                    "[type]",
                    "[url_origin]",
                    "[layout]",
                    "[img]",
                    "[overview]",
                    "[toTop]",
                    "[show]",
                    "[linkOnly]",
                    "[viewCount]",
                    "[viewCount_r]",
                    "[assistant]",
                    "[city]",
                    "[No]"},
                  isPage: isPage,
                  offset: offset,
                  limit: limit
                  );
        }

        public IEnumerable<article> SelectByIds(Guid[] ids, string[] fileds = null)
        {
            if (ids == null || !ids.Any())
            {
                return null;
            }

            if (fileds == null || !fileds.Any())
            {
                fileds = new string[] { "[id]", "[title]", "[author]", "[author_origin]", "[time]", "[url]", "[type]", "[url_origin]", "[layout]", "[img]", "[overview]", "[toTop]", "[show]", "[linkOnly]", "[viewCount]", "[viewCount_r]", "[assistant]", "[city]", "[No]" };
            }
            string sql = $@"SELECT {string.Join(",", fileds)} FROM [dbo].[article] WHERE IsDeleted = 0 and id IN ({string.Join(",", ids.Select(p => $"'{p}'"))})";
            var result = _db.Query<article>(sql, new { });
            return result;
        }

        public IEnumerable<Article_ESData> GetArticleData(int pageNo, int pageSize, DateTime lastTime)
        {
            string sql = @"
SELECT --top 100
	[id],[title],[time],[show],[viewCount],[viewCount_r],CreateTime,updateTime,ttt.tagName
FROM 
	[dbo].[article]
	left JOIN (
		SELECT t2.dataID, tagName = STUFF((
	                                SELECT ',' + tag.name
	                                FROM [iSchoolData].[dbo].GeneralTagBind AS tagBind
	                                inner join [iSchoolData].[dbo].GeneralTag tag on tagBind.tagID = tag.id
	                                WHERE tagBind.dataID = t2.dataID
	                                FOR XML PATH('')
	                                ), 1, 1, '')
		FROM [iSchoolData].[dbo].GeneralTagBind AS t2
		WHERE  
			t2.dataType = 1 and t2.dataID is not null
		GROUP BY t2.dataID
	) AS ttt on ttt.dataID = id
where
	IsDeleted = 0  
	and(
		([time] > @lastTime and  CreateTime is null and updateTime is null)
		or CreateTime > @lastTime or updateTime > @lastTime
	)
   and [time] <= getdate() {0}
order BY 
	[time] asc 
OFFSET @offset ROWS 
FETCH NEXT @limit ROWS ONLY;";

            string where = "";
            if (lastTime == Convert.ToDateTime("1999-01-01"))
            {
                where += " and [show] = 1 ";
            }

            var result = _db.Query<Article_ESData>(string.Format(sql, where), new { lastTime, offset = pageNo * pageSize, limit = pageSize });
            return result;
        }

        public (IEnumerable<view_articlejoinuv>, int total) SelectView_ArticleJoinUVPage(string where, object param = null, string order = null, string[] fileds = null, bool isPage = false, int offset = 0, int limit = 20)
        {
            return _db.GetBy<view_articlejoinuv>(where, param, order, fileds, isPage, offset, limit);
        }

        public IEnumerable<article> GetTalent(Guid talentId, int page, int size, List<Guid> articleIds = default)
        {
            DynamicParameters sqlParameters = new DynamicParameters();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(@"select 
	                                                a.id,
	                                                a.title,
	                                                substring(a.html,0,300) as html,
	                                                a.viewCount,
	                                                a.viewCount_r,
                                                    a.show,
                                                    a.time
	                                                from Article_Talent as t
	                                                left join article as a on t.ArticleID = a.Id and a.IsDeleted = 0 
	                                                where TalentID = @talentId
                                               ");

            sqlParameters.Add("@talentId", talentId);
            sqlParameters.Add("@page", page);
            sqlParameters.Add("@size", size);

            if (articleIds != null)
            {
                stringBuilder.Append(" and a.id in @articleIds");
                sqlParameters.Add("@articleIds", articleIds);
            }

            stringBuilder.Append(" order by a.time desc offset (@page - 1) * @size rows fetch next @size rows only ");

            return _db.Query<article>(stringBuilder.ToString(), sqlParameters).ToList();
        }


        public Guid? GetAuthorTalentId(Guid articleId)
        {
            var sql = $@"
select
    AT.TalentID
from
    Article_Talent AT
where 
    AT.ArticleID = @articleId
";
            return _db.Query<Guid?>(sql, new { articleId }).FirstOrDefault();
        }

        public long GetArticleTotal(Guid talentId, DateTime? startTime, DateTime? endTime)
        {
            var sql = $@"
select
    count(1)
from
    Article_Talent AT
    left join article A on AT.ArticleID = A.Id and A.IsDeleted = 0
where 
    AT.TalentID = @talentId
	and (@startTime is null or A.updateTime >= @startTime)
	and (@endTime is null or A.updateTime <= @endTime)

";
            return _db.QuerySingle<long>(sql, new { talentId, startTime, endTime });
        }

        public IEnumerable<Guid> GetArticleId(List<Guid> ids)
        {
            string sql = @"
                        SELECT [id]
                          FROM [dbo].[article]
                          WHERE IsDeleted = 0 and id IN @ids";
            var result = _db.Query<Guid>(sql, new { ids = ids });
            return result;
        }

        public Dictionary<int, string> GetNearArticle(int no)
        {
            var str_SQL = @"SELECT
	                            [No],
	                            Title 
                            FROM
	                            article 
                            WHERE
	                            IsDeleted = 0 and [No] IN (
		                            ( SELECT MIN ( [No] ) FROM article WHERE [No] > @NO AND IsDeleted = 0 and Show = 1),
		                            ( SELECT MAX ( [No] ) FROM article WHERE [No] < @NO AND IsDeleted = 0 and Show = 1) 
	                            )
                            ORDER BY
	                            [No]";
            var result = _db.Query<(int, string)>(str_SQL, new { no }).ToDictionary(k => k.Item1, v => v.Item2);
            return result;
        }

        public article GetArticleByNo(long no)
        {
            string sql = @"
                        SELECT *
                          FROM [dbo].[article]
                          WHERE IsDeleted = 0 and No = @no";
            return _db.Query<article>(sql, new { no }).FirstOrDefault();
        }

        public async Task<IEnumerable<tag_bind>> GetHotTags(int limit = 30)
        {
            string sql = $@"SELECT TOP {limit}
                                tb.tagID,
	                            COUNT ( tb.dataID ) AS bindCount 
                            FROM
	                            tag_bind as tb
	                            LEFT JOIN article as a on a.id = tb.dataID and a.IsDeleted = 0 
                            WHERE
	                            tb.dataType = 1 
	                            AND tb.tagID IS NOT NULL 
	                            and a.show = 1
                            GROUP BY
	                            tb.tagID
                            ORDER BY
	                            bindCount DESC";
            return await _db.QueryAsync<tag_bind>(sql, new { limit });
        }

        public async Task<Dictionary<Guid, bool>> CheckTagIDExist(IEnumerable<Guid> ids)
        {
            var str_SQL = $@"SELECT 
                                DISTINCT(tagID) 
                            FROM
	                            tag_bind 
                            WHERE
	                            tagid IN @ids
	                            AND 
                                dataType = 1";
            var result = await _db.QueryAsync<Guid>(str_SQL, new { ids });
            var dic = new Dictionary<Guid, bool>();
            if (result?.Any() == true)
            {
                foreach (var item in ids.Distinct())
                {
                    dic.Add(item, result.Contains(item));
                }
            }
            return dic;
        }

        public async Task<(IEnumerable<article>, int)> PageByTagID(Guid tagID, int offset = 0, int limit = 20)
        {
            var str_SQL_Total = $@"SELECT
	                                Count(1)
                                FROM
	                                article AS a
	                                LEFT JOIN tag_bind AS tb ON tb.dataID = a.id 
                                WHERE
	                                tagID = @tagID 
	                                AND a.IsDeleted = 0 and a.show = 1";
            var total = await _db.QuerySingleAsync<int>(str_SQL_Total, new { tagID });
            var str_SQL = $@"SELECT
	                            a.* 
                            FROM
	                            article AS a
	                            LEFT JOIN tag_bind AS tb ON tb.dataID = a.id 
                            WHERE
	                            tagID = @tagID 
	                            AND a.IsDeleted = 0 and a.show = 1 
                            ORDER BY
	                            updateTime DESC
                            OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY;";
            return (await _db.QueryAsync<article>(str_SQL, new { tagID, offset, limit }), total);
        }


        public async Task<IEnumerable<Guid>> GetLastestArticleIds(int size)
        {
            string sql = $@"
SELECT
    top {size} id
FROM 
	[dbo].[article]
WHERE
	IsDeleted = 0 and Show = 1
ORDER BY 
	[time] desc 
";
            return await _db.QueryAsync<Guid>(sql, new{ });
        }

        public async Task<IEnumerable<article>> GetNewest(int count = 9)
        {
            var str_SQL = $@"SELECT TOP {count} 
                                id,
	                            title,
	                            author,
	                            [TIME],
	                            toTop,
	                            show,
	                            linkOnly,
	                            viewCount,
	                            viewCount_r,
	                            assistant,
	                            city,
	                            type,
	                            [NO] 
                            FROM
	                            article 
                            WHERE
	                            IsDeleted = 0 
	                            AND show = 1 
                            ORDER BY
	                            article.time DESC";
            return await db.QueryAsync<article>(str_SQL, new { });
        }


        public async Task<Guid?> GetTalentUserId(Guid articleId)
        {
            string sql = @"
SELECT  userInfo.id FROM Article_Talent
JOIN article ON article.id = Article_Talent.ArticleID
JOIN iSchoolUser.dbo.talent ON talent.id = Article_Talent.TalentID
JOIN iSchoolUser.dbo.userInfo  on userInfo.id =talent.user_id
WHERE article.id = @articleId ";
            return await db.ExecuteScalarAsync<Guid?>(sql, new { articleId });
        }

        public async Task<IEnumerable<article>> GetTopViewCounts(int cityId, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            string sql = $@"
select * from
	article A
where 
    IsDeleted = 0
    AND SHOW = 1
    AND Time between @startTime and @endTime
    AND (
        EXISTS(
            SELECT 1 FROM Article_Areas WHERE Article_Areas.ArticleId = A.id AND CityId=@cityId
        ) 
        OR NOT EXISTS(
            SELECT 1 FROM Article_Areas WHERE A.id = Article_Areas.ArticleId
        )
    )
ORDER BY
    ViewCount desc, ToTop desc, time DESC
offset (@pageIndex-1)*@pageSize row
FETCH next @pageSize rows only
";
            return await _db.QueryAsync<article>(sql, new { cityId, startTime, endTime, pageIndex, pageSize });
        }
    }
}