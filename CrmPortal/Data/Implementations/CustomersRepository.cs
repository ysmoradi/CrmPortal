using CrmPortal.Data.Contracts;
using CrmPortal.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Data.Implementations
{
    public class CustomersRepository : ICustomersRepository
    {
        public virtual CrmPortalDbContext DbContext { get; set; }

        public virtual async Task<Customer> FindCustomerById(Guid id, CancellationToken cancellationToken)
        {
            return await DbContext.Customers.FindAsync(id, cancellationToken);
        }
    }
}
