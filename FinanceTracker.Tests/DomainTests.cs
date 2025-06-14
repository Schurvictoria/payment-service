using System;
using FinanceTracker.Domain;
using Xunit;

namespace FinanceTracker.Tests
{
    public class DomainTests
    {
        [Fact]
        public void BankAccount_CreatedWithCorrectValues()
        {
            var acc = new BankAccount("Test", 100);
            Assert.Equal("Test", acc.Name);
            Assert.Equal(100, acc.Balance);
            Assert.NotEqual(Guid.Empty, acc.Id);
        }

        [Fact]
        public void Category_CreatedWithCorrectValues()
        {
            var cat = new Category(OperationType.Expense, "Food");
            Assert.Equal(OperationType.Expense, cat.Type);
            Assert.Equal("Food", cat.Name);
            Assert.NotEqual(Guid.Empty, cat.Id);
        }

        [Fact]
        public void Operation_CreatedWithCorrectValues()
        {
            var accId = Guid.NewGuid();
            var catId = Guid.NewGuid();
            var op = new Operation(OperationType.Income, accId, 50, DateTime.Today, catId, "desc");
            Assert.Equal(OperationType.Income, op.Type);
            Assert.Equal(accId, op.BankAccountId);
            Assert.Equal(50, op.Amount);
            Assert.Equal(catId, op.CategoryId);
            Assert.Equal("desc", op.Description);
            Assert.NotEqual(Guid.Empty, op.Id);
        }

        [Fact]
        public void Factory_ThrowsOnInvalidAccount()
        {
            Assert.Throws<ArgumentException>(() => DomainFactory.CreateBankAccount(" "));
        }

        [Fact]
        public void Factory_ThrowsOnInvalidCategory()
        {
            Assert.Throws<ArgumentException>(() => DomainFactory.CreateCategory(OperationType.Income, ""));
        }

        [Fact]
        public void Factory_ThrowsOnInvalidOperation()
        {
            Assert.Throws<ArgumentException>(() => DomainFactory.CreateOperation(OperationType.Expense, Guid.NewGuid(), 0, DateTime.Now, Guid.NewGuid()));
        }
    }
}
