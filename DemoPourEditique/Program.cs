using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Util;

namespace DemoPourEditique
{

    public interface IPrinter
    {
        void Print(string value);
    }


    public class ConsolePrinter : IPrinter
    {
        private int _counter = 0;
        
        public ConsolePrinter()
        {
            Console.WriteLine("!!!!! ===> new Console printer!!!!");
        }
        
        public void Print(string value)
        {
            _counter++;
            Console.WriteLine($"{_counter}: On console: {value}");
        }
    }
    
    public class DisplayPrinter : IPrinter
    {
        public void Print(string value)
        {
            Console.WriteLine($"On display: {value}");
        }
    }

    public class Reporting: Disposable
    {
        private readonly IEnumerable<IPrinter> _printers;

        public Reporting(IEnumerable<IPrinter> printers)
        {
            _printers = printers ?? throw new ArgumentNullException(nameof(printers));
            Console.WriteLine("Reporting created.");
        }

        public void WriteReport()
        {
            foreach (var printer in _printers)
            {
                printer.Print("Mon rapport complet.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("Reporting disposed");
            base.Dispose(disposing);
        }
    }

    public class Reporter
    {
        private readonly Reporting _reporting;
        private readonly ConsolePrinter _printer;

        public Reporter(Reporting reporting, ConsolePrinter printer)
        {
            _reporting = reporting ?? throw new ArgumentNullException(nameof(reporting));
            _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        }

        public void Report(string something)
        {
            _reporting.WriteReport();
            
            _printer.Print($"Last words: {something}");
        }
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsolePrinter>().As<IPrinter>().AsSelf().SingleInstance();
            builder.RegisterType<DisplayPrinter>().As<IPrinter>().PreserveExistingDefaults().SingleInstance();
            builder.RegisterType<Reporting>().SingleInstance();
            builder.RegisterType<Reporter>().SingleInstance();
            
            using (var container = builder.Build())
            {
                var reporting = container.Resolve<Reporting>();
            
                reporting.WriteReport();                
                
                var reporting2 = container.Resolve<Reporting>();
                reporting2.WriteReport();
                
                
                var reporter = container.Resolve<Reporter>();
                
                reporter.Report("Voila...");
            }
            
            
        }
    }
}