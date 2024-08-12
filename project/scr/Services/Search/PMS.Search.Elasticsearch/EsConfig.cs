using System;
using System.Collections.Generic;
using System.Text;

namespace PMS.Search.Elasticsearch
{
    public class EsConfig
    {
        public Indices Indices { get; set; } = new Indices();
    }

    public class Indices
    {
        public Index School { get; set; } = new Index() { Name = "schoolindex_09", Alias = "school" };

        public Index Comment { get; set; } = new Index() { Name = "commentindex_03", Alias = "comment" };

        public Index Question { get; set; } = new Index() { Name = "questionindex_03", Alias = "question" };

        public Index Article { get; set; } = new Index() { Name = "articleindex_06", Alias = "article" };

        public Index Topic { get; set; } = new Index() { Name = "topicindex", Alias = "topic" };

        public Index Circle { get; set; } = new Index() { Name = "circleindex", Alias = "circle" };

        public Index Talent { get; set; } = new Index() { Name = "talentindex", Alias = "talent" };

        public Index University { get; set; } = new Index() { Name = "universityindex", Alias = "university" };

        public Index Course { get; set; } = new Index() { Name = "courseindex", Alias = "course" };

        public Index Evaluation { get; set; } = new Index() { Name = "evaluationindex", Alias = "evaluation" };

        public Index Keyword { get; set; } = new Index() { Name = "keywordindex", Alias = "keyword" };

    }

    public class Index
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public string SearchIndex
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Alias) ? Alias : Name;
            }
        }
    }
}
