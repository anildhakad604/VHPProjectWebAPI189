using AutoMapper;
using VHPProjectDAL.DataModel;
using VHPProjectDTOModel.DesignationDTO.request;
using VHPProjectDTOModel.DesignationDTO.response;
using VHPProjectDTOModel.MemberDTO.request;
using VHPProjectDTOModel.WallDTO.request;
using VHPProjectDTOModel.WallDTO.response;

namespace VHPProjectDAL.AutoMapperProfile
{
    public class MapperProfileDeclaration : Profile
    {
        public MapperProfileDeclaration()
        {
            CreateMap<Designation, DesignationResponse_DTO>();
            CreateMap<AddDesignationRequest_DTO, Designation>();
            
            CreateMap<AddMemberRequestDTO, Member>();

            CreateMap<AddMemberRequestDTO, Member>().ReverseMap();

            // DTO -> Entities
            CreateMap<AddWallDto, Wall>();
            CreateMap<AddWallDto, WallPostImages>();
            CreateMap<AddPostCommentRequest, WallPostComments>()
                .ForMember(d => d.CommentText, opt => opt.MapFrom(s => s.Comment));
            CreateMap<WallLikeDislikeRequest, WallPostLike>()
            .ForMember(dest => dest.Like,
               opt => opt.MapFrom(src => src.Like ? 1 : 0)); // <-- FIX

            CreateMap<WallPostComments, CommentDto>()
                .ForMember(d => d.CommentString, opt => opt.MapFrom(s => s.CommentText))
                .ForMember(d => d.IdWallPostComments, opt => opt.MapFrom(s => s.IdwallPostComments))
                .ForMember(d => d.CommentMemberId, opt => opt.MapFrom(s => s.MemberDetailsId))
                .ForMember(d => d.CommentDate, opt => opt.MapFrom(s => s.InsertDateTime.HasValue ? s.InsertDateTime.Value : DateTime.MinValue));

        }
    }
}
