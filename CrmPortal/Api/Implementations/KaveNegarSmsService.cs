using CrmPortal.Api.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Api.Implementations
{
    public class KaveNegarSmsService : ISmsService
    {
        public virtual async Task SendSms(string phoneNo, string message, CancellationToken cancellationToken)
        {
            // Sned SMS to Customer
        }
    }
}
