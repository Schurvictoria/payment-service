using System;

namespace FinanceTracker.Domain
{
    /// <summary>
    /// Тип операции: Доход или Расход
    /// </summary>
    public enum OperationType { Income, Expense }

    /// <summary>
    /// Банковский счет пользователя
    /// </summary>
    public class BankAccount
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        /// <summary>
        /// Создает новый счет
        /// </summary>
        public BankAccount(string name, decimal balance = 0)
        {
            Id = Guid.NewGuid();
            Name = name;
            Balance = balance;
        }
    }

    /// <summary>
    /// Категория операции (доход/расход)
    /// </summary>
    public class Category
    {
        public Guid Id { get; }
        public OperationType Type { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Создает новую категорию
        /// </summary>
        public Category(OperationType type, string name)
        {
            Id = Guid.NewGuid();
            Type = type;
            Name = name;
        }
    }

    /// <summary>
    /// Финансовая операция (доход/расход)
    /// </summary>
    public class Operation
    {
        public Guid Id { get; }
        public OperationType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Создает новую операцию
        /// </summary>
        public Operation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            Id = Guid.NewGuid();
            Type = type;
            BankAccountId = bankAccountId;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
            Description = description;
        }
    }

    /// <summary>
    /// Фабрика для создания доменных объектов с валидацией
    /// </summary>
    public static class DomainFactory
    {
        /// <summary>
        /// Создает счет с валидацией
        /// </summary>
        public static BankAccount CreateBankAccount(string name, decimal balance = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Account name cannot be empty");
            return new BankAccount(name, balance);
        }

        /// <summary>
        /// Создает категорию с валидацией
        /// </summary>
        public static Category CreateCategory(OperationType type, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");
            return new Category(type, name);
        }

        /// <summary>
        /// Создает операцию с валидацией
        /// </summary>
        public static Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = "")
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");
            return new Operation(type, bankAccountId, amount, date, categoryId, description);
        }
    }
}
