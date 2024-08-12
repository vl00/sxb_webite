using iSchool;
using Microsoft.AspNetCore.Mvc;
using NPOI.XSSF.UserModel;
using PMS.Infrastructure.Application.IService;
using PMS.School.Application.IServices;
using PMS.School.Domain.Entities.WechatDemo;
using Sxb.Inside.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Inside.Controllers
{
    public class WechatDemoController : Controller
    {
        IOnlineSchoolRecruitService _onlineSchoolRecruitService;
        ISchoolOverViewService _schoolOverViewService;
        ICityInfoService _cityInfoService;
        ISchoolService _schoolService;
        IOnlineSchoolQuotaService _onlineSchoolQuotaService;
        IOnlineSchoolFractionService _onlineSchoolFractionService;
        IAreaRecruitPlanService _areaRecruitPlanService;
        IOnlineSchoolProjectService _onlineSchoolProjectService;
        IOnlineSchoolAchievementService _onlineSchoolAchievementService;
        IOnlineSchoolExtLevelService _onlineSchoolExtLevelService;

        public WechatDemoController(IOnlineSchoolRecruitService onlineSchoolRecruitService, ISchoolOverViewService schoolOverViewService, ICityInfoService cityInfoService, ISchoolService schoolService,
            IOnlineSchoolQuotaService onlineSchoolQuotaService, IOnlineSchoolFractionService onlineSchoolFractionService, IAreaRecruitPlanService areaRecruitPlanService, IOnlineSchoolProjectService onlineSchoolProjectService,
            IOnlineSchoolAchievementService onlineSchoolAchievementService, IOnlineSchoolExtLevelService onlineSchoolExtLevelService)
        {
            _onlineSchoolExtLevelService = onlineSchoolExtLevelService;
            _onlineSchoolAchievementService = onlineSchoolAchievementService;
            _onlineSchoolProjectService = onlineSchoolProjectService;
            _areaRecruitPlanService = areaRecruitPlanService;
            _onlineSchoolFractionService = onlineSchoolFractionService;
            _onlineSchoolQuotaService = onlineSchoolQuotaService;
            _schoolService = schoolService;
            _cityInfoService = cityInfoService;
            _schoolOverViewService = schoolOverViewService;
            _onlineSchoolRecruitService = onlineSchoolRecruitService;
        }

        /// <summary>
        /// 导入招生信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadSchools()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(1);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var groupNames = new List<KeyValuePair<string, (int, int)>>();
            var groupRow = currentSheet.GetRow(0);
            foreach (var merge in currentSheet.MergedRegions)
            {
                var cell = groupRow.GetCell(merge.FirstColumn);
                groupNames.Add(new KeyValuePair<string, (int, int)>(cell.StringCellValue, (merge.FirstColumn, merge.LastColumn)));
            }
            if (groupNames.Any()) groupNames = groupNames.OrderBy(p => p.Value.Item1).ToList();
            var datas = new List<IEnumerable<(int, string, string, string)>>();
            for (int i = 3; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) cellValue = null;
                    rowData.Add((o, groupNames.FirstOrDefault(p => p.Value.Item2 >= o && p.Value.Item1 <= o).Key,
                        titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            var errorEIDs = new List<Guid>();//未插入的学部

            var insertRecruits = new List<OnlineSchoolRecruitInfo>();
            var insertSchoolOverviews = new List<SchoolOverViewInfo>();
            var recruitWays = new Dictionary<Guid, string>();

            foreach (var row in datas)
            {
                if (!Guid.TryParse(row.FirstOrDefault(p => p.Item1 == 2).Item4, out Guid eid)) continue;
                #region OnlineSchoolRecruitInfo #46-233

                await _onlineSchoolRecruitService.DeleteIfExisted(eid);

                //招生信息-广州全类型幼儿园+外籍人员全学段学校+国际高中专用 46-56
                var recruitDatas = row.Where(p => p.Item1 >= 46 && p.Item1 <= 56);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 46:
                                entity.Requirement = item.Item4;
                                break;
                            case 47:
                                entity.Target = item.Item4;
                                break;
                            case 48:
                                 entity.MaxAge = item.Item4;
                                break;
                            case 49:
                                 entity.MinAge = item.Item4;
                                break;
                            case 50:
                                if (int.TryParse(item.Item4, out int qantity)) entity.Quantity = qantity;
                                break;
                            case 51:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 52:
                                entity.Scale = item.Item4;
                                break;
                            case 53:
                                entity.Score = item.Item4;
                                break;
                            case 54:
                                entity.Material = item.Item4;
                                break;
                            case 56:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //户籍生招生信息-广州公民办小学专用	57-67
                recruitDatas = row.Where(p => p.Item1 >= 57 && p.Item1 <= 67);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 2,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 57:
                                entity.Requirement = item.Item4;
                                break;
                            case 58:
                                entity.Target = item.Item4;
                                break;
                            case 59:
                                entity.MaxAge = item.Item4;
                                break;
                            case 60:
                                entity.MinAge = item.Item4;
                                break;
                            case 61:
                                if (int.TryParse(item.Item4, out int qantity)) entity.Quantity = qantity;
                                break;
                            case 62:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 63:
                                entity.Scale = item.Item4;
                                break;
                            case 64:
                                entity.Score = item.Item4;
                                break;
                            case 65:
                                entity.Material = item.Item4;
                                break;
                            case 66:
                                //entity.Brief = item.Item4;
                                break;
                            case 67:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //户籍生招生信息-广州公民办初中专用	68-81											
                recruitDatas = row.Where(p => p.Item1 >= 68 && p.Item1 <= 81);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 2,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 68:
                                entity.Requirement = item.Item4;
                                break;
                            case 69:
                                entity.Target = item.Item4;
                                break;
                            case 70:
                                entity.MaxAge = item.Item4;
                                break;
                            case 71:
                                entity.MinAge = item.Item4;
                                break;
                            case 72:
                                if (int.TryParse(item.Item4, out int qantity)) entity.Quantity = qantity;
                                break;
                            case 73:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 74:
                                entity.Scale = item.Item4;
                                break;
                            case 75:
                                entity.AllocationPrimary = item.Item4;
                                break;
                            case 76:
                                entity.CounterpartPrimary = item.Item4;
                                break;
                            case 77:
                                entity.AllocationScore = item.Item4;
                                break;
                            case 78:
                                entity.CounterpartScore = item.Item4;
                                break;
                            case 79:
                                entity.Material = item.Item4;
                                break;
                            case 80:
                                //entity.Brief = item.Item4;
                                break;
                            case 81:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //招生信息-广州公民办高中专用 82-93
                recruitDatas = row.Where(p => p.Item1 >= 82 && p.Item1 <= 93);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 82:
                                entity.Requirement = item.Item4;
                                break;
                            case 83:
                                entity.Target = item.Item4;
                                break;
                            case 84:
                                entity.MaxAge = item.Item4;
                                break;
                            case 85:
                                entity.MinAge = item.Item4;
                                break;
                            case 86:
                                if (int.TryParse(item.Item4, out int planQuantity)) entity.PlanQuantity = planQuantity;
                                break;
                            case 87:
                                if (int.TryParse(item.Item4, out int quotaPlanQuantity)) entity.QuotaPlanQuantity = quotaPlanQuantity;
                                break;
                            case 88:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 89:
                                entity.Scale = item.Item4;
                                break;
                            case 90:
                                entity.Score = item.Item4;
                                break;
                            case 91:
                                entity.Material = item.Item4;
                                break;
                            case 92:
                                //entity.CounterpartScore = item.Item4;
                                break;
                            case 93:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //非户籍生积分入学-广州公民办小学+公民办初中专用 94-105
                recruitDatas = row.Where(p => p.Item1 >= 94 && p.Item1 <= 105);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 3,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 94:
                                entity.Requirement = item.Item4;
                                break;
                            case 95:
                                entity.Target = item.Item4;
                                break;
                            case 96:
                                entity.MaxAge = item.Item4;
                                break;
                            case 97:
                                entity.MinAge = item.Item4;
                                break;
                            case 98:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 99:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 100:
                                if (int.TryParse(item.Item4, out int integralPassLevel)) entity.IntegralPassLevel = integralPassLevel;
                                break;
                            case 101:
                                if (int.TryParse(item.Item4, out int integralAdmitLevel)) entity.IntegralAdmitLevel = integralAdmitLevel;
                                break;
                            case 102:
                                entity.Material = item.Item4;
                                break;
                            case 103:
                                //entity.Material = item.Item4;
                                break;
                            case 104:
                                entity.Brief = item.Item4;
                                break;
                            case 105:
                                entity.IntegralImgUrl = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //分类招生信息-广州民办小学+民办初中专用 106-116
                recruitDatas = row.Where(p => p.Item1 >= 106 && p.Item1 <= 116);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 4,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 106:
                                entity.Requirement = item.Item4;
                                break;
                            case 107:
                                entity.Target = item.Item4;
                                break;
                            case 108:
                                entity.MaxAge = item.Item4;
                                break;
                            case 109:
                                entity.MinAge = item.Item4;
                                break;
                            case 110:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 111:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 112:
                                entity.Scale = item.Item4;
                                break;
                            case 113:
                                entity.Score = item.Item4;
                                break;
                            case 114:
                                entity.Material = item.Item4;
                                break;
                            case 115:
                                //entity.Material = item.Item4;
                                break;
                            case 116:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //社会公开招生信息-广州民办小学+民办初中专用 117-127
                recruitDatas = row.Where(p => p.Item1 >= 117 && p.Item1 <= 127);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 5,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 117:
                                entity.Requirement = item.Item4;
                                break;
                            case 118:
                                entity.Target = item.Item4;
                                break;
                            case 119:
                                entity.MaxAge = item.Item4;
                                break;
                            case 120:
                                entity.MinAge = item.Item4;
                                break;
                            case 121:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 122:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 123:
                                entity.Scale = item.Item4;
                                break;
                            case 124:
                                entity.Score = item.Item4;
                                break;
                            case 125:
                                entity.Material = item.Item4;
                                break;
                            case 126:
                                //entity.Material = item.Item4;
                                break;
                            case 127:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //招生信息-深圳全学段学校专用 128-139
                recruitDatas = row.Where(p => p.Item1 >= 128 && p.Item1 <= 139);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 128:
                                entity.Requirement = item.Item4;
                                break;
                            case 129:
                                entity.Target = item.Item4;
                                break;
                            case 130:
                                entity.MaxAge = item.Item4;
                                break;
                            case 131:
                                entity.MinAge = item.Item4;
                                break;
                            case 132:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 133:
                                if (int.TryParse(item.Item4, out int quotaPlanQuantity)) entity.QuotaPlanQuantity = quotaPlanQuantity;
                                break;
                            case 134:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 135:
                                entity.Scale = item.Item4;
                                break;
                            case 136:
                                entity.SchoolScope = item.Item4;
                                break;
                            case 137:
                                entity.Material = item.Item4;
                                break;
                            case 138:
                                //entity.Brief = item.Item4;
                                break;
                            case 139:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //招生信息-成都全类型幼儿园、小学、初中+国际高中+外籍高中专用 140-150
                recruitDatas = row.Where(p => p.Item1 >= 140 && p.Item1 <= 150);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 140:
                                entity.Requirement = item.Item4;
                                break;
                            case 141:
                                entity.Target = item.Item4;
                                break;
                            case 142:
                                entity.MaxAge = item.Item4;
                                break;
                            case 143:
                                entity.MinAge = item.Item4;
                                break;
                            case 144:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 145:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 146:
                                entity.Scale = item.Item4;
                                break;
                            case 147:
                                entity.ScribingScopeStr = item.Item4;
                                break;
                            case 148:
                                entity.Material = item.Item4;
                                break;
                            case 149:
                                //entity.Material = item.Item4;
                                break;
                            case 150:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //招生信息-成都公民办高中专用 151-163
                recruitDatas = row.Where(p => p.Item1 >= 151 && p.Item1 <= 163);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 151:
                                entity.Requirement = item.Item4;
                                break;
                            case 152:
                                entity.Target = item.Item4;
                                break;
                            case 153:
                                entity.MaxAge = item.Item4;
                                break;
                            case 154:
                                entity.MinAge = item.Item4;
                                break;
                            case 155:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 156:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 157:
                                if (int.TryParse(item.Item4, out int tzQuantity)) entity.TZQuantity = tzQuantity;
                                break;
                            case 158:
                                if (int.TryParse(item.Item4, out int tjQuantity)) entity.TJQuantity = tjQuantity;
                                break;
                            case 159:
                                entity.Scale = item.Item4;
                                break;
                            case 160:
                                entity.Score = item.Item4;
                                break;
                            case 161:
                                entity.Material = item.Item4;
                                break;
                            case 162:
                                //entity.Brief = item.Item4;
                                break;
                            case 163:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //面向“5+2”区域招生-成都公民办高中专用 164-169
                recruitDatas = row.Where(p => p.Item1 >= 164 && p.Item1 <= 169);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 7,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 164:
                                entity.MaxAge = item.Item4;
                                break;
                            case 165:
                                entity.MinAge = item.Item4;
                                break;
                            case 166:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 167:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 168:
                                if (int.TryParse(item.Item4, out int tzQuantity)) entity.TZQuantity = tzQuantity;
                                break;
                            case 169:
                                if (int.TryParse(item.Item4, out int tjQuantity)) entity.TJQuantity = tjQuantity;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //面向非“5+2”区域招生-成都公民办高中专用 170-175
                recruitDatas = row.Where(p => p.Item1 >= 170 && p.Item1 <= 175);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 8,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 170:
                                entity.MaxAge = item.Item4;
                                break;
                            case 171:
                                entity.MinAge = item.Item4;
                                break;
                            case 172:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 173:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 174:
                                if (int.TryParse(item.Item4, out int tzQuantity)) entity.TZQuantity = tzQuantity;
                                break;
                            case 175:
                                if (int.TryParse(item.Item4, out int tjQuantity)) entity.TJQuantity = tjQuantity;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //项目班招生计划-成都公民办高中专用 176-178
                recruitDatas = row.Where(p => p.Item1 >= 176 && p.Item1 <= 178);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 9,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 176:
                                entity.ProjectClassName = item.Item4;
                                break;
                            case 177:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 178:
                                entity.Score = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //招生信息-重庆全学段专用 179-190
                recruitDatas = row.Where(p => p.Item1 >= 179 && p.Item1 <= 190);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 179:
                                entity.Requirement = item.Item4;
                                break;
                            case 180:
                                entity.Target = item.Item4;
                                break;
                            case 181:
                                entity.MaxAge = item.Item4;
                                break;
                            case 182:
                                entity.MinAge = item.Item4;
                                break;
                            case 183:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 184:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 185:
                                entity.Scale = item.Item4;
                                break;
                            case 186:
                                entity.Score = item.Item4;
                                break;
                            case 187:
                                entity.ScribingScopeStr = item.Item4;
                                break;
                            case 188:
                                entity.Material = item.Item4;
                                break;
                            case 189:
                                //entity.Material = item.Item4;
                                break;
                            case 190:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //招生信息-重庆公民办高中专用 191-203
                recruitDatas = row.Where(p => p.Item1 >= 191 && p.Item1 <= 203);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 0,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 191:
                                entity.Requirement = item.Item4;
                                break;
                            case 192:
                                entity.Target = item.Item4;
                                break;
                            case 193:
                                entity.MaxAge = item.Item4;
                                break;
                            case 194:
                                entity.MinAge = item.Item4;
                                break;
                            case 195:
                                if (int.TryParse(item.Item4, out int planQuantity)) entity.PlanQuantity = planQuantity;
                                break;
                            case 196:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 197:
                                if (int.TryParse(item.Item4, out int pzQuantity)) entity.PZQuantity = pzQuantity;
                                break;
                            case 198:
                                if (int.TryParse(item.Item4, out int lzQuantity)) entity.LZQuantity = lzQuantity;
                                break;
                            case 199:
                                entity.Scale = item.Item4;
                                break;
                            case 200:
                                entity.Score = item.Item4;
                                break;
                            case 201:
                                entity.Material = item.Item4;
                                break;
                            case 202:
                                //entity.Brief = item.Item4;
                                break;
                            case 203:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //自主招生-广州公民办高中专用 204-214
                recruitDatas = row.Where(p => p.Item1 >= 204 && p.Item1 <= 214);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 6,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 204:
                                entity.Requirement = item.Item4;
                                break;
                            case 205:
                                entity.Target = item.Item4;
                                break;
                            case 206:
                                entity.MaxAge = item.Item4;
                                break;
                            case 207:
                                entity.MinAge = item.Item4;
                                break;
                            case 208:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 209:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 210:
                                entity.Scale = item.Item4;
                                break;
                            case 211:
                                entity.Score = item.Item4;
                                break;
                            case 212:
                                entity.Material = item.Item4;
                                break;
                            case 213:

                                break;
                            case 214:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }
                //自主招生-深圳公民办初中专用 215-226
                recruitDatas = row.Where(p => p.Item1 >= 215 && p.Item1 <= 226);
                if (recruitDatas?.Any() == true)
                {
                    var entity = new OnlineSchoolRecruitInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = 6,
                        Year = 2021,
                        EID = eid
                    };
                    foreach (var item in recruitDatas)
                    {
                        switch (item.Item1)
                        {
                            case 215:
                                entity.Requirement = item.Item4;
                                break;
                            case 216:
                                entity.Target = item.Item4;
                                break;
                            case 217:
                                entity.MaxAge = item.Item4;
                                break;
                            case 218:
                                entity.MinAge = item.Item4;
                                break;
                            case 219:
                                if (int.TryParse(item.Item4, out int quantity)) entity.Quantity = quantity;
                                break;
                            case 220:
                                if (int.TryParse(item.Item4, out int classQuantity)) entity.ClassQuantity = classQuantity;
                                break;
                            case 221:
                                entity.Scale = item.Item4;
                                break;
                            case 222:
                                entity.Score = item.Item4;
                                break;
                            case 223:
                                if (int.TryParse(item.Item4, out int zkfskzx)) entity.ZKFSKZX = zkfskzx;
                                break;
                            case 224:
                                entity.Material = item.Item4;
                                break;
                            case 225:
                                //entity.Brief = item.Item4;
                                break;
                            case 226:
                                entity.Brief = item.Item4;
                                break;
                        }
                    }
                    insertRecruits.Add(entity);
                }

                #endregion

                #region 其他信息 #295-300
                var entity_SchoolOverView = await _schoolOverViewService.GetByEID(eid);
                if (entity_SchoolOverView == null || entity_SchoolOverView.ID == Guid.Empty) entity_SchoolOverView = new SchoolOverViewInfo();
                if (entity_SchoolOverView.EID != default)
                {
                    entity_SchoolOverView.OAName = entity_SchoolOverView.OAAccount = entity_SchoolOverView.MPName = entity_SchoolOverView.MPAppID = entity_SchoolOverView.VANAme = entity_SchoolOverView.VAAppID = null;
                }
                var otherInfos = row.Where(p => p.Item1 >= 295 && p.Item1 <= 300);
                if (otherInfos?.Any() == true)
                {
                    if (entity_SchoolOverView.EID == Guid.Empty) { entity_SchoolOverView.EID = eid; }
                    else
                    {
                        entity_SchoolOverView.OAName = entity_SchoolOverView.OAAccount = entity_SchoolOverView.MPName = entity_SchoolOverView.MPAppID = entity_SchoolOverView.VANAme = entity_SchoolOverView.VAAppID = null;
                    }
                    foreach (var info in otherInfos)
                    {
                        switch (info.Item1)
                        {
                            case 295:
                                entity_SchoolOverView.OAName = info.Item4;
                                break;
                            case 296:
                                entity_SchoolOverView.OAAccount = info.Item4;
                                break;
                            case 297:
                                entity_SchoolOverView.MPName = info.Item4;
                                break;
                            case 298:
                                entity_SchoolOverView.MPAppID = info.Item4;
                                break;
                            case 299:
                                entity_SchoolOverView.VANAme = info.Item4;
                                break;
                            case 300:
                                entity_SchoolOverView.VAAppID = info.Item4;
                                break;
                        }
                    }
                }
                #region 招生方式 #17
                var recruitWay = row.FirstOrDefault(p => p.Item1 == 17);
                if (!string.IsNullOrWhiteSpace(recruitWay.Item4))
                {
                    if (entity_SchoolOverView.EID == Guid.Empty) entity_SchoolOverView.EID = eid;
                    entity_SchoolOverView.RecruitWay = Newtonsoft.Json.JsonConvert.SerializeObject(recruitWay.Item4.Split('；', StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    entity_SchoolOverView.RecruitWay = null;
                }
                #endregion
                if (entity_SchoolOverView.EID != Guid.Empty) insertSchoolOverviews.Add(entity_SchoolOverView);
                #endregion

                #region 费用相关 #288-291
                var costs = row.Where(p => p.Item1 >= 288 && p.Item1 <= 291);
                if (costs?.Any() == true)
                {
                    if (insertRecruits.Any(p => p.EID == eid))
                    {
                        var recruit = insertRecruits.Where(p => p.EID == eid).OrderBy(p => p.Type).First();
                        var otherCostKeys = string.Empty;
                        var otherCostValues = string.Empty;
                        foreach (var cost in costs)
                        {
                            switch (cost.Item1)
                            {
                                case 288:
                                    recruit.ApplyCost = cost.Item4;
                                    break;
                                case 289:
                                    recruit.Tuition = cost.Item4;
                                    break;
                                case 290:
                                    otherCostKeys = cost.Item4;
                                    break;
                                case 291:
                                    otherCostValues = cost.Item4;
                                    break;
                            }
                        }
                        if (otherCostKeys.Contains('；'))
                        {
                            var keys = otherCostKeys.Split('；');
                            var values = otherCostValues.Split('；');
                            if (keys.Length != values.Length) { errorEIDs.Add(eid); continue; }
                            var otherCostData = new List<IEnumerable<string>>();
                            for (int i = 0; i < keys.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(keys[i])) otherCostData.Add(new string[] { keys[i], values[i] });
                            }
                            recruit.OtherCost = Newtonsoft.Json.JsonConvert.SerializeObject(otherCostData);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(otherCostKeys))
                            {
                                recruit.OtherCost = Newtonsoft.Json.JsonConvert.SerializeObject(new List<IEnumerable<string>>()
                                {
                                    new string[]{ otherCostKeys,otherCostValues}
                                });
                            }
                        }
                    }
                }
                #endregion
            }

            #region 插入招生信息
            var insertRecruitCount = 0;
            if (insertRecruits?.Any() == true)
            {
                if (errorEIDs?.Any() == true)
                {
                    insertRecruits.RemoveAll(p => errorEIDs.Contains(p.EID));
                }
                foreach (var item in insertRecruits)
                {
                    if (await _onlineSchoolRecruitService.AddAsync(item)) insertRecruitCount++;
                }
            }
            #endregion

            #region 插入学部其他信息
            var insertSchoolOverviewCount = 0;
            var updateSchoolOverviewCount = 0;
            if (insertSchoolOverviews?.Any() == true)
            {
                foreach (var item in insertSchoolOverviews)
                {
                    if (item.ID == Guid.Empty)
                    {
                        item.ID = Guid.NewGuid();
                        if (await _schoolOverViewService.AddAsync(item)) insertSchoolOverviewCount++;
                    }
                    else
                    {
                        if (await _schoolOverViewService.UpdateAsync(item)) updateSchoolOverviewCount++;
                    }
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertRecruitCount,
                insertSchoolOverviewCount,
                updateSchoolOverviewCount,
                errorEIDs = Newtonsoft.Json.JsonConvert.SerializeObject(errorEIDs),
                EIDs = Newtonsoft.Json.JsonConvert.SerializeObject(insertRecruits.Select(p => p.EID).Distinct())
            });


            return Json(result);
        }

        /// <summary>
        /// 导入招生日程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadRecruitSchedules()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int, string, string)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);


            var cities = await _cityInfoService.GetAllCityCodes();
            var cityCode = cities.FirstOrDefault(p => p.Name.Contains(currentSheet.SheetName))?.Id;
            var areas = await _cityInfoService.GetAreaCode(cityCode.Value);

            var insertRecruitSchedules = new List<RecruitScheduleInfo>();

            foreach (var row in datas)
            {
                var entity = new RecruitScheduleInfo()
                {
                    ID = Guid.NewGuid()
                };
                foreach (var cell in row)
                {
                    switch (cell.Item1)
                    {
                        case 0:
                            var city = cities.FirstOrDefault(p => p.Name.Contains(cell.Item3));
                            if (city != null) entity.CityCode = city.Id;
                            break;
                        case 1:
                            entity.SchFType = SchFTypeUtil.GetCode(cell.Item3);
                            break;
                        case 2:
                            var area = areas.FirstOrDefault(p => p.AreaName.Contains(cell.Item3.Replace("区", "")));
                            if (area != null) entity.AreaCode = area.AdCode;
                            if (cell.Item3.Contains("大鹏") && entity.CityCode == 440300) entity.AreaCode = 440312;
                            break;
                        case 3:
                            if (int.TryParse(cell.Item3, out int recruitType)) entity.RecruitType = recruitType;
                            break;
                        case 4:
                            entity.StrDate = cell.Item3.Replace("\"", "");
                            break;
                        case 5:
                            entity.Content = cell.Item3;
                            break;
                        case 6:
                            if (int.TryParse(cell.Item3, out int index)) entity.Index = index;
                            break;
                    }
                }
                await _onlineSchoolRecruitService.RemoveRecruitSchedules(entity.CityCode?.ToString(), entity.AreaCode?.ToString(), entity.SchFType, entity.RecruitType ?? -1);
                insertRecruitSchedules.Add(entity);
            }

            #region InsertToDB
            var insertRecruitScheduleCount = 0;
            if (insertRecruitSchedules.Any())
            {
                foreach (var item in insertRecruitSchedules)
                {
                    if (await _onlineSchoolRecruitService.InsertRecruitSchedule(item)) insertRecruitScheduleCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertRecruitScheduleCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入指标分配
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadQuotas()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(1);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i)?.StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string TitleName, string Value)>>();
            for (int i = 2; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 1; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var targetEIDs = titleNames.Where(p => p.Key > 4).Select(p =>
            {
                if (Guid.TryParse(p.Value, out Guid eid)) return eid;
                return Guid.Empty;
            });
            targetEIDs = targetEIDs.Where(p => p != Guid.Empty);
            var targetSchoolInfos = await _schoolService.SearchSchools(targetEIDs);

            var insertQuotas = new List<OnlineSchoolQuotaInfo>();

            foreach (var row in datas)
            {

                var entity = new OnlineSchoolQuotaInfo()
                {
                    ID = Guid.NewGuid(),
                    Type = 1,
                    Year = 2021
                };
                var schoolData = new List<string[]>();
                foreach (var cell in row)
                {
                    switch (cell.Item1)
                    {

                        case 1:
                            if (Guid.TryParse(cell.Item3, out Guid eid))
                            {
                                await _onlineSchoolQuotaService.RemoveByEID(eid, entity.Type);
                                entity.EID = eid;
                            }
                            break;
                        case 2:
                            if (int.TryParse(cell.Item3, out int examineeQuantity)) entity.ExamineeQuantity = examineeQuantity;
                            break;
                        case 3:
                            if (int.TryParse(cell.Item3, out int provinceCityQuantity)) entity.ProvinceCityQuantity = provinceCityQuantity;
                            break;
                        case 4:
                            if (int.TryParse(cell.Item3, out int areaQuantity)) entity.AreaQuantity = areaQuantity;
                            break;

                        default:
                            if (Guid.TryParse(cell.Item2, out Guid targetEID))
                            {
                                if (targetSchoolInfos.Any(p => p.ExtId == targetEID))
                                {
                                    schoolData.Add(new string[] { targetSchoolInfos.FirstOrDefault(p => p.ExtId == targetEID).Name, cell.Item3, cell.Item2 });
                                }
                            }

                            break;
                    }
                }
                entity.SchoolData = Newtonsoft.Json.JsonConvert.SerializeObject(schoolData);
                insertQuotas.Add(entity);
            }

            #region InsertToDB
            var insertQuotasCount = 0;
            if (insertQuotas.Any())
            {
                foreach (var quota in insertQuotas)
                {
                    if (await _onlineSchoolQuotaService.AddAsync(quota)) insertQuotasCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertQuotasCount
            });


            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> UploadType2Quotas()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string TitleName, string Value)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 1; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var insertQuotas = new List<OnlineSchoolQuotaInfo>();

            foreach (var row in datas)
            {

                var entity = new OnlineSchoolQuotaInfo()
                {
                    ID = Guid.NewGuid(),
                    Type = 2,
                    Year = 2021
                };
                foreach (var cell in row)
                {
                    switch (cell.Index)
                    {

                        case 1:
                            if (Guid.TryParse(cell.Value, out Guid eid))
                            {
                                await _onlineSchoolQuotaService.RemoveByEID(eid, entity.Type);
                                entity.EID = eid;
                            }
                            break;
                        case 2:
                            entity.CoreSchool = cell.Value;
                            break;
                        case 3:
                            if (int.TryParse(cell.Value, out int examineeQuantity)) entity.ExamineeQuantity = examineeQuantity;
                            break;
                        case 4:
                            if (int.TryParse(cell.Value, out int coreQuantity)) entity.CoreQuantity = coreQuantity;
                            break;
                    }
                }
                if (entity.EID == Guid.Empty) continue;
                insertQuotas.Add(entity);
            }

            #region InsertToDB
            var insertQuotasCount = 0;
            if (insertQuotas.Any())
            {
                foreach (var quota in insertQuotas)
                {
                    if (await _onlineSchoolQuotaService.AddAsync(quota)) insertQuotasCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertQuotasCount
            });


            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> UploadType3Quotas()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(1);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i)?.StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string TitleName, string Value)>>();
            for (int i = 2; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 1; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var targetEIDs = titleNames.Where(p => p.Key > 1).Select(p =>
            {
                if (Guid.TryParse(p.Value, out Guid eid)) return eid;
                return Guid.Empty;
            });
            targetEIDs = targetEIDs.Where(p => p != Guid.Empty);
            var targetSchoolInfos = await _schoolService.SearchSchools(targetEIDs);

            var insertQuotas = new List<OnlineSchoolQuotaInfo>();

            foreach (var row in datas)
            {

                var entity = new OnlineSchoolQuotaInfo()
                {
                    ID = Guid.NewGuid(),
                    Type = 3,
                    Year = 2021
                };
                var schoolData = new List<string[]>();
                foreach (var cell in row)
                {
                    switch (cell.Index)
                    {

                        case 1:
                            if (Guid.TryParse(cell.Value, out Guid eid))
                            {
                                await _onlineSchoolQuotaService.RemoveByEID(eid, entity.Type);
                                entity.EID = eid;
                            }
                            break;
                        default:
                            if (Guid.TryParse(cell.TitleName, out Guid targetEID))
                            {
                                if (targetSchoolInfos.Any(p => p.ExtId == targetEID))
                                {
                                    schoolData.Add(new string[] { targetSchoolInfos.FirstOrDefault(p => p.ExtId == targetEID).Name, cell.Value, cell.TitleName });
                                }
                            }

                            break;
                    }
                }
                if (entity.EID == Guid.Empty) continue;
                entity.SchoolData = Newtonsoft.Json.JsonConvert.SerializeObject(schoolData);
                insertQuotas.Add(entity);
            }

            #region InsertToDB
            var insertQuotasCount = 0;
            if (insertQuotas.Any())
            {
                foreach (var quota in insertQuotas)
                {
                    if (await _onlineSchoolQuotaService.AddAsync(quota)) insertQuotasCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertQuotasCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入分数线
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFractions()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int, string, string)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 1; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var insertFractions = new List<OnlineSchoolFractionInfo>();

            foreach (var row in datas)
            {
                var entity = new OnlineSchoolFractionInfo()
                {
                    ID = Guid.NewGuid(),
                };
                foreach (var cell in row.OrderBy(p => p.Item1))
                {
                    if (entity.ID == default) break;
                    switch (cell.Item1)
                    {

                        case 1:
                            if (Guid.TryParse(cell.Item3, out Guid eid))
                            {
                                entity.EID = eid;
                            }
                            else
                            {
                                entity.ID = default;
                            }
                            break;
                        case 2:
                            if (int.TryParse(cell.Item3, out int year))
                            {
                                if (entity.EID != default) await _onlineSchoolFractionService.RemoveByEIDYear(entity.EID, year);
                                entity.Year = year;
                            }
                            else
                            {
                                entity.ID = default;
                            }
                            break;
                        case 3:
                            if (int.TryParse(cell.Item3, out int batchType)) { entity.BatchType = batchType; }
                            else
                            {
                                entity.ID = default;
                            }
                            break;
                        case 4:
                            entity.Area = cell.Item3;
                            break;
                        case 5:
                            entity.StudentType = cell.Item3;
                            break;
                        case 6:
                            if (int.TryParse(cell.Item3, out int fraction)) { entity.Fraction = fraction; }
                            //else { entity.Fraction = -1; }
                            break;
                        case 7:
                            if (int.TryParse(cell.Item3, out int lowestFraction)) entity.LowestFraction = lowestFraction;
                            break;
                    }
                }

                if (entity.ID != default) insertFractions.Add(entity);
            }

            #region InsertToDB
            var insertFractionsCount = 0;
            if (insertFractions.Any())
            {
                foreach (var item in insertFractions)
                {
                    if (await _onlineSchoolFractionService.AddAsync(item)) insertFractionsCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertFractionsCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入区域招生政策
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAreaRecruitPlans()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string Tilte, string Value)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    var titleName = string.Empty;
                    titleNames.TryGetValue(o, out titleName);
                    rowData.Add((o, titleName, cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var cityName = currentSheet.SheetName?.Trim();
            var cities = await _cityInfoService.GetAllCityCodes();
            var cityCode = cities.FirstOrDefault(p => p.Name.Contains(cityName))?.Id;
            var areas = await _cityInfoService.GetAreaCode(cityCode.Value);

            var tempAreaRecruits = new List<AreaRecruitPlanInfo>();
            var dic_PM = new Dictionary<string, string>();//积分办法

            foreach (var row in datas)
            {
                var entity = new AreaRecruitPlanInfo();
                var urlData = new string[2];
                foreach (var cell in row)
                {
                    switch (cell.Index)
                    {
                        case 0:
                            var schFTypeCode = SchFTypeUtil.GetCode(cell.Value);
                            if (!string.IsNullOrWhiteSpace(schFTypeCode)) { entity.SchFType = schFTypeCode; }
                            else
                            {
                                return Content($"Invalid school type -> {cell.Value}");
                            }
                            break;
                        case 1:
                            var areaCode = areas.FirstOrDefault(p => p.AreaName == cell.Value.Replace("区", ""))?.AdCode;
                            if (areaCode.HasValue) { entity.AreaCode = areaCode.Value.ToString(); }
                            else
                            {
                                entity.AreaCode = cityCode.ToString();
                            }
                            if (cell.Item3.Contains("大鹏") && cityCode == 440300) entity.AreaCode = "440312";
                            break;
                        case 2:
                            if (int.TryParse(cell.Value, out int year)) entity.Year = year;
                            break;
                        case 3:
                            urlData[0] = cell.Value;
                            break;
                        case 4:
                            urlData[1] = cell.Value;
                            break;
                        case 5:
                            if (!dic_PM.ContainsKey($"{entity.SchFType}{entity.AreaCode}{entity.Year}")) dic_PM.Add($"{entity.SchFType}{entity.AreaCode}{entity.Year}", cell.Value);
                            break;
                    }
                }
                entity.UrlData = Newtonsoft.Json.JsonConvert.SerializeObject(urlData);
                await _areaRecruitPlanService.RemoveByAreaYearSchFType(entity.SchFType, entity.AreaCode, entity.Year);
                tempAreaRecruits.Add(entity);
            }

            #region InsertToDB
            var insertAreaRecruitPlanCount = 0;
            var insertAreaRecruits = new List<AreaRecruitPlanInfo>();
            if (tempAreaRecruits.Any())
            {
                foreach (var item in tempAreaRecruits.Select(p => p.AreaCode).Distinct())
                {
                    foreach (var item1 in tempAreaRecruits.Where(p => p.AreaCode == item).Select(p => p.SchFType).Distinct())
                    {
                        foreach (var item2 in tempAreaRecruits.Where(p => p.SchFType == item1 && p.AreaCode == item).Select(p => p.Year).Distinct())
                        {
                            insertAreaRecruits.Add(new AreaRecruitPlanInfo()
                            {
                                ID = Guid.NewGuid(),
                                Year = item2,
                                AreaCode = item,
                                SchFType = item1,
                                UrlData = Newtonsoft.Json.JsonConvert.SerializeObject(tempAreaRecruits.Where(p => p.AreaCode == item && p.Year == item2 && p.SchFType == item1).Select(p => p.UrlData_Obj).Distinct()),
                                PointMethod = dic_PM.TryGetValue($"{item1}{item}{item2}", out string str_PM) ? str_PM : null
                            });
                        }
                    }
                }
                if (insertAreaRecruits?.Any() == true)
                {
                    foreach (var item in insertAreaRecruits)
                    {
                        if (!string.IsNullOrWhiteSpace(item.PointMethod)) item.PointMethod = item.PointMethod.Replace("[http", "<img src=\"http").Replace("jpg]", "jpg\" />");
                        if (await _areaRecruitPlanService.AddAsync(item)) insertAreaRecruitPlanCount++;
                    }
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertAreaRecruitPlanCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入开设的课程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadSchoolProjects()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string Tilte, string Value)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var insertProjects = new List<OnlineSchoolProjectInfo>();

            foreach (var row in datas)
            {
                var entity = new OnlineSchoolProjectInfo()
                {
                    ID = Guid.NewGuid()
                };
                foreach (var cell in row)
                {
                    switch (cell.Index)
                    {
                        case 1:
                            if (Guid.TryParse(cell.Value, out Guid eid)) await _onlineSchoolProjectService.RemoveByEID(eid); entity.EID = eid;
                            break;
                        case 2:
                            entity.Name = cell.Value;
                            break;
                        case 3:
                            entity.ItemJson = $"[{cell.Value}]";
                            break;
                    }
                }
                insertProjects.Add(entity);
            }

            #region InsertToDB
            var insertProjectCount = 0;
            if (insertProjects?.Any() == true)
            {
                foreach (var item in insertProjects)
                {
                    if (await _onlineSchoolProjectService.AddAsync(item)) insertProjectCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                insertProjectCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入学校认证
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadSchoolCertifications()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string Tilte, string Value)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var modifyCertifications = new List<SchoolOverViewInfo>();

            foreach (var row in datas)
            {
                var entity = new SchoolOverViewInfo();
                foreach (var cell in row)
                {
                    switch (cell.Index)
                    {
                        case 1:
                            if (Guid.TryParse(cell.Value, out Guid eid)) await _onlineSchoolProjectService.RemoveByEID(eid); entity.EID = eid;
                            break;
                        case 2:
                            entity.Certifications = $"[{cell.Value}]";
                            break;
                    }
                }
                modifyCertifications.Add(entity);
            }

            #region InsertToDB
            var modifyCount = 0;
            if (modifyCertifications?.Any() == true)
            {
                var count = modifyCertifications.Select(p => p.EID).Distinct();
                var asd = modifyCertifications.GroupBy(p => p.EID).Where(p => p.Count() > 1);
                foreach (var item in modifyCertifications)
                {
                    if (await _schoolOverViewService.ModifySchoolCertifications(item.EID, item.Certifications)) modifyCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                modifyCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入升学成绩
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAchievements()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);

            var groupNames = new List<KeyValuePair<string, (int, int)>>();
            var groupRow = currentSheet.GetRow(0);
            foreach (var merge in currentSheet.MergedRegions)
            {
                var cell = groupRow.GetCell(merge.FirstColumn);
                groupNames.Add(new KeyValuePair<string, (int, int)>(cell.StringCellValue, (merge.FirstColumn, merge.LastColumn)));
            }
            if (groupNames.Any()) groupNames = groupNames.OrderBy(p => p.Value.Item1).ToList();

            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(1);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string GroupName, string Tilte, string Value)>>();
            for (int i = 2; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, groupNames.FirstOrDefault(p => p.Value.Item2 >= o && p.Value.Item1 <= o).Key, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var insertAchievements = new List<OnlineSchoolAchievementInfo>();

            foreach (var row in datas)
            {
                if (Guid.TryParse(row.FirstOrDefault(p => p.Index == 0).Value, out Guid eid))
                {
                    if (eid == Guid.Empty) continue;
                    if (int.TryParse(row.FirstOrDefault(p => p.Index == 1).Value, out int year))
                    {
                        await _onlineSchoolAchievementService.RemoveByEIDYear(eid, year);
                        #region 升学成绩-初中
                        var cells = row.Where(p => p.Index >= 2 && p.Index <= 17);
                        if (cells?.Any() == true)
                        {
                            var entity = new OnlineSchoolAchievementInfo()
                            {
                                ID = Guid.NewGuid(),
                                EID = eid,
                                Year = year
                            };
                            foreach (var cell in cells)
                            {
                                switch (cell.Index)
                                {
                                    case 2:
                                        if (int.TryParse(cell.Value, out int avg)) entity.Avg = avg;
                                        break;
                                    case 3:
                                        if (int.TryParse(cell.Value, out int max)) entity.Max = max;
                                        break;
                                    case 4:
                                        break;
                                    case 5:
                                        if (int.TryParse(cell.Value, out int countHigh)) entity.CountHigh = countHigh;
                                        break;
                                    case 6:
                                        if (decimal.TryParse(cell.Value, out decimal percentHigh)) entity.PercentHigh = $"{(percentHigh * 100).ToString("F2")}%";
                                        break;
                                    case 7:
                                        if (int.TryParse(cell.Value, out int countAdvance)) entity.CountAdvance = countAdvance;
                                        break;
                                    case 8:
                                        if (decimal.TryParse(cell.Value, out decimal percentAdvance)) entity.PercentAdvance = $"{(percentAdvance * 100).ToString("F2")}%";
                                        break;
                                    case 9:

                                        break;
                                    case 10:

                                        break;
                                    case 11:
                                        if (decimal.TryParse(cell.Value, out decimal percentZD)) entity.PercentZD = $"{(percentZD * 100).ToString("F2")}%";
                                        break;
                                    case 12:
                                        if (int.TryParse(cell.Value, out int count700)) entity.Count700 = count700;
                                        break;
                                    case 13:
                                        if (decimal.TryParse(cell.Value, out decimal percent700)) entity.Percent700 = $"{(percent700 * 100).ToString("F2")}%";
                                        break;
                                    case 14:
                                        if (int.TryParse(cell.Value, out int countLZ)) entity.CountLZ = countLZ;
                                        break;
                                    case 15:
                                        if (decimal.TryParse(cell.Value, out decimal percentLZ)) entity.PercentLZ = $"{(percentLZ * 100).ToString("F2")}%";
                                        break;
                                    case 16:
                                        entity.News = cell.Value;
                                        break;
                                    case 17:
                                        var dic_Forward = new Dictionary<int, object>();
                                        var array_Split = cell.Value.Split("|");
                                        if (array_Split?.Any() == true)
                                        {
                                            if (array_Split.First() != "[]") dic_Forward.Add(1, Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"[{array_Split.First()}]"));
                                            if (array_Split.Length > 1)
                                            {
                                                if (array_Split.Last() != "[]") dic_Forward.Add(2, Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"[{array_Split.Last()}]"));
                                            }
                                        }
                                        if (dic_Forward?.Any() == true) entity.Forward = $"[{Newtonsoft.Json.JsonConvert.SerializeObject(dic_Forward)}]";
                                        break;
                                }
                            }
                            insertAchievements.Add(entity);
                        }
                        #endregion

                        #region 升学成绩-高中
                        cells = row.Where(p => p.Index >= 18 && p.Index <= 25);
                        if (cells?.Any() == true)
                        {
                            var entity = new OnlineSchoolAchievementInfo()
                            {
                                ID = Guid.NewGuid(),
                                EID = eid,
                                Year = year
                            };
                            foreach (var cell in cells)
                            {
                                switch (cell.Index)
                                {
                                    case 18:
                                        if (decimal.TryParse(cell.Value, out decimal percentGY)) entity.PercentGY = $"{(percentGY * 100).ToString("F2")}%";
                                        break;
                                    case 19:
                                        if (decimal.TryParse(cell.Value, out decimal percentZB)) entity.PercentZB = $"{(percentZB * 100).ToString("F2")}%";
                                        break;
                                    case 20:
                                        if (decimal.TryParse(cell.Value, out decimal percentBK)) entity.PercentBK = $"{(percentBK * 100).ToString("F2")}%";
                                        break;
                                    case 21:
                                        if (int.TryParse(cell.Value, out int maxWK)) entity.MaxWK = maxWK;
                                        break;
                                    case 22:
                                        if (int.TryParse(cell.Value, out int maxLK)) entity.MaxLK = maxLK;
                                        break;
                                    case 23:
                                        if (decimal.TryParse(cell.Value, out decimal percentYB)) entity.PercentYB = $"{(percentYB * 100).ToString("F2")}%";
                                        break;
                                    case 24:
                                        entity.News = cell.Value;
                                        break;
                                    case 25:
                                        var dic_Forward = new Dictionary<int, object>();
                                        var array_Split = cell.Value.Split("|");
                                        if (array_Split?.Any() == true)
                                        {
                                            if (array_Split.First() != "[]") dic_Forward.Add(1, Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"[{array_Split.First()}]"));
                                            if (array_Split.Length > 1)
                                            {
                                                if (array_Split.Last() != "[]") dic_Forward.Add(2, Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>($"[{array_Split.Last()}]"));
                                            }
                                        }
                                        if (dic_Forward?.Any() == true) entity.Forward = $"[{Newtonsoft.Json.JsonConvert.SerializeObject(dic_Forward)}]";
                                        break;
                                }
                            }
                            insertAchievements.Add(entity);
                        }
                        #endregion
                    }
                }

            }

            #region InsertToDB
            var modifyCount = 0;
            if (insertAchievements?.Any() == true)
            {
                foreach (var item in insertAchievements)
                {
                    if (await _onlineSchoolAchievementService.AddAsync(item)) modifyCount++;
                }
            }
            #endregion

            result = ResponseResult.Success(new
            {
                modifyCount
            });


            return Json(result);
        }

        /// <summary>
        /// 导入分数线2
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFractions2()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);

            var groupNames = new List<KeyValuePair<string, (int, int)>>();
            var groupRow = currentSheet.GetRow(0);
            foreach (var merge in currentSheet.MergedRegions)
            {
                var cell = groupRow.GetCell(merge.FirstColumn);
                groupNames.Add(new KeyValuePair<string, (int, int)>(cell.StringCellValue, (merge.FirstColumn, merge.LastColumn)));
            }
            if (groupNames.Any()) groupNames = groupNames.OrderBy(p => p.Value.Item1).ToList();
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(1);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string GroupName, string TitleName, string Value)>>();
            for (int i = 2; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int Index, string GroupName, string TitleName, string Value)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) cellValue = null;
                    rowData.Add((o, groupNames.FirstOrDefault(p => p.Value.Item2 >= o && p.Value.Item1 <= o).Key,
                        titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }

            if (!datas.Any()) return Json(result);

            var insertFractions = new List<SchoolFractionInfo2>();

            foreach (var row in datas)
            {
                var eid = Guid.Empty;
                var year = 0;

                #region 基础数据
                var cells = row.Where(p => p.Index <= 1);
                if (cells?.Any() == true)
                {
                    foreach (var cell in cells)
                    {
                        switch (cell.Index)
                        {

                            case 0:
                                if (!Guid.TryParse(cell.Value, out eid)) { continue; }
                                break;
                            case 1:
                                if (!int.TryParse(cell.Value, out year)) { continue; }
                                break;
                        }
                    }
                }
                if (eid == Guid.Empty || year < 1) { continue; } else { await _onlineSchoolFractionService.Remove2AsyncByEIDYear(eid, year); }
                #endregion

                #region 中考分数线（深圳）
                cells = row.Where(p => p.Index >= 2 && p.Index <= 3);
                if (cells?.Any() == true)
                {
                    var entity = new SchoolFractionInfo2()
                    {
                        ID = Guid.NewGuid(),
                        EID = eid,
                        Year = year,
                        Type = 1
                    };
                    var items = new List<string[]>();

                    foreach (var cell in cells)
                    {
                        switch (cell.Index)
                        {
                            case 2:
                                items.Add(new string[] { "AC类", cell.Value });
                                break;
                            case 3:
                                items.Add(new string[] { "D类", cell.Value });
                                break;
                        }
                    }
                    if (items.Any())
                    {
                        entity.Data = Newtonsoft.Json.JsonConvert.SerializeObject(items);
                        insertFractions.Add(entity);
                    }
                }
                #endregion

                #region 中考录取分数线（成都）
                cells = row.Where(p => p.Index >= 4 && p.Index <= 9);
                if (cells?.Any() == true)
                {
                    var entity = new SchoolFractionInfo2()
                    {
                        ID = Guid.NewGuid(),
                        EID = eid,
                        Year = year,
                        Type = 2
                    };
                    var dic = new Dictionary<string, dynamic>();
                    var items = new List<IEnumerable<string>>();
                    var array52 = new List<string>() { "\"5+2\"区域(统招)" };
                    var arrayOther = new List<string>() { "其余15个区(市)县" };
                    foreach (var cell in cells)
                    {
                        switch (cell.Index)
                        {
                            case 4:
                                dic.Add("TZ", cell.Value);
                                break;
                            case 5:
                                dic.Add("TJ", cell.Value);
                                break;
                            case 6:
                            case 7:
                                array52.Add(cell.Value);
                                break;
                            case 8:
                            case 9:
                                arrayOther.Add(cell.Value);
                                break;
                        }
                    }
                    if (array52.Count() > 1) items.Add(array52);
                    if (arrayOther.Count() > 1) items.Add(arrayOther);
                    if (items?.Any() == true) dic.Add("Items", items);
                    if (dic.Any())
                    {
                        entity.Data = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                        insertFractions.Add(entity);
                    }
                }
                #endregion

                #region 分数线（重庆）
                cells = row.Where(p => p.Index >= 10 && p.Index <= 12);
                if (cells?.Any() == true)
                {
                    var entity = new SchoolFractionInfo2()
                    {
                        ID = Guid.NewGuid(),
                        EID = eid,
                        Year = year,
                        Type = 3
                    };
                    var dic = new Dictionary<string, dynamic>();
                    var dicLZ = new Dictionary<string, dynamic>();
                    var dicFLZ = new Dictionary<string, dynamic>();
                    foreach (var cell in cells)
                    {
                        switch (cell.Index)
                        {
                            case 10:
                                dicLZ.Add("LZ", cell.Value);
                                break;
                            case 11:
                                dicLZ.Add("LQ", cell.Value);
                                break;
                            case 12:
                                dicFLZ.Add("LQ", cell.Value);
                                break;
                        }
                    }
                    if (dicLZ.Any()) dic.Add("LZ", new List<dynamic>() { dicLZ });
                    if (dicFLZ.Any()) dic.Add("FLZ", new List<dynamic>() { dicFLZ });
                    if (dic.Any())
                    {
                        entity.Data = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                        insertFractions.Add(entity);
                    }
                }
                #endregion
            }
            #region InsertToDB
            var insertFractionsCount = 0;
            if (insertFractions.Any())
            {
                foreach (var item in insertFractions)
                {
                    if (await _onlineSchoolFractionService.Add2Async(item)) insertFractionsCount++;
                }

                result = ResponseResult.Success(new
                {
                    insertFractionsCount
                });
            }
            #endregion

            return Json(result);
        }

        /// <summary>
        /// 导入招生信息的划片范围(EID格式)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadScribingScopeEID()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            #region Load datas
            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string TitleName, string Value)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 1; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }
            #endregion

            #region Process datas
            if (!datas.Any()) return Json(result);

            var modifyEntities = new List<OnlineSchoolRecruitInfo>();
            var notExistedEIDs = new List<(Guid EID, int Year, int Type)>();
            foreach (var row in datas)
            {
                var entity = new OnlineSchoolRecruitInfo();
                foreach (var cell in row)
                {
                    switch (cell.Index)
                    {
                        case 1:
                            if (Guid.TryParse(cell.Value, out Guid eid)) entity.EID = eid;
                            break;
                        case 2:
                            if (int.TryParse(cell.Value, out int year)) entity.Year = year;
                            break;
                        case 3:
                            if (int.TryParse(cell.Value, out int type)) entity.Type = type;
                            break;
                        case 4:
                            entity.ScribingScopeEIDs = cell.Value;
                            break;
                    }
                }
                if (notExistedEIDs.Any(p => p.EID == entity.EID && p.Year == entity.Year && p.Type == entity.Type)) continue;
                if (modifyEntities.Any(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year))
                {
                    var find = modifyEntities.FirstOrDefault(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year);
                    if (find == null || find.ID == Guid.Empty) continue;
                    var scribingScopeEIDs = new List<Guid>();
                    if (find.ScribingScopeEIDs_Obj != null) scribingScopeEIDs.AddRange(find.ScribingScopeEIDs_Obj);
                    if (!scribingScopeEIDs.Any(p => p == new Guid(entity.ScribingScopeEIDs)))
                    {
                        scribingScopeEIDs.Add(new Guid(entity.ScribingScopeEIDs));
                        find.ScribingScopeEIDs = Newtonsoft.Json.JsonConvert.SerializeObject(scribingScopeEIDs);
                    }
                }
                else
                {
                    var finds = await _onlineSchoolRecruitService.GetByEID(entity.EID, entity.Year, entity.Type);
                    if (finds?.Any() == true)
                    {
                        var find = finds.First();
                        find.ScribingScopeEIDs = Newtonsoft.Json.JsonConvert.SerializeObject(new string[] { entity.ScribingScopeEIDs });
                        modifyEntities.Add(find);
                    }
                    else
                    {
                        notExistedEIDs.Add((entity.EID, entity.Year, entity.Type));
                    }
                }
            }
            #endregion

            #region Save datas
            var modifyCount = 0;
            if (modifyEntities.Any())
            {
                foreach (var item in modifyEntities)
                {
                    if (await _onlineSchoolRecruitService.UpdateAsync(item)) modifyCount++;
                }
                result = ResponseResult.Success(new { modifyCount, notExistedEIDs });
            }
            else
            {
                result = ResponseResult.Failed(notExistedEIDs);
            }
            #endregion

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> UploadSchoolExtLevels()
        {
            var result = ResponseResult.Failed();
            var file = Request.Form.Files.First();

            #region Load datas
            var excelFile = new XSSFWorkbook(file.OpenReadStream());
            var currentSheet = excelFile.GetSheetAt(0);
            var titleNames = new Dictionary<int, string>();
            var titleRow = currentSheet.GetRow(0);
            for (int i = 0; i < titleRow.Cells.Count(); i++)
            {
                titleNames.Add(i, titleRow.GetCell(i).StringCellValue);
            }

            var datas = new List<IEnumerable<(int Index, string TitleName, string Value)>>();
            for (int i = 1; i <= currentSheet.LastRowNum; i++)
            {
                var row = currentSheet.GetRow(i);
                if (row == null) continue;
                var rowData = new List<(int, string, string)>();
                for (int o = 0; o <= row.LastCellNum; o++)
                {
                    var cell = row.GetCell(o, NPOI.SS.UserModel.MissingCellPolicy.RETURN_BLANK_AS_NULL);
                    if (cell == null) continue;
                    var cellValue = string.Empty;
                    switch (cell.CellType)
                    {
                        case NPOI.SS.UserModel.CellType.Numeric:
                            cellValue = cell.NumericCellValue.ToString();
                            break;
                        case NPOI.SS.UserModel.CellType.String:
                        case NPOI.SS.UserModel.CellType.Blank:
                            cellValue = cell.StringCellValue;
                            break;
                    }
                    if (string.IsNullOrWhiteSpace(cellValue)) { cellValue = null; }

                    rowData.Add((o, titleNames[o], cellValue));
                }
                datas.Add(rowData);
            }
            #endregion

            #region Process datas
            if (!datas.Any()) return Json(result);

            var cities = await _cityInfoService.GetAllCityCodes();
            var insertDatas = new List<OnlineSchoolExtLevelInfo>();
            var errorCount = 0;
            foreach (var row in datas)
            {
                var entity = new OnlineSchoolExtLevelInfo()
                {
                    ID = Guid.NewGuid()
                };
                errorCount++;
                foreach (var cell in row.OrderByDescending(p => p.Index))
                {
                    if (entity.ID == default) break;
                    switch (cell.Index)
                    {
                        case 0:
                            if (cities.Any(p => p.Name == cell.Value)) { entity.CityCode = cities.FirstOrDefault(p => p.Name == cell.Value)?.Id; }
                            else { entity.ID = default; }
                            break;
                        case 1:
                            if (!string.IsNullOrWhiteSpace(SchFTypeUtil.GetCode(cell.Value))) { entity.SchFType = SchFTypeUtil.GetCode(cell.Value); }
                            else { entity.ID = default; }
                            break;
                        case 2:
                            if (!string.IsNullOrWhiteSpace(cell.Value)) { entity.ReplaceSource = cell.Value; }
                            else { entity.ID = default; }
                            break;
                        case 3:
                            if (!string.IsNullOrWhiteSpace(cell.Value)) { entity.LevelName = cell.Value; }
                            else { entity.ID = default; }
                            break;
                    }
                }
                if (entity.ID == default || string.IsNullOrWhiteSpace(entity.LevelName) || string.IsNullOrWhiteSpace(entity.ReplaceSource)) continue;
                await _onlineSchoolExtLevelService.RemoveByCityCodeSchFType(entity.CityCode.Value, entity.SchFType);
                insertDatas.Add(entity);
                errorCount--;
            }
            #endregion

            #region Save datas
            var saveCount = 0;
            if (insertDatas.Any())
            {
                foreach (var item in insertDatas)
                {
                    if (await _onlineSchoolExtLevelService.AddAsync(item)) saveCount++;
                }
                result = ResponseResult.Success(new { saveCount, errorCount });
            }
            #endregion

            return Json(result);
        }
    }
}
