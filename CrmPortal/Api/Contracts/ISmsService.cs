using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Api.Contracts
{
    public interface ISmsService
    {
        Task SendSms(string phoneNo, string message, CancellationToken cancellationToken);
    }
}
