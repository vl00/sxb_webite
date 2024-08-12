using Org.BouncyCastle.Asn1.Cms;
using PMS.TopicCircle.Domain.Entities;
using PMS.TopicCircle.Domain.Repositories;
using ProductManagement.Framework.Cache.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMS.TopicCircle.Application.Services
{
    public class KeyValueService : IKeyValueService
    {
        IKeyValueRepository _keyValueRepository;
        IEasyRedisClient _easyRedisClient;
        public KeyValueService(
            IKeyValueRepository keyValueRepository,
            IEasyRedisClient easyRedisClient
            )
        {
            this._keyValueRepository = keyValueRepository;
            this._easyRedisClient = easyRedisClient;
        }

        public string Get(string key)
        {
            string value = this._easyRedisClient.GetAsync<string>(key).GetAwaiter().GetResult();
            if (value == null)
            {
                keyValue kv = this._keyValueRepository.Get(key);
                if (kv != null)
                {
                    return kv.Value;
                }
                else
                {
                    return null;
                }

            }
            else {
                return value;
            }
        }

        public bool Set(string key, string value)
        {
            bool dbIsSet;
            keyValue kv = this._keyValueRepository.Get(key);

            if (kv == null)
            {
                dbIsSet = this._keyValueRepository.Add(new keyValue()
                {
                    Key = key,
                    Value = value
                });
            }
            else
            {
                kv.Value = value;
                dbIsSet = this._keyValueRepository.Update(kv);
            }
            return dbIsSet;
        }
    }
}
