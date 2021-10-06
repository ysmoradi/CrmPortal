using Bit.Core.Exceptions;
using Bit.Core.Models;
using Bit.IdentityServer.Implementations;
using CrmPortal.Data;
using CrmPortal.Model;
using CrmPortal.Util;
using IdentityServer3.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Api.Implementations
{
    public class CrmPortalUserService : UserService
    {
        public virtual CrmPortalDbContext DbContext { get; set; }

        public async override Task<BitJwtToken> LocalLogin(LocalAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
                throw new BadRequestException("InvalidUserNameAndOrPassword");

            User user = await DbContext.Users.SingleOrDefaultAsync(u => u.UserName.ToLower() == context.UserName.ToLower(), cancellationToken);

            if (user == null)
                throw new BadRequestException("InvalidUserNameAndOrPassword");

            if (!HashUtility.VerifyHash(context.Password, user.Password))
                throw new BadRequestException("InvalidUserNameAndOrPassword");

            var token = new Model.Token { IP = "", LoggedInDateTime = System.DateTimeOffset.UtcNow, UserId = user.Id };

            DbContext.Set<Model.Token>().Add(token);

            await DbContext.SaveChangesAsync();

            return new BitJwtToken
            {
                UserId = user.Id.ToString(),
                Claims = new Dictionary<string, string> 
                {
                    { "TokenId", token.Id.ToString() }
                }
            };
        }
    }
}
