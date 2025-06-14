using System;
using System.Diagnostics;
using System.IO;
using FinanceTracker.Domain;

namespace FinanceTracker.Infrastructure
{
    public interface ICommand
    {
        void Execute();
    }

    public class CreateAccountCommand : ICommand
    {
        private readonly BankAccountFacade _facade;
        private readonly string _name;
        private readonly decimal _balance;

        public CreateAccountCommand(BankAccountFacade facade, string name, decimal balance = 0)
        {
            _facade = facade;
            _name = name;
            _balance = balance;
        }

        public void Execute() => _facade.Create(_name, _balance);
    }

    public class TimingCommandDecorator : ICommand
    {
        private readonly ICommand _command;
        public TimingCommandDecorator(ICommand command) => _command = command;
        public void Execute()
        {
            var sw = Stopwatch.StartNew();
            _command.Execute();
            sw.Stop();
            Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} мс");
        }
    }

    public abstract class DataImporter
    {
        public void Import(string filePath)
        {
            var data = ReadFile(filePath);
            ParseData(data);
            SaveData();
        }
        protected abstract string ReadFile(string filePath);
        protected abstract void ParseData(string data);
        protected abstract void SaveData();
    }

    public interface IExportVisitor
    {
        void Visit(BankAccount account);
        void Visit(Category category);
        void Visit(Operation operation);
    }

    public class CsvExportVisitor : IExportVisitor
    {
        private readonly TextWriter _writer;
        public CsvExportVisitor(TextWriter writer) => _writer = writer;
        public void Visit(BankAccount account) => _writer.WriteLine($"Account;{account.Id};{account.Name};{account.Balance}");
        public void Visit(Category category) => _writer.WriteLine($"Category;{category.Id};{category.Type};{category.Name}");
        public void Visit(Operation operation) => _writer.WriteLine($"Operation;{operation.Id};{operation.Type};{operation.BankAccountId};{operation.Amount};{operation.Date};{operation.CategoryId};{operation.Description}");
    }
}
