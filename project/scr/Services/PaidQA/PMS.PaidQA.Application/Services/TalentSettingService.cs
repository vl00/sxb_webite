using PMS.PaidQA.Domain.Dtos;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Domain.EntityExtend;
using PMS.PaidQA.Repository;
using PMS.UserManage.Application.IServices;
using ProductManagement.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.PaidQA.Application.Services
{

    public class TalentSettingService : ApplicationService<TalentSetting>, ITalentSettingService
    {
        ITalentSettingRepository _talentSettingRepository;
        IRegionTypeService _regionTypeService;
        IUserService _userService;
        IEvaluateService _evaluateService;
        IOrderRepository _orderRepository;
        IGradeService _gradeService;
        ITalentService _talentService;
        ICollectionService _collectionService;

        public TalentSettingService(ITalentSettingRepository talentSettingRepository, IRegionTypeService regionTypeService,
            IUserService userService, IEvaluateService evaluateService, IGradeService gradeService
            , IOrderRepository orderRepository, ITalentService talentService, ICollectionService collectionService) : base(talentSettingRepository)
        {
            _talentService = talentService;
            _gradeService = gradeService;
            _talentSettingRepository = talentSettingRepository;
            _regionTypeService = regionTypeService;
            _userService = userService;
            _evaluateService = evaluateService;
            _orderRepository = orderRepository;
            _collectionService = collectionService;
        }

        public async Task<TalentSetting> GetByTalentUserID(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return null;
            var str_Where = $"TalentUserID = @talentUserID";
            var finds = _talentSettingRepository.GetBy(str_Where, new { talentUserID });
            if (finds?.Any() == true) { return finds.First(); }
            else
            {
                if (_talentService.IsTalent(talentUserID.ToString()))
                {
                    var userInfo = _userService.GetTalentDetail(talentUserID);
                    var levels = await GetAllTalentLevels();
                    var entity = new TalentSetting()
                    {
                        IsEnable = false,
                        Price = 0,
                        TalentUserID = talentUserID,
                        TalentLevelTypeID = levels?.Any(p => p.Name == "资深专家") == true ? levels.FirstOrDefault(p => p.Name == "资深专家").ID : Guid.Empty
                    };
                    if (userInfo?.Role.HasValue == true && userInfo.Role.Value == 1)
                    {
                        var offcialLevel = levels.FirstOrDefault(p => p.Name == "官方认证");
                        if (offcialLevel?.ID != Guid.Empty)
                        {
                            entity.TalentLevelTypeID = offcialLevel.ID;
                        }
                    }
                    if (_talentSettingRepository.Add(entity)) return entity;
                }
            }
            return null;
        }

        public async Task<TalentDetailExtend> GetDetail(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return null;
            var talentSetting = _talentSettingRepository.GetBy("TalentUserID = @talentUserID", new { talentUserID }).FirstOrDefault();
            if (talentSetting != null)
            {
                var result = CommonHelper.MapperProperty<TalentSetting, TalentDetailExtend>(talentSetting);
                var userInfo = await Task.Run(() =>
                {
                    return _userService.GetTalentDetail(talentUserID);
                });
                result.TalentRegions = await _regionTypeService.GetByTalentUserID(talentUserID);
                result.TalentGrades = await _gradeService.GetByTalentUserID(talentUserID);
                result.TalentLevelName = await _talentSettingRepository.GetLevelName(talentUserID);
                result.AvgScore = await _evaluateService.GetTalentAvgScope(talentUserID);
                result.SixHourReplyPercent = await _orderRepository.GetSixHoursReplyPercentByTalentUserID(talentUserID);
                if (userInfo != null)
                {
                    result.AuthName = userInfo.Certification_preview;
                    result.HeadImgUrl = userInfo.HeadImgUrl;
                    result.NickName = userInfo.Nickname;
                    result.TelentIntroduction = userInfo.Introduction;
                    if (userInfo.Role.HasValue) result.TalentType = userInfo.Role.Value;
                }
                result.SchoolExtId = await _talentSettingRepository.GetSchoolId(talentSetting.TalentUserID);
                return result;
            }
            return null;
        }

        public async Task<TalentRecordDetailDto> GetRecordDetail(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return null;
            var talentRecordDto = await _talentSettingRepository.GetTalentRecord(talentUserID);
            if (talentRecordDto != null)
            {
                var result = CommonHelper.MapperProperty<TalentRecordDto, TalentRecordDetailDto>(talentRecordDto);
                var userInfo = await Task.Run(() =>
                {
                    return _userService.GetTalentDetail(talentUserID);
                });
                result.TalentRegions = await _regionTypeService.GetByTalentUserID(talentUserID);
                result.TalentGrades = await _gradeService.GetByTalentUserID(talentUserID);
                result.TalentLevelName = await _talentSettingRepository.GetLevelName(talentUserID);
                result.AvgScore = await _evaluateService.GetTalentAvgScope(talentUserID);
                result.SixHourReplyPercent = await _orderRepository.GetSixHoursReplyPercentByTalentUserID(talentUserID);
                if (userInfo != null)
                {
                    result.AuthName = userInfo.Certification_preview;
                    result.HeadImgUrl = userInfo.HeadImgUrl;
                    result.NickName = userInfo.Nickname;
                    result.TelentIntroduction = userInfo.Introduction;
                    if (userInfo.Role.HasValue) result.TalentType = userInfo.Role.Value;
                }
                result.SchoolExtId = await _talentSettingRepository.GetSchoolId(talentRecordDto.TalentUserID);
                return result;
            }
            return null;
        }


        public async Task<string> GetTalentLevelName(Guid talentUserID)
        {
            if (talentUserID == Guid.Empty) return string.Empty;
            return await _talentSettingRepository.GetLevelName(talentUserID);
        }

        public async Task<IEnumerable<LevelType>> GetAllTalentLevels()
        {
            return await _talentSettingRepository.GetAllTalentLevels();
        }

        public async Task<IEnumerable<PageTalentIDDto>> PageTalentIDs(int pageIndex = 1, int pageSize = 10, Guid? gradeID = null, Guid? regionTypeID = null, Guid? levelID = null, int? orderTypeID = null
            , decimal minPrice = 0, decimal maxPrice = 0, string nickName = null, bool isInternal = false)
        {
            orderTypeID = orderTypeID ?? 0;
            return await _talentSettingRepository.PageTalentIDs(pageIndex, pageSize, gradeID, regionTypeID, levelID, orderTypeID.Value, minPrice, maxPrice, nickName, isInternal);
        }

        public async Task<IEnumerable<TalentDetailExtend>> GetDetails(IEnumerable<Guid> talentUserIDs)
        {
            if (talentUserIDs == null || !talentUserIDs.Any()) return null;
            var talentSettings = _talentSettingRepository.GetBy("TalentUserID in @talentUserIDs", new { talentUserIDs });
            if (talentSettings?.Any() == true)
            {
                var results = new List<TalentDetailExtend>();
                var userInfos = await Task.Run(() =>
                {
                    return _userService.GetTalentDetails(talentUserIDs);
                });
                var talentRegions = await _regionTypeService.GetByTalentUserIDs(talentSettings.Select(p => p.TalentUserID).Distinct());
                foreach (var talentSetting in talentSettings)
                {
                    var result = CommonHelper.MapperProperty<TalentSetting, TalentDetailExtend>(talentSetting);
                    var userInfo = userInfos.FirstOrDefault(p => p.Id == talentSetting.TalentUserID);
                    if (talentRegions != null && talentRegions.TryGetValue(talentSetting.TalentUserID, out List<RegionType> region))
                    {
                        result.TalentRegions = region;
                    }
                    result.TalentGrades = await _gradeService.GetByTalentUserID(talentSetting.TalentUserID);
                    result.TalentLevelName = await _talentSettingRepository.GetLevelName(talentSetting.TalentUserID);
                    result.AvgScore = await _evaluateService.GetTalentAvgScope(talentSetting.TalentUserID);
                    result.SixHourReplyPercent = await _orderRepository.GetSixHoursReplyPercentByTalentUserID(talentSetting.TalentUserID);
                    if (userInfo != null)
                    {
                        result.AuthName = userInfo.Certification_preview;
                        result.HeadImgUrl = userInfo.HeadImgUrl;
                        result.NickName = userInfo.Nickname;
                        result.TelentIntroduction = userInfo.Introduction;
                        if (userInfo.Role.HasValue) result.TalentType = userInfo.Role.Value;
                    }
                    results.Add(result);
                }
                return results;
            }
            return null;
        }

        public async Task<IEnumerable<TalentDetailExtend>> GetDetailsBySchool(Guid schoolExtId)
        {

            var talentSettings = await  _talentSettingRepository.GetBySchool(schoolExtId);
            if (talentSettings?.Any() == true)
            {
                var results = new List<TalentDetailExtend>();
                var userInfos = await Task.Run(() =>
                {
                    return _userService.GetTalentDetails(talentSettings.Select(t=>t.TalentUserID).ToList());
                });
                var talentRegions = await _regionTypeService.GetByTalentUserIDs(talentSettings.Select(p => p.TalentUserID).Distinct());
                foreach (var talentSetting in talentSettings)
                {
                    var result = CommonHelper.MapperProperty<TalentSetting, TalentDetailExtend>(talentSetting);
                    var userInfo = userInfos.FirstOrDefault(p => p.Id == talentSetting.TalentUserID);
                    if (talentRegions != null && talentRegions.TryGetValue(talentSetting.TalentUserID, out List<RegionType> region))
                    {
                        result.TalentRegions = region;
                    }
                    result.TalentGrades = await _gradeService.GetByTalentUserID(talentSetting.TalentUserID);
                    result.TalentLevelName = await _talentSettingRepository.GetLevelName(talentSetting.TalentUserID);
                    result.AvgScore = await _evaluateService.GetTalentAvgScope(talentSetting.TalentUserID);
                    result.SixHourReplyPercent = await _orderRepository.GetSixHoursReplyPercentByTalentUserID(talentSetting.TalentUserID);
                    if (userInfo != null)
                    {
                        result.AuthName = userInfo.Certification_preview;
                        result.HeadImgUrl = userInfo.HeadImgUrl;
                        result.NickName = userInfo.Nickname;
                        result.TelentIntroduction = userInfo.Introduction;
                        if (userInfo.Role.HasValue) result.TalentType = userInfo.Role.Value;
                    }
                    results.Add(result);
                }
                return results;
            }
            return null;
        }

        public async Task<IEnumerable<TalentDetailExtend>> GetSimilarTalents(Guid talentID, int count = 5)
        {
            var similarTalentIDs = new List<Guid>();
            var talentIDs = await _talentSettingRepository.GetSimilarTalentIDs(talentID);
            if (talentIDs?.Any() == true)
            {
                similarTalentIDs.AddRange(talentIDs.Select(p => p.Key).Distinct().Take(count));
                if (similarTalentIDs.Count() < count) { count = count - similarTalentIDs.Count(); }
                else
                {
                    count = 0;
                }
            }
            if (count > 0)
            {
                var notINIDs = new List<Guid>() { talentID };
                if (similarTalentIDs?.Any() == true) notINIDs.AddRange(similarTalentIDs);
                var randomIDs = await _talentSettingRepository.GetRandomTalents(count, notINIDs);
                if (randomIDs?.Any() == true)
                {
                    similarTalentIDs.AddRange(randomIDs.Select(p => p.TalentUserID).Distinct());
                }
            }
            if (similarTalentIDs?.Any() == true)
            {
                var finds = await GetDetails(similarTalentIDs);
                if (finds?.Any() == true)
                {
                    var finalResult = new List<TalentDetailExtend>();
                    foreach (var item in similarTalentIDs)
                    {
                        if (finds.Any(p => p.TalentUserID == item)) finalResult.Add(finds.First(p => p.TalentUserID == item));
                    }
                    return finalResult;
                }
            }
            return null;
        }

        public async Task<TalentDetailExtend> GetDetail(Guid talentUserID, Guid operatorID)
        {
            var detail = await this.GetDetail(talentUserID);
            detail.IsFollowed = _collectionService.IsCollected(operatorID, talentUserID);
            return detail;
        }

        public async Task<IEnumerable<TalentDetailExtend>> PageSimilarTalents(Guid talentID, int pageIndex = 1, int pageSize = 10)
        {
            if (talentID == Guid.Empty) return null;
            var regions = await _regionTypeService.GetByTalentUserID(talentID);
            var grades = await _gradeService.GetByTalentUserID(talentID);

            var regionTypeIDs = regions?.Select(p => p.ID).Distinct();
            var gradeIDs = grades?.Select(p => p.ID).Distinct();
            var ids = await _talentSettingRepository.PageSimilarTalentIDs(regionTypeIDs, gradeIDs, pageIndex, pageSize, talentID);
            if (ids?.Any() == true)
            {
                var userIDs = new List<Guid>();
                foreach (var item in ids)
                {
                    userIDs.Add(item.Key);
                }
                var finds = await GetDetails(userIDs);
                if (finds?.Any() == true)
                {
                    var finalResult = new List<TalentDetailExtend>();
                    foreach (var item in userIDs)
                    {
                        if (finds.Any(p => p.TalentUserID == item)) finalResult.Add(finds.First(p => p.TalentUserID == item));
                    }
                    return finalResult;
                }
            }
            return null;
        }


        public async Task<IEnumerable<(int Grade, Guid UserId)>> GradeUserIds()
        {
            return await _talentSettingRepository.GradeUserIds();
        }
    }
}
