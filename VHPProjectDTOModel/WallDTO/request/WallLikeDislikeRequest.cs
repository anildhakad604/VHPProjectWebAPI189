namespace VHPProjectDTOModel.WallDTO.request
{
    public class WallLikeDislikeRequest
    {
        public int MemberDetailsId { get; set; }
        public int WallId { get; set; }
        public bool Like { get; set; }
    }
}
