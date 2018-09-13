using Bit.Core.Contracts;
using Bit.Owin.Exceptions;
using CrmPortal.Api.Contracts;
using CrmPortal.Data;
using CrmPortal.Data.Contracts;
using CrmPortal.Model;
using Microsoft.EntityFrameworkCore;
using Refit;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmPortal.Api
{
    public interface ICustomersController
    {
        [Post("/api/Customers/AddNewCustomer")]
        Task<Customer> AddNewCustomer(Customer customer, CancellationToken cancellationToken);

        [Post("/api/Customers/DeactivateCustomerById/{customerId}")]
        Task DeactivateCustomerById(Guid customerId, CancellationToken cancellationToken);
    }

    [RoutePrefix("Customers")]
    public class CustomersController : ApiController, ICustomersController
    {
        public virtual CrmPortalDbContext DbContext { get; set; }

        public virtual IUserInformationProvider UserInformationProvider { get; set; }

        public virtual ISmsService SmsService { get; set; }

        [HttpPost, Route("AddNewCustomer")]
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

        [HttpPost, Route("DeactivateCustomerById/{customerId}")]
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
