using Microsoft.Extensions.Configuration;
using ProductManagement.API.SMS;
using System;
using System.Collections.Generic;
using System.Text;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20190711;
//using TencentCloud.Sms.V20210111;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtend
    {
        internal static IConfiguration Configuration;

        public static void AddTencentSms(this IServiceCollection services)
        {
            services.AddSingleton((sp) =>
            {
                Configuration = sp.GetRequiredService<IConfiguration>();
                Credential cred;
                string region = Configuration.GetSection(Keys.Region).Value;
                /* 必要步骤：
               * 实例化一个认证对象，入参需要传入腾讯云账户密钥对 secretId 和 secretKey
               * 本示例采用从环境变量读取的方式，需要预先在环境变量中设置这两个值
               * 您也可以直接在代码中写入密钥对，但需谨防泄露，不要将代码复制、上传或者分享给他人
               * CAM 密匙查询：https://console.cloud.tencent.com/cam/capi*/
                cred = new Credential
                {
                    SecretId = Configuration.GetSection(Keys.CredentialSecretId).Value, //"AKIDuZBYflyUUETpsfCRP6B3g2A2cwMdLOr4",
                    SecretKey = Configuration.GetSection(Keys.CredentialSecretKey).Value //"4xd3C5hLksjAK9j2GDtLWTAajgNnysW1"
                };
                /* 非必要步骤:
                 * 实例化一个客户端配置对象，可以指定超时时间等配置 */
                //ClientProfile clientProfile = new ClientProfile();
                /* SDK 默认用 TC3-HMAC-SHA256 进行签名
                 * 非必要请不要修改该字段 */
                //clientProfile.SignMethod = ClientProfile.SIGN_TC3SHA256;
                /* 非必要步骤
                 * 实例化一个客户端配置对象，可以指定超时时间等配置 */
                //HttpProfile httpProfile = new HttpProfile();
                /* SDK 默认使用 POST 方法
                 * 如需使用 GET 方法，可以在此处设置，但 GET 方法无法处理较大的请求 */
                //httpProfile.ReqMethod = "GET";
                /* SDK 有默认的超时时间，非必要请不要进行调整
                 * 如有需要请在代码中查阅以获取最新的默认值 */
                //httpProfile.Timeout = 10; // 请求连接超时时间，单位为秒(默认60秒)
                /* SDK 会自动指定域名，通常无需指定域名，但访问金融区的服务时必须手动指定域名
               * 例如 SMS 的上海金融区域名为 sms.ap-shanghai-fsi.tencentcloudapi.com */
                //httpProfile.Endpoint = "sms.tencentcloudapi.com";
                // 代理服务器，当您的环境下有代理服务器时设定
                //httpProfile.WebProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY");
                //clientProfile.HttpProfile = httpProfile;
                return new SmsClient(cred, region);
            });

            services.AddScoped<ITencentSmsService, TencentSmsService>();
        }
    }
}
