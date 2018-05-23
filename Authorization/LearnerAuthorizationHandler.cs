using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using lmsextreg.Models;
using lmsextreg.Constants;
using lmsextreg.Data;

namespace lmsextreg.Authorization
{
    public class LearnerAuthorizationHandler
        : AuthorizationHandler<OperationAuthorizationRequirement, Enrollment>
    {
        UserManager<ApplicationUser> _userManager;

        public LearnerAuthorizationHandler(UserManager<ApplicationUser> userMgr)
        {
            _userManager = userMgr;
        }
        
        protected override Task
            HandleRequirementAsync( AuthorizationHandlerContext authContext,
                                    OperationAuthorizationRequirement authRequirement,
                                    Enrollment authResource)
            {
                if ( authContext == null || authResource == null )
                {
                    return Task.CompletedTask;
                }

                if  (   authRequirement.Name != CRUDConstants.CREATE    &&
                        authRequirement.Name != CRUDConstants.RETRIEVE  &&
                        authRequirement.Name != CRUDConstants.UPDATE    &&
                        authRequirement.Name != CRUDConstants.DELETE
                    )
                {
                    return Task.CompletedTask;
                }

                if ( authResource.LearnerID == _userManager.GetUserId(authContext.User) )
                {
                    authContext.Succeed(authRequirement);
                }

                return Task.CompletedTask;
            }
    }
}