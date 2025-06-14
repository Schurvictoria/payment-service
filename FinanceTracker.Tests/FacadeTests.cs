using System;
using System.Linq;
using FinanceTracker.Domain;
using FinanceTracker.Infrastructure;
using Xunit;

namespace FinanceTracker.Tests
{
    public class FacadeTests
    {
        [Fact]
        public void BankAccountFacade_CanCreateAndDelete()
        {
            var facade = new BankAccountFacade();
            var acc = facade.Create("Test", 10);
            Assert.Single(facade.GetAll());
            facade.Delete(acc.Id);
            Assert.Empty(facade.GetAll());
        }

        [Fact]
        public void CategoryFacade_CanCreateAndDelete()
        {
            var facade = new CategoryFacade();
            var cat = facade.Create(OperationType.Income, "Salary");
            Assert.Single(facade.GetAll());
            facade.Delete(cat.Id);
            Assert.Empty(facade.GetAll());
        }

        [Fact]
        public void OperationFacade_CanCreateAndDelete()
        {
            var facade = new OperationFacade();
            var op = facade.Create(OperationType.Expense, Guid.NewGuid(), 5, System.DateTime.Now, Guid.NewGuid(), "");
            Assert.Single(facade.GetAll());
            facade.Delete(op.Id);
            Assert.Empty(facade.GetAll());
        }
    }
}
