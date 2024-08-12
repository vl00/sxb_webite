using System;
namespace PMS.School.Domain.Entities
{
    public class OnlineSchool
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Name_e { get; set; }
        public string Website { get; set; }
        public string Intro { get; set; }
        public bool Show { get; set; }
        public byte Status { get; set; }
        public string Logo { get; set; }
        public DateTime CreateTime { get; set; }
        public Guid Creator { get; set; }
        public DateTime ModifyDateTime { get; set; }
        public Guid Modifier { get; set; }
        public bool IsValid { get; set; }

    }
}
