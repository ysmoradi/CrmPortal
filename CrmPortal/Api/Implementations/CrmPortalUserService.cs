using Bit.IdentityServer.Implementations;
using Bit.Owin.Exceptions;
using IdentityServer3.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Api.Implementations
{
    public class CrmPortalUserService : UserService
    {
        public async override Task<string> GetUserIdByLocalAuthenticationContextAsync(LocalAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (context.UserName == context.Password)
                return context.UserName;

            throw new DomainLogicException("LoginFailed");
        }

        public async override Task<bool> UserIsActiveAsync(IsActiveContext context, string userId, CancellationToken cancellationToken)
        {
            return true;
        }
    }
}
