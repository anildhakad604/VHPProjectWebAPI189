//using Microsoft.AspNetCore.Authorization;

//namespace VHPProjectDAL.Helper.AuthHandlers
//{
//    public class PermissionRequirement : IAuthorizationRequirement
//    {
//        public string Permission { get; }
//        public PermissionRequirement(string permission)
//        {
//            Permission = permission;
//        }
//    }

//    public static class Policies
//    {
//        public static AuthorizationPolicy Create(string permission)
//        {
//            return new AuthorizationPolicyBuilder()
//                .AddRequirements(new PermissionRequirement(permission))
//                .Build();
//        }
//    }
//}
