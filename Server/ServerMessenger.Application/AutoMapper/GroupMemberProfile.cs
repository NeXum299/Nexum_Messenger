using AutoMapper;
using ServerMessenger.Application.DTO;
using ServerMessenger.Core.Entities;

namespace ServerMessenger.Application.AutoMapper
{
    /// <summary></summary>
    public class GroupMemberProfile : Profile
    {
        public GroupMemberProfile()
        {
            CreateMap<GroupMember, GroupMemberDto>().ReverseMap();
        }
    }
}
