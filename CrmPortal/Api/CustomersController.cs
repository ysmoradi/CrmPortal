using Bit.Core.Contracts;
using Bit.Core.Exceptions;
using CrmPortal.Api.Contracts;
using CrmPortal.Data;
using CrmPortal.Data.Contracts;
using CrmPortal.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Web.Http;
using Refit;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmPortal.Api
{
    public interface ICustomersController
    {
        [Post("/api/v1/customers/add-new-customer")]
        Task<Customer> AddNewCustomer(Customer customer, CancellationToken cancellationToken);

        [Post("/api/v1/customers/deactivate-customer-by-id/{customerId}")]
        Task DeactivateCustomerById(Guid customerId, CancellationToken cancellationToken);
    }

    [ApiVersion("1.0"), RoutePrefix("v{version:apiVersion}/customers")]
    public class CustomersController : ApiController, ICustomersController
    {
        public virtual CrmPortalDbContext DbContext { get; set; }

        public virtual IUserInformationProvider UserInformationProvider { get; set; }

        public virtual ISmsService SmsService { get; set; }

        [HttpPost, Route("add-new-customer")]
        public virtual async Task<Customer> AddNewCustomer(Customer customer, CancellationToken cancellationToken)
        {
            if (await DbContext.BlackLists.AnyAsync(bl => bl.Value == customer.FirstName || bl.Value == customer.LastName, cancellationToken))
                throw new DomainLogicException("InvalidFirstNameOrLastName");

            Guid currentUserId = Guid.Parse(UserInformationProvider.GetCurrentUserId());

            customer.CreatedById = currentUserId;
            customer.IsActive = true;

            await DbContext.Customers.AddAsync(customer, cancellationToken);

            await SmsService.SendSms(customer.PhoneNo, $"Welcome {customer.FirstName} {customer.LastName}", cancellationToken);

            return customer;
        }

        public virtual ICustomersRepository CustomersRepository { get; set; }

        [HttpPost, Route("deactivate-customer-by-id/{customerId}")]
        public virtual async Task DeactivateCustomerById(Guid customerId, CancellationToken cancellationToken)
        {
            Customer customerToBeDeactivated = await CustomersRepository.FindCustomerById(customerId, cancellationToken);

            if (customerToBeDeactivated == null)
                throw new ResourceNotFoundException("CustomerCouldNotBeFound");

            if (customerToBeDeactivated.IsActive == false)
                throw new DomainLogicException("CustomerIsAlreadyDeactivated");

            customerToBeDeactivated.IsActive = false;

            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
