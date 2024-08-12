using ProductManagement.Framework.Foundation;
using PMS.PaidQA.Domain.Entities;
using PMS.PaidQA.Repository;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PMS.PaidQA.Domain.EntityExtend;

namespace PMS.PaidQA.Application.Services
{
    public class HotTypeService : ApplicationService<HotType>, IHotTypeService
    {
        IHotTypeRepository _hotTypeRepository;
        IEasyRedisClient _easyRedisClient;
        IHotQuestionRepository _hotQuestionRepository;
        ITalentSettingService _talentSettingService;
        IOrderService _orderService;
        public HotTypeService(IHotTypeRepository hotTypeRepository, IEasyRedisClient easyRedisClient, IHotQuestionRepository hotQuestionRepository,
            ITalentSettingService talentSettingService, IOrderService orderService) : base(hotTypeRepository)
        {
            _orderService = orderService;
            _talentSettingService = talentSettingService;
            _hotQuestionRepository = hotQuestionRepository;
            _easyRedisClient = easyRedisClient;
            _hotTypeRepository = hotTypeRepository;
        }

        public async Task<bool> ChangeViewCount(Guid id, int step = 1, bool isUp = true)
        {
            if (id == Guid.Empty) return false;
            var find = await _hotQuestionRepository.GetByOrderID(id);
            if (find != null)
            {
                if (isUp)
                {
                    find.ViewCount += step;
                }
                else
                {
                    find.ViewCount -= step;
                }
                return await _hotQuestionRepository.UpdateAsync(find);
            }
            return false;
        }

        public IEnumerable<HotType> GetAll(int type = 1)
        {
            var str_Where = $"IsValid = 1 and type = @type";
            return _hotTypeRepository.GetBy(str_Where, new { type }, "Sort");
        }

        public async Task<IEnumerable<Guid>> GetAllOrderIDs()
        {
            var redisKey = "AllHotTypeOrderIDs";
            var ids = await _easyRedisClient.GetOrAddAsync(redisKey, async () =>
            {
                var finds = await _hotTypeRepository.GetAllOrderIDs();
                if (finds?.Any() == true)
                {
                    return CommonHelper.ListRandom(finds);
                }
                return new List<Guid>();
            }, TimeSpan.FromHours(3));
            return ids;
        }

        public async Task<HotQuestionExtend> GetHotQuestionDetail(Guid orderID)
        {
            if (orderID == Guid.Empty)
            {
                return null;
            }
            return await _hotQuestionRepository.GetWithTypeNameByOrderID(orderID);
        }

        public async Task<IEnumerable<Guid>> GetOrderIDByHotTypeID(Guid hotTypeID, int pageIndex = 1, int pageSize = 10, int sortType = 1)
        {
            if (hotTypeID == Guid.Empty) return null;
            return await _hotTypeRepository.GetOrderIDByHotTypeID(hotTypeID, pageIndex, pageSize, sortType);
        }

        public async Task<IEnumerable<Guid>> GetRandomOrderIDByGrade(int gradeType)
        {
            if (gradeType < 1) return null;
            var result = new List<Guid>();
            var finds = await _hotQuestionRepository.GetRandomOrderIDByGradeSort(gradeType);
            if (finds?.Any() == true)
            {
                foreach (var userID in finds.Select(p => p.Item2).Distinct().OrderBy(p => Guid.NewGuid()).Take(3))
                {
                    result.Add(finds.Where(p => p.Item2 == userID).OrderBy(p => Guid.NewGuid()).FirstOrDefault().Item1);
                }
            }
            return result;
        }
        public async Task<IEnumerable<Guid>> GetRandomOrderIDByGrade(IEnumerable<int> gradeTypes)
        {
            if (gradeTypes?.Any() == false) return null;
            var result = new List<Guid>();
            var finds = await _hotQuestionRepository.GetRandomOrderIDByGradeSorts(gradeTypes);
            if (finds?.Any() == true)
            {
                foreach (var userID in finds.Select(p => p.Item2).Distinct().OrderBy(p => Guid.NewGuid()).Take(3))
                {
                    result.Add(finds.Where(p => p.Item2 == userID).OrderBy(p => Guid.NewGuid()).FirstOrDefault().Item1);
                }
            }
            return result;
        }

        public async Task<IEnumerable<Guid>> GetRandomOrderIDs(int pageIndex = 1, int pageSize = 10, bool flag = false)
        {
            var orderIDs = await GetAllOrderIDs();
            if (flag)
            {
                return CommonHelper.ListRandom(orderIDs).Take(pageSize);
            }
            else
            {
                return orderIDs.Skip(--pageIndex * pageSize).Take(pageSize);
            }
        }

        public async Task<IEnumerable<(Guid, int)>> GetViewCountByOrderIDs(IEnumerable<Guid> ids)
        {
            if (ids?.Any() == true)
            {
                return await _hotTypeRepository.GetViewCountByOrderIDs(ids);
            }
            else
            {
                return null;
            }
        }

