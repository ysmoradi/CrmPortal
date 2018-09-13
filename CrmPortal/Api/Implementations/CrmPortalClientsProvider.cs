using Bit.IdentityServer.Contracts;
using Bit.IdentityServer.Implementations;
using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;

namespace CrmPortal.Api.Implementations
{
    public class CrmPortalClientsProvider : OAuthClientsProvider
    {
        public override IEnumerable<Client> GetClients()
        {
            yield return GetResourceOwnerFlowClient(new BitResourceOwnerFlowClient
            {
                ClientId = "CrmPortal",
                ClientName = "CrmPortal",
                Enabled = true,
                Secret = "secret",
                TokensLifetime = TimeSpan.FromDays(7)
            });
        }
    }
}
