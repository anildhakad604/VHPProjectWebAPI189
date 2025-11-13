using Microsoft.AspNetCore.Authorization;
using VHPProjectDAL.Repository.MemberRepo;

namespace VHPProjectWebAPI.Helper.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        IMemberRepository _memberRepository;
        public PermissionHandler(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;

        }

        protected override async Task HandleRequirementAsync(
           AuthorizationHandlerContext context,
           PermissionRequirement requirement)
        {

            var memberIdClaim = context.User.FindFirst("MemberId")?.Value;
            if (memberIdClaim == null)
            {
                context.Fail();
                return;
            }

            var memberId = int.Parse(memberIdClaim);

            var member = await _memberRepository.GetPlainMemberById(memberId);
            if (member == null)
            {
                context.Fail();
                return;
            }

            bool isAdmin = member.IsAdmin == true;


            var adminPermissions = new[]
            {
                Policies.AddSatsang,
                Policies.DeleteMember,
                Policies.AddMember
            };

            var memberPermissions = new[]
            {
                Policies.UpdateMember
            };

            if (isAdmin && adminPermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else if (!isAdmin && memberPermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

    }
}
