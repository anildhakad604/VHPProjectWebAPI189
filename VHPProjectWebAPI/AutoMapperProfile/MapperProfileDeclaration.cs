using AutoMapper;
using VHPProjectDAL.DataModel;
using VHPProjectDTOModel.MemberDTO.request;

namespace VHPProjectDAL.AutoMapperProfile
{
    public class MapperProfileDeclaration : Profile
    {
        public MapperProfileDeclaration()
        {
            //CreateMap<Member, AddMemberRequestDTO>()
            //    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName));

            CreateMap<AddMemberRequestDTO, Member>();

            CreateMap<AddMemberRequestDTO, Member>().ReverseMap();


        }
    }
}
