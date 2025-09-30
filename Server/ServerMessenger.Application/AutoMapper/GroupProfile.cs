using AutoMapper;
using ServerMessenger.Application.DTO;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.AutoMapper
{
    public class GroupProfile : Profile
    {
        public GroupProfile()
        {
            CreateMap<Group, GroupDto>().ReverseMap();
        }
    }
}
