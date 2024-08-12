using System;
namespace PMS.Infrastructure.Domain.Entities
{
    public class LocalInfo
    {
        public int Id { get; set; }
        public string Name   { get; set; }
        public int Parentid { get; set; }
        public int Type  { get; set; }
        public string Description  { get; set; }
    
    }
}
