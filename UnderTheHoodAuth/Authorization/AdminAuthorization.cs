using Microsoft.AspNetCore.Authorization;

namespace UnderTheHoodAuth.Authorization
{
    public class AdminAuthorization : IAuthorizationRequirement
    {
        public AdminAuthorization(int probationMonths)
        {
            ProbationMonths = probationMonths;
        }

        public int ProbationMonths { get; }
    }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminAuthorization>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminAuthorization requirement)
        {
            if (!context.User.HasClaim(x => x.Type == "DOJ"))
            {
                return Task.CompletedTask;
            }

            if (DateTime.TryParse(context.User.FindFirst(x => x.Type == "DOJ")?.Value, out var date)) {
                var period = DateTime.Now - date;
                 if(period.Days * 30 > requirement.ProbationMonths)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
