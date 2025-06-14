using System;
using System.IO;
using FinanceTracker.Domain;
using FinanceTracker.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddSingleton<BankAccountFacade>();
            services.AddSingleton<CategoryFacade>();
            services.AddSingleton<OperationFacade>();
            var provider = services.BuildServiceProvider();

            var accountFacade = provider.GetRequiredService<BankAccountFacade>();
            var categoryFacade = provider.GetRequiredService<CategoryFacade>();
            var operationFacade = provider.GetRequiredService<OperationFacade>();

            Console.WriteLine("Добро пожаловать в модуль 'Учет финансов'!");
            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Показать счета");
                Console.WriteLine("3. Создать категорию");
                Console.WriteLine("4. Показать категории");
                Console.WriteLine("5. Добавить операцию");
                Console.WriteLine("6. Показать операции");
                Console.WriteLine("7. Группировка по категориям");
                Console.WriteLine("8. Баланс за период");
                Console.WriteLine("9. Экспортировать данные в CSV");
                Console.WriteLine("0. Выход");
                Console.Write("Ваш выбор: ");
                var input = Console.ReadLine();
                if (input == "0") break;
                try
                {
                    switch (input)
                    {
                        case "1":
                            Console.Write("Название счета: ");
                            var accName = Console.ReadLine();
                            Console.Write("Начальный баланс: ");
                            decimal.TryParse(Console.ReadLine(), out var accBal);
                            var createAccCmd = new CreateAccountCommand(accountFacade, accName, accBal);
                            var timedCmd = new TimingCommandDecorator(createAccCmd);
                            timedCmd.Execute();
                            break;
                        case "2":
                            foreach (var acc in accountFacade.GetAll())
                                Console.WriteLine($"{acc.Id} | {acc.Name} | Баланс: {acc.Balance}");
                            break;
                        case "3":
                            Console.Write("Название категории: ");
                            var catName = Console.ReadLine();
                            Console.Write("Тип (0 - Доход, 1 - Расход): ");
                            Enum.TryParse<OperationType>(Console.ReadLine(), out var catType);
                            categoryFacade.Create(catType, catName);
                            Console.WriteLine("Категория создана.");
                            break;
                        case "4":
                            foreach (var cat in categoryFacade.GetAll())
                                Console.WriteLine($"{cat.Id} | {cat.Name} | Тип: {cat.Type}");
                            break;
                        case "5":
                            Console.Write("ID счета: ");
                            Guid.TryParse(Console.ReadLine(), out var opAccId);
                            var accObj = accountFacade.GetById(opAccId);
                            if (accObj == null)
                            {
                                Console.WriteLine("Ошибка: счет не найден.");
                                break;
                            }
                            Console.Write("ID категории: ");
                            Guid.TryParse(Console.ReadLine(), out var opCatId);
                            var catObj = categoryFacade.GetById(opCatId);
                            if (catObj == null)
                            {
                                Console.WriteLine("Ошибка: категория не найдена.");
                                break;
                            }
                            Console.Write("Сумма: ");
                            decimal.TryParse(Console.ReadLine(), out var opAmount);
                            Console.Write("Тип (0 - Доход, 1 - Расход): ");
                            Enum.TryParse<OperationType>(Console.ReadLine(), out var opType);
                            Console.Write("Описание: ");
                            var opDesc = Console.ReadLine();
                            operationFacade.Create(opType, opAccId, opAmount, DateTime.Now, opCatId, opDesc);
                            Console.WriteLine("Операция добавлена.");
                            break;
                        case "6":
                            foreach (var op in operationFacade.GetAll())
                                Console.WriteLine($"{op.Id} | {op.Type} | {op.Amount} | {op.Date} | {op.Description}");
                            break;
                        case "7":
                            Console.WriteLine("Группировка по категориям (0 - Доход, 1 - Расход): ");
                            Enum.TryParse<OperationType>(Console.ReadLine(), out var groupType);
                            var groups = operationFacade.GroupByCategory(categoryFacade.GetAll(), groupType);
                            foreach (var (cat, total) in groups)
                                Console.WriteLine($"{cat.Name} ({cat.Type}): {total}");
                            break;
                        case "8":
                            Console.Write("Введите дату начала (yyyy-MM-dd): ");
                            DateTime.TryParse(Console.ReadLine(), out var from);
                            Console.Write("Введите дату конца (yyyy-MM-dd): ");
                            DateTime.TryParse(Console.ReadLine(), out var to);
                            var delta = operationFacade.GetBalanceDelta(from, to);
                            Console.WriteLine($"Разница доходов и расходов: {delta}");
                            break;
                        case "9":
                            Console.Write("Введите путь к файлу для экспорта (например, data.csv): ");
                            var path = Console.ReadLine();
                            using (var writer = new StreamWriter(path))
                            {
                                var visitor = new CsvExportVisitor(writer);
                                foreach (var acc in accountFacade.GetAll())
                                    visitor.Visit(acc);
                                foreach (var cat in categoryFacade.GetAll())
                                    visitor.Visit(cat);
                                foreach (var op in operationFacade.GetAll())
                                    visitor.Visit(op);
                            }
                            Console.WriteLine("Данные экспортированы в CSV.");
                            break;
                        default:
                            Console.WriteLine("Неизвестная команда.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка: {ex.Message}");
                }
            }
            Console.WriteLine("До свидания!");
        }
    }
}
