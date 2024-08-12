using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;
using PMS.Infrastructure.Application.IService;
using PMS.UserManage.Application.IServices;
using PMS.UserManage.Application.ModelDto;
using ProductManagement.API.Http.Interface;
using ProductManagement.Framework.Cache.Redis;
using ProductManagement.Framework.Foundation;
using ProductManagement.Framework.MongoDb;
using Sxb.Web.Response;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Web.Areas.Common.WeChatQRCallBackHandle.Handlers
{

    /// <summary>
    /// 处理网课通顾问海报扫描事件
    /// </summary>
    public class WangKeTongGuWenHaiBaoHandler : ISubscribeCallBackHandler
    {
        Dictionary<string, string> cityTips = new Dictionary<string, string>()
        {
            { "cd",@"欢迎参加【年末聚惠！1元囤好礼】活动！
邀请1位宝妈即可获得1积分。
回复关键词【积分】，可查询当前积分进度 [太阳]

[礼物]6积分可1元购买
《婴儿洗衣液2.5L》/《彩虹湿巾5包》二选一

[礼物]20积分可1元购买
《不一样的卡梅拉12册》/《冠积 积木桌1套》
《你看起来很好吃 7册》/《3D打印笔》四选一 

长按保存下方您的个人推广海报邀请好友获取奖品吧[太阳]"},
            { 
            "gz",@"欢迎参加【双十一1元购】活动！
邀请1位宝妈即可获得1积分。
回复关键词【积分】，可查询当前积分进度 [太阳]

[礼物]2积分可1元购任一款式 《24色迪士尼原木杆彩铅》

[礼物]5积分可1元购《8册熊孩子奇趣洞洞书》

[礼物]10积分可1元购《迪士尼文具大礼盒13件 》 /《致砖超能机械师编程玩具》

长按保存下方您的专属海报邀请好友获取奖品吧[太阳]"}
        };

        IUserService _userService;
        IWeChatService _weChatService;
        ILogger<WangKeTongGuWenHaiBaoHandler> _logger;
        IMongoService _mongoService;
        IHostEnvironment _hostEnvironment;
        IWXWorkServiceClient _wxWorkServiceClient;
        IEasyRedisClient _easyRedisClient;
        IHttpClientFactory _httpClientFactory;
        IMarketingServiceClient _marketingServiceClient;
        public WangKeTongGuWenHaiBaoHandler(
                IUserService userService
            , IWeChatService weChatService
            , ILogger<WangKeTongGuWenHaiBaoHandler> logger
            , IMongoService mongoService
            , IHostEnvironment hostEnvironment
            , IWXWorkServiceClient wxWorkServiceClient
            , IEasyRedisClient easyRedisClient
            , IHttpClientFactory httpClientFactory
            , IMarketingServiceClient marketingServiceClient)
        {
            _userService = userService;
            _weChatService = weChatService;
            _logger = logger;
            _mongoService = mongoService;
            _hostEnvironment = hostEnvironment;
            _wxWorkServiceClient = wxWorkServiceClient;
            _easyRedisClient = easyRedisClient;
            _httpClientFactory = httpClientFactory;
            _marketingServiceClient = marketingServiceClient;
        }

        public async Task<ResponseResult> Process(SubscribeCallBackQueryRequest qrequest, SubscribeCallBackFormRequest frequest)
        {
            string city = "cd";
            if (!string.IsNullOrEmpty(qrequest.DataJson))
            {
                city = JObject.Parse(qrequest.DataJson)["city"].Value<string>();
            }

            //顾问的用户ID
            var guWenUserId = qrequest.DataId;
            //发展人的OpenId
            var faZhangRenOpenId = frequest.OpenId;
            var guWenUserInfo = _userService.GetUserInfo(guWenUserId.Value);
            var faZhangRenUserInfo = _userService.GetUserByWeixinOpenId(faZhangRenOpenId);
            var guwenUnionId = await _userService.GetWXUnionId(guWenUserInfo.Id);
            var fazhangRenUnionId = await _userService.GetWXUnionId(faZhangRenUserInfo.Id);
            /****************************************
             ** ToDo:                              **
             **    记录顾问海报扫码数、发展人数    **
             **    生成发展人海报                  **
             **    服务号发送发展人海报。          **
             ****************************************/

            //发送提示语
            bool sendTipsFlag = await _weChatService.SendText(faZhangRenOpenId, cityTips[city]);
            //if (!sendTipsFlag)
            //{
            //    return ResponseResult.Failed("发送提示语失败。");
            //}

            //发送海报
            string mediaId = await GetWXMediaId(guWenUserInfo, faZhangRenUserInfo, guwenUnionId, fazhangRenUnionId, faZhangRenOpenId,city);

            if (!string.IsNullOrEmpty(mediaId))
            {
                _logger.LogInformation($"本次获得的媒体ID:{mediaId}");
                bool sendHaiBaoFlag = await _weChatService.SendImage(faZhangRenOpenId, mediaId);
                if (!sendHaiBaoFlag)
                {
                    _logger.LogError("发送海报失败。mediaId={mediaId},openId={openId}", mediaId, faZhangRenOpenId);
                }
            }

            #region 插入统计记录
            string collectionName = nameof(WangKeTongGuWenStatic);
            if (!_hostEnvironment.IsProduction())
            {
                collectionName += $"_{_hostEnvironment.EnvironmentName}";
            }
            var document = _mongoService.ImongdDb.GetCollection<WangKeTongGuWenStatic>(collectionName);

            if (document == null)
            {
                await _mongoService.ImongdDb.CreateCollectionAsync(collectionName);
                document = _mongoService.ImongdDb.GetCollection<WangKeTongGuWenStatic>(collectionName);
            }
            //插入一条关联记录用于统计
            await document.InsertOneAsync(new WangKeTongGuWenStatic()
            {
                Id = ObjectId.GenerateNewId(),
                CreateTime = DateTime.Now,
                FaZhangRenNickName = faZhangRenUserInfo.NickName,
                FaZhangRenUserId = faZhangRenUserInfo.Id,
                GuWenNickName = guWenUserInfo.NickName,
                GuWenUserId = guWenUserInfo.Id
            });

            #endregion

            #region 预锁粉
            //预锁粉
            await _marketingServiceClient.LockFans(faZhangRenUserInfo.Id, guWenUserInfo.Id,1);
            #endregion



            return ResponseResult.Success("OK");


        }


        /// <summary>
        /// 获取当次海报的微信媒体ID
        /// </summary>
        /// <param name="guwenUserInfo"></param>
        /// <param name="fazhangRenUserInfo"></param>
        /// <returns></returns>
        async Task<string> GetWXMediaId(UserInfoDto guwenUserInfo, UserInfoDto fazhangRenUserInfo, string guwenUionId, string fazhangRenUnionId, string fazhangRenOpenId,string city)
        {
            try
            {
                string mediaIdCacheKey = $"wangketonghaibao:{fazhangRenUserInfo.Id}:{guwenUserInfo.Id}";
                var baseImgPath = Path.Combine(_hostEnvironment.ContentRootPath, $"Resources/WangKeTong/inviter_{city}.png");
                string customerQrCodeUrl = await _wxWorkServiceClient.GetAddCustomerQrCode(new ProductManagement.API.Http.Request.WXWork.GetAddCustomerQrCodeRequest()
                {
                    AdviserId = guwenUionId,
                    InviterId = fazhangRenUnionId,
                    InviterOpenId = fazhangRenOpenId,
                    AdviserUserId = guwenUserInfo.Id.ToString()
                });
                //比对qrcodeImage MD5以及baseImageMd5，有变动则从新生成。
                var qrcodeImage = await GraphicsHelper.ImageFromUrl(customerQrCodeUrl);
                string qrcodeMd5 = qrcodeImage.GetMD5();
                var baseImage = Image.FromFile(baseImgPath);
                string baseImageMd5 = baseImage.GetMD5();

                var haibaodata = GraphicsHelper.CreateWangKeTongInviterHaiBao(
                    baseImage
                    , fazhangRenUserInfo.NickName
                    , await GraphicsHelper.ImageFromUrl(fazhangRenUserInfo.HeadImgUrl)
                    , qrcodeImage)?.ToArray();

                _logger.LogDebug($"本次生成的BaseImageMd5值:{baseImageMd5};QRCodeMd5值：{qrcodeMd5}");
                if (await _easyRedisClient.ExistsAsync(mediaIdCacheKey))
                {
                    var mediaImageCahce = await _easyRedisClient.GetAsync<WangKeTongMediaImageChache>(mediaIdCacheKey);
                    _logger.LogDebug($"缓存的BaseImageMD5值:{mediaImageCahce.BaseImageMD5}");
                    _logger.LogDebug($"缓存的QRCodeMD5值:{mediaImageCahce.QRCodeMD5}");
                    if (
                        mediaImageCahce.BaseImageMD5.Equals(baseImageMd5, StringComparison.OrdinalIgnoreCase)
                        &&
                        mediaImageCahce.QRCodeMD5.Equals(qrcodeMd5, StringComparison.OrdinalIgnoreCase)
                        )
                    {
                        _logger.LogDebug("媒体ID来自于缓存");
                        return mediaImageCahce.MediaId;
                    }
                }

                string mediaId = await _weChatService.AddTempMedia(WeChat.Interface.MediaType.image, haibaodata, "inviter.png");
                await _easyRedisClient.AddAsync(mediaIdCacheKey, new WangKeTongMediaImageChache()
                {
                    MediaId = mediaId,
                    BaseImageMD5 = baseImageMd5,
                    QRCodeMD5 = qrcodeMd5
                }, TimeSpan.FromDays(3));
                _logger.LogDebug("媒体ID来自于新上传");
                return mediaId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取海报MediaId发生异常。fazhangRenOpenId={fazhangRenOpenId},fazhangRenUnionId={fazhangRenUnionId}", fazhangRenOpenId, fazhangRenUnionId);
                return "";
            }

        }

        async Task<string> GetUnionId(string openId)
        {
            return await _easyRedisClient.GetOrAddAsync($"openId:{openId}", async () =>
              {
                  return await _weChatService.GetUnionId(openId, PMS.Infrastructure.Application.Enums.WeChatAppChannel.fwh);
              });

        }


      }

    public class WangKeTongGuWenStatic
    {
        public ObjectId Id { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid GuWenUserId { get; set; }
        public string GuWenNickName { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid FaZhangRenUserId { get; set; }
        public string FaZhangRenNickName { get; set; }
        public DateTime CreateTime { get; set; }



    }

    public class WangKeTongMediaImageChache
    {

        public string MediaId { get; set; }

        public string BaseImageMD5 { get; set; }

        public string QRCodeMD5 { get; set; }
    }
}
