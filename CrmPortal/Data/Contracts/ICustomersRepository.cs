using CrmPortal.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Data.Contracts
{
    public interface ICustomersRepository
    {
        Task<Customer> FindCustomerById(Guid id, CancellationToken cancellationToken);
    }
}
