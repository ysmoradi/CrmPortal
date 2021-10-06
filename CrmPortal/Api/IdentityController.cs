using Bit.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrmPortal.Api
{
    public class IdentityController
    {
        public IUserInformationProvider UserInformationProvider { get; set; }
         
        public async Task Logout()
        {
            var tokenId = Guid.Parse(UserInformationProvider.GetBitJwtToken().Claims["TokenId"]);

            // delete
        }
    }
}
