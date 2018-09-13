using Bit.Core.Contracts;
using Bit.Owin.Exceptions;
using CrmPortal.Data;
using CrmPortal.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmPortal.Api
{
    [RoutePrefix("Customers")]
    public class CustomersController : ApiController
    {
        public CrmPortalDbContext DbContext { get; set; }

        public IUserInformationProvider UserInformationProvider { get; set; }

        [HttpPost, Route("AddNewCustomer")]
        public async Task<Customer> AddNewCustomer(Customer customer, CancellationToken cancellationToken)
        {
            if (await DbContext.BlackLists.AnyAsync(bl => bl.Value == customer.FirstName || bl.Value == customer.LastName, cancellationToken))
                throw new DomainLogicException("InvalidFirstNameOrLastName");

            Guid currentUserId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            customer.CreatedById = currentUserId;

            await DbContext.Customers.AddAsync(customer, cancellationToken);

            return customer;
        }

        [HttpPost, Route("DeactivateCustomerById/{customerId}")]
        public async Task DeactivateCustomerById(Guid customerId, CancellationToken cancellationToken)
        {
            Customer customerToBeDeactivated = await DbContext.Customers.FindAsync(customerId, cancellationToken);

            if (customerToBeDeactivated == null)
                throw new ResourceNotFoundException("CustomerCouldNotBeFound");

            if (customerToBeDeactivated.IsActive == false)
                throw new DomainLogicException("CustomerIsAlreadyDeactivated");

            customerToBeDeactivated.IsActive = false;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
