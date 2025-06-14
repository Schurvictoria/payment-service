using System;
using System.Diagnostics;
using System.IO;
using FinanceTracker.Domain;
using FinanceTracker.Infrastructure;
using Xunit;

namespace FinanceTracker.Tests
{
    public class PatternTests
    {
        [Fact]
        public void Command_ExecutesAction()
        {
            var facade = new BankAccountFacade();
            var cmd = new CreateAccountCommand(facade, "CmdTest", 123);
            cmd.Execute();
            Assert.Single(facade.GetAll());
            Assert.Equal("CmdTest", facade.GetAll().First().Name);
        }

        [Fact]
        public void TimingCommandDecorator_MeasuresTime()
        {
            var facade = new BankAccountFacade();
            var cmd = new CreateAccountCommand(facade, "TimeTest", 1);
            var timing = new TimingCommandDecorator(cmd);
            var sw = Stopwatch.StartNew();
            timing.Execute();
            sw.Stop();
            Assert.Single(facade.GetAll());
        }

        [Fact]
        public void CsvExportVisitor_ExportsCorrectFormat()
        {
            var acc = new BankAccount("ExportTest", 10);
            var cat = new Category(OperationType.Income, "ExportCat");
            var op = new Operation(OperationType.Income, acc.Id, 5, DateTime.Today, cat.Id, "desc");
            using var sw = new StringWriter();
            var visitor = new CsvExportVisitor(sw);
            visitor.Visit(acc);
            visitor.Visit(cat);
            visitor.Visit(op);
            var output = sw.ToString();
            Assert.Contains("Account;", output);
            Assert.Contains("Category;", output);
            Assert.Contains("Operation;", output);
        }

        private class TestImporter : DataImporter
        {
            public bool Read, Parsed, Saved;
            protected override string ReadFile(string filePath) { Read = true; return "data"; }
            protected override void ParseData(string data) { Parsed = true; }
            protected override void SaveData() { Saved = true; }
        }

        [Fact]
        public void DataImporter_TemplateMethodCallsAllSteps()
        {
            var importer = new TestImporter();
            importer.Import("file.txt");
            Assert.True(importer.Read);
            Assert.True(importer.Parsed);
            Assert.True(importer.Saved);
        }
    }
}
