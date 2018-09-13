using Bit.Test;
using Bit.Test.Core.Implementations;
using CrmPortal.Api;
using CrmPortal.Api.Contracts;
using CrmPortal.Data.Contracts;
using CrmPortal.Model;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Refit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CrmPortal.Test.Api
{
    [TestClass]
    public class CustomersControllerTests
    {
        [TestMethod]
        public async Task AddNewCustomerSuccessTest()
        {
            ISmsService smsService = A.Fake<ISmsService>();

            A.CallTo(() => smsService.SendSms(A<string>.Ignored, A<string>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.CompletedTask);

            using (CrmPortalTestEnv testEnv = new CrmPortalTestEnv(new TestEnvironmentArgs
            {
                AdditionalDependencies = (dependencyManager, services) =>
                {
                    dependencyManager.RegisterInstance(smsService);
                }
            }))
            {
                var token = await testEnv.Server.Login("User1", "P@ssw0rd", "CrmPortal", "secret");

                ICustomersController customersControllerClient = testEnv.Server.BuildRefitClient<ICustomersController>(token);

                Customer customer = await customersControllerClient.AddNewCustomer(new Customer
                {
                    FirstName = "C1",
                    LastName = "C1",
                    NationalCode = "1270340050",
                    PhoneNo = "09123456789"
                }, CancellationToken.None);

                Assert.AreNotEqual(Guid.Empty, customer.Id);

                A.CallTo(() => smsService.SendSms("09123456789", "Welcome C1 C1", A<CancellationToken>.Ignored))
                    .MustHaveHappenedOnceExactly();

                CustomersController customersController = TestDependencyManager.CurrentTestDependencyManager
                   .Objects.OfType<CustomersController>()
                   .Single();

                A.CallTo(() => customersController.AddNewCustomer(A<Customer>.That.Matches(c => c.FirstName == "C1" && c.LastName == "C1"), A<CancellationToken>.Ignored))
                    .MustHaveHappenedOnceExactly();
            }
        }

        [TestMethod]
        public async Task AddNewCustomerShouldResultsIntoAnErrorIfFNameIsABlackListName()
        {
            using (CrmPortalTestEnv testEnv = new CrmPortalTestEnv())
            {
                var token = await testEnv.Server.Login("User1", "P@ssw0rd", "CrmPortal", "secret");

                HttpClient httpClient = testEnv.Server.BuildHttpClient(token);

                ICustomersController customersController = testEnv.Server.BuildRefitClient<ICustomersController>(token);

                try
                {
                    await customersController.AddNewCustomer(new Customer { FirstName = "Bad1", LastName = "Bad1", NationalCode = "1270340050" }, CancellationToken.None);
                    Assert.Fail();
                }
                catch (ApiException ex) when (ex.Content.Contains("InvalidFirstNameOrLastName"))
                {

                }
            }
        }

        [TestMethod]
        public async Task DeactivateCustomerShouldThorwAnExceptionIfCustomerIsAlreadyDeactivated()
        {
            using (CrmPortalTestEnv testEnv = new CrmPortalTestEnv(new TestEnvironmentArgs
            {
                AdditionalDependencies = (dependencyManager, services) =>
                {
                    ICustomersRepository customersRepository = A.Fake<ICustomersRepository>();

                    A.CallTo(() => customersRepository.FindCustomerById(A<Guid>.Ignored, A<CancellationToken>.Ignored))
                        .Returns(Task.FromResult(new Customer
                        {
                            IsActive = false
                        }));

                    dependencyManager.RegisterInstance(customersRepository);
                }
            }))
            {
                var token = await testEnv.Server.Login("User1", "P@ssw0rd", "CrmPortal", "secret");

                ICustomersController customersControllerClient = testEnv.Server.BuildRefitClient<ICustomersController>(token);

                try
                {
                    await customersControllerClient.DeactivateCustomerById(Guid.NewGuid(), CancellationToken.None);
                    Assert.Fail();
                }
                catch (ApiException ex) when (ex.Content.Contains("CustomerIsAlreadyDeactivated"))
                {

                }
            }
        }
    }
}
