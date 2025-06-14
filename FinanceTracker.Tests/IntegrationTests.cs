using System;
using System.Linq;
using FinanceTracker.Domain;
using FinanceTracker.Infrastructure;
using Xunit;

namespace FinanceTracker.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void FullScenario_CreateAccountCategoryOperation()
        {
            var accFacade = new BankAccountFacade();
            var catFacade = new CategoryFacade();
            var opFacade = new OperationFacade();

            var acc = accFacade.Create("Интеграционный счет", 1000);
            var cat = catFacade.Create(OperationType.Expense, "Еда");
            var op = opFacade.Create(OperationType.Expense, acc.Id, 250, DateTime.Today, cat.Id, "Обед");

            Assert.Single(accFacade.GetAll());
            Assert.Single(catFacade.GetAll());
            Assert.Single(opFacade.GetAll());
            Assert.Equal(acc.Id, op.BankAccountId);
            Assert.Equal(cat.Id, op.CategoryId);
            Assert.Equal(250, op.Amount);
            Assert.Equal("Обед", op.Description);
        }

        [Fact]
        public void CreateOperation_WithNonexistentAccount_Throws()
        {
            var opFacade = new OperationFacade();
            var catId = Guid.NewGuid();
            Assert.Throws<ArgumentException>(() => opFacade.Create(OperationType.Income, Guid.NewGuid(), 100, DateTime.Now, catId));
        }

        [Fact]
        public void CreateOperation_WithNonexistentCategory_Throws()
        {
            var accFacade = new BankAccountFacade();
            var opFacade = new OperationFacade();
            var acc = accFacade.Create("Test", 10);
            Assert.Throws<ArgumentException>(() => opFacade.Create(OperationType.Income, acc.Id, 100, DateTime.Now, Guid.NewGuid()));
        }

        [Fact]
        public void CreateOperation_WithNegativeAmount_Throws()
        {
            var accFacade = new BankAccountFacade();
            var catFacade = new CategoryFacade();
            var opFacade = new OperationFacade();
            var acc = accFacade.Create("Test", 10);
            var cat = catFacade.Create(OperationType.Income, "Salary");
            Assert.Throws<ArgumentException>(() => opFacade.Create(OperationType.Income, acc.Id, -100, DateTime.Now, cat.Id));
        }
    }
}
