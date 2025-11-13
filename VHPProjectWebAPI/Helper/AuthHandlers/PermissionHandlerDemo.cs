//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using MasterProjDAL.UserRolePerRepo;
//using System.Threading.Tasks;

//namespace MasterProjWebAPI.Helper.AuthHandlers
//{
//    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
//    {
//        private readonly IUserRolePerRepository _userRolePerRepository;
//        private readonly IHttpContextAccessor _httpContextAccessor;

//        public PermissionHandler(IUserRolePerRepository userRolePerRepository, IHttpContextAccessor httpContextAccessor)
//        {
//            _userRolePerRepository = userRolePerRepository;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
//        {
//            var userId = context.User.FindFirst("UserId")?.Value;
//            if (string.IsNullOrEmpty(userId)) return;

//            var permissions = await _userRolePerRepository.GetUserPermissionsByUserId(int.Parse(userId));
//            if (permissions.Contains(requirement.Permission))
//            {
//                context.Succeed(requirement);
//            }
//        }
//    }
//}
