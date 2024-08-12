using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.School.Domain.Dtos
{
    [Serializable]
    public class KeyValueDto<T>
    {
        public KeyValueDto()
        {
        }

        public string Year { get; set; }
        public string Key { get; set; }
        public T Value { get; set; }
        public int Other { get; set; }
        public string Message { get; set; }
    }
    [Serializable]
    public class KeyValueDto<K, V, M>
    {
        public K Key { get; set; }

        public V Value { get; set; }

        public M Message { get; set; }
    }

    [Serializable]
    public class KeyValueDto<K, V, M, D>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public M Message { get; set; }
        public D Data { get; set; }
    }

    [Serializable]
    public class KeyValueDto<K, V, M, D, O>
    {
        public K Key { get; set; }
        public V Value { get; set; }
        public M Message { get; set; }
        public D Data { get; set; }
        public O Other { get; set; }
    }
}
