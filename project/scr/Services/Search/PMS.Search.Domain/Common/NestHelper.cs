using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Domain.Common
{
    public class NestHelper
    {
        public static List<Func<QueryContainerDescriptor<T>, QueryContainer>> KeywordsToNest<T>(List<string> keywords, bool isMatchAllWord, Field field, Field wordField = null)
            where T : class
        {
            wordField = wordField == null ? field : wordField;

            var innerShoulds = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
            foreach (var keyword in keywords)
            {
                var conditions = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
                conditions.Add(s => s.Match(m => m.Field(field).Query(keyword)));

                if (isMatchAllWord && keyword.Length > 1)//仅一个字, 不需要
                {
                    //结果必须完全命中所有字
                    foreach (var word in keyword)
                    {
                        conditions.Add(q => q.Match(m => m.Field(wordField).Query(word.ToString())));
                    }
                }
                innerShoulds.Add(s => s.Bool(b => b.Must(conditions)));
            }
            return innerShoulds;
        }

        public static List<Func<QueryContainerDescriptor<T>, QueryContainer>> KeywordsToNest<T>(List<string> keywords, bool isMatchAllWord, Fields fields, Fields wordFields = null)
            where T : class
        {
            wordFields = wordFields == null ? fields : wordFields;

            var innerShoulds = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
            foreach (var keyword in keywords)
            {
                var conditions = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
                conditions.Add(s => new MultiMatchQuery()
                {
                    Fields = fields,
                    Query = keyword
                });

                if (isMatchAllWord && keyword.Length > 1)//仅一个字, 不需要
                {
                    //结果必须完全命中所有字
                    foreach (var word in keyword)
                    {
                        conditions.Add(q => new MultiMatchQuery()
                        {
                            Fields = wordFields,
                            Query = word.ToString()
                        });
                    }
                }
                innerShoulds.Add(s => s.Bool(b => b.Must(conditions)));
            }
            return innerShoulds;
        }



        public static List<Func<QueryContainerDescriptor<T>, QueryContainer>> KeywordsToNest<T>(List<string> keywords, bool isMatchAllWord
            , Func<string, QueryContainerDescriptor<T>, QueryContainer> matchQuery
            , Func<string, QueryContainerDescriptor<T>, QueryContainer> wordMathcQuery = null)
            where T : class
        {
            wordMathcQuery = wordMathcQuery == null ? matchQuery : wordMathcQuery;

            var innerShoulds = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
            foreach (var keyword in keywords)
            {
                var conditions = new List<Func<QueryContainerDescriptor<T>, QueryContainer>>();
                conditions.Add(s => matchQuery.Invoke(keyword, s));

                if (isMatchAllWord && keyword.Length > 1)//仅一个字, 不需要
                {
                    //结果必须完全命中所有字
                    foreach (var word in keyword)
                    {
                        conditions.Add(s => wordMathcQuery.Invoke(word.ToString(), s));
                    }
                }
                innerShoulds.Add(s => s.Bool(b => b.Must(conditions)));
            }
            return innerShoulds;
        }
    }
}
