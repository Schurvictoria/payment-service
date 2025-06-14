using System;
using System.Collections.Generic;
using System.Linq;
using FinanceTracker.Domain;

namespace FinanceTracker.Infrastructure
{
    public class BankAccountFacade
    {
        private readonly List<BankAccount> _accounts = new();

        public BankAccount Create(string name, decimal balance = 0)
        {
            var account = DomainFactory.CreateBankAccount(name, balance);
            _accounts.Add(account);
            return account;
        }

        public void Delete(Guid id) => _accounts.RemoveAll(a => a.Id == id);
        public IEnumerable<BankAccount> GetAll() => _accounts;
        public BankAccount? GetById(Guid id) => _accounts.FirstOrDefault(a => a.Id == id);
    }

    public class CategoryFacade
    {
        private readonly List<Category> _categories = new();

        public Category Create(OperationType type, string name)
        {
            var category = DomainFactory.CreateCategory(type, name);
            _categories.Add(category);
            return category;
        }

        public void Delete(Guid id) => _categories.RemoveAll(c => c.Id == id);
        public IEnumerable<Category> GetAll() => _categories;
        public Category? GetById(Guid id) => _categories.FirstOrDefault(c => c.Id == id);
    }

    public class OperationFacade
    {
        private readonly List<Operation> _operations = new();

        public Operation Create(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            var operation = DomainFactory.CreateOperation(type, bankAccountId, amount, date, categoryId, description);
            _operations.Add(operation);
            return operation;
        }

        public void Delete(Guid id) => _operations.RemoveAll(o => o.Id == id);
        public IEnumerable<Operation> GetAll() => _operations;
        public Operation? GetById(Guid id) => _operations.FirstOrDefault(o => o.Id == id);

        public IEnumerable<(Category Category, decimal Total)> GroupByCategory(IEnumerable<Category> categories, OperationType type)
        {
            return _operations
                .Where(o => o.Type == type)
                .GroupBy(o => o.CategoryId)
                .Select(g => (categories.FirstOrDefault(c => c.Id == g.Key), g.Sum(o => o.Amount)))
                .Where(x => x.Item1 != null)!;
        }

        public decimal GetBalanceDelta(DateTime from, DateTime to)
        {
            var income = _operations.Where(o => o.Type == OperationType.Income && o.Date >= from && o.Date <= to).Sum(o => o.Amount);
            var expense = _operations.Where(o => o.Type == OperationType.Expense && o.Date >= from && o.Date <= to).Sum(o => o.Amount);
            return income - expense;
        }
    }
}
