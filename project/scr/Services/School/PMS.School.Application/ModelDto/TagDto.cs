using System;
using System.Linq.Expressions;
using PMS.School.Domain.Entities;

namespace PMS.School.Application.ModelDto
{
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string SpellCode { get; set; }
        public int Type { get; set; }
        public int Sort { get; set; }
        public long No { get; set; }
        public bool active { get; set; } = false;
    }
}
