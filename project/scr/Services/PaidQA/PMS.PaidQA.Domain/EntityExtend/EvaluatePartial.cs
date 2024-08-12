using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace PMS.PaidQA.Domain.Entities
{
    public partial class Evaluate
    {

        [Write(false)]
        public List<EvaluateTagRelation> EvaluateTagRelations { get; set; }

    }
}