        public async Task<TalentDetailExtend> GetRecommendTalentBySchFTypeCode(string schFTypeCode, Guid userID)
        {
            //var dic = new Dictionary<string, IEnumerable<Guid>>();
            //dic.Add("lx130", new Guid[] { new Guid("36781a83-b4e6-4cff-bd64-88ddc8f9dcd9"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("99423dae-9751-4b06-9d95-48e7c5ee91e7") });
            //dic.Add("lx121", new Guid[] { new Guid("36781a83-b4e6-4cff-bd64-88ddc8f9dcd9"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("99423dae-9751-4b06-9d95-48e7c5ee91e7") });
            //dic.Add("lx120", new Guid[] { new Guid("36781a83-b4e6-4cff-bd64-88ddc8f9dcd9"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("99423dae-9751-4b06-9d95-48e7c5ee91e7") });
            //dic.Add("lx110", new Guid[] { new Guid("36781a83-b4e6-4cff-bd64-88ddc8f9dcd9"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("99423dae-9751-4b06-9d95-48e7c5ee91e7") });
            //dic.Add("lx230", new Guid[] { new Guid("8bf14802-2a3c-412b-8fe5-a2ec00a2a143"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("3800f60e-bce0-494b-8cae-ecd655c9bf9a") });
            //dic.Add("lx231", new Guid[] { new Guid("99423dae-9751-4b06-9d95-48e7c5ee91e7"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("3800f60e-bce0-494b-8cae-ecd655c9bf9a") });
            //dic.Add("lx220", new Guid[] { new Guid("8bf14802-2a3c-412b-8fe5-a2ec00a2a143"), new Guid("99196fbc-0c77-45d8-acc1-356994b7cf4a"), new Guid("3800f60e-bce0-494b-8cae-ecd655c9bf9a") });
            //dic.Add("lx210", new Guid[] { new Guid("99423dae-9751-4b06-9d95-48e7c5ee91e7"), new Guid("1fdac50d-c2a0-473c-9f2f-aed416103832"), new Guid("3530fd26-6d04-44c4-a8ed-ef5d94e5faca") });
            //dic.Add("lx330", new Guid[] { new Guid("46f0a04b-3957-41f5-bbbc-279492c49393"), new Guid("3530fd26-6d04-44c4-a8ed-ef5d94e5faca"), new Guid("1fdac50d-c2a0-473c-9f2f-aed416103832") });
            //dic.Add("lx331", new Guid[] { new Guid("46f0a04b-3957-41f5-bbbc-279492c49393"), new Guid("3530fd26-6d04-44c4-a8ed-ef5d94e5faca"), new Guid("1fdac50d-c2a0-473c-9f2f-aed416103832") });
            //dic.Add("lx320", new Guid[] { new Guid("46f0a04b-3957-41f5-bbbc-279492c49393"), new Guid("3530fd26-6d04-44c4-a8ed-ef5d94e5faca"), new Guid("1fdac50d-c2a0-473c-9f2f-aed416103832") });
            //dic.Add("lx310", new Guid[] { new Guid("46f0a04b-3957-41f5-bbbc-279492c49393"), new Guid("3530fd26-6d04-44c4-a8ed-ef5d94e5faca"), new Guid("1fdac50d-c2a0-473c-9f2f-aed416103832") });
            //dic.Add("lx432", new Guid[] { new Guid("b58a55c8-a8f1-4247-a9e1-082b5e4cc5c3"), new Guid("166d34e9-3408-4b01-b335-4dd1b6d073c9"), new Guid("90509502-5426-4b69-a8f3-e5d1d03a32ce") });
            //dic.Add("lx430", new Guid[] { new Guid("b58a55c8-a8f1-4247-a9e1-082b5e4cc5c3"), new Guid("ce96a454-0ee2-46a6-96c2-f36faa72f063"), new Guid("166d34e9-3408-4b01-b335-4dd1b6d073c9") });
            //dic.Add("lx420", new Guid[] { new Guid("35071785-46d9-4e86-b9f0-7fb9a484ff7d"), new Guid("ce96a454-0ee2-46a6-96c2-f36faa72f063"), new Guid("166d34e9-3408-4b01-b335-4dd1b6d073c9") });
            //dic.Add("lx410", new Guid[] { new Guid("35071785-46d9-4e86-b9f0-7fb9a484ff7d"), new Guid("166d34e9-3408-4b01-b335-4dd1b6d073c9"), new Guid("90509502-5426-4b69-a8f3-e5d1d03a32ce") });
            //await _easyRedisClient.AddAsync("RecommendTalents_SchFType", dic);
            var finds = await _easyRedisClient.GetAsync<Dictionary<string, IEnumerable<Guid>>>("RecommendTalents_SchFType");

            if (finds?.Any(p => p.Key == schFTypeCode) == true)
            {
                var userIDs = finds[schFTypeCode];
                if (userIDs?.Any() == true)
                {
                    var talents = await _talentSettingService.GetDetails(userIDs.Distinct());

                    var processingOrders = await _orderService.Page(1, 99, userID, new int[] { 0, 1, 2, 3 }, true);
                    if (processingOrders?.Items?.Any() == true)
                    {
                        var orderAnwserIDs = processingOrders.Items.Select(x => x.AnswerID).ToList();
                        var lessUsers = talents.Where(p => !orderAnwserIDs.Contains(p.TalentUserID));
                        if (lessUsers?.Any() == true)
                        {
                            lessUsers = CommonHelper.ListRandom(lessUsers);
                            return lessUsers.First();
                        }
                    }
                    talents = CommonHelper.ListRandom(talents);
                    return talents.First();
                }
            }

            return null;
        }
    }
}
