using AutoMapper;

namespace ProductManagement.Tool.AutoMapper
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile configuration);
    }
}
