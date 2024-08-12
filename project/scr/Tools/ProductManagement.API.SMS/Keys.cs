using System;
using System.Collections.Generic;
using System.Text;

namespace ProductManagement.API.SMS
{
   internal static class Keys
    {
        public readonly static string CredentialSecretId = "TencentSdkConfig:Credential:SecretId";

        public readonly static string CredentialSecretKey = "TencentSdkConfig:Credential:SecretKey";

        public readonly static string Region = "TencentSdkConfig:Region";

        public readonly static string SMSAppId = "TencentSdkConfig:Sms:App:{0}:id";
    }
}
