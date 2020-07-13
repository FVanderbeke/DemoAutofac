using System;
using System.Collections.Generic;
using Autofac;

namespace DemoAutofac
{
    public interface ILog
    {
        void Print(string log);
    }

    public class ConsoleLog : ILog
    {
        private readonly int _val = new Random().Next();

        public ConsoleLog()
        {
            Console.WriteLine("new console log");
        }

        public void Print(string log)
        {
            Console.WriteLine($"{_val} => {log}");
        }
    }

    public interface IEngine
    {
        void Start();
    }

    public class DieselEngine : IEngine
    {
        private readonly ILog _log;

        public DieselEngine(ILog log)
        {
            Console.WriteLine("new diesel engine");
            this._log = log;
        }

        public void Start()
        {
            _log.Print("Engine started.");
        }
    }

    public class Car
    {
        private readonly IEngine _engine;
        private readonly ILog _log;
        private bool _moving;

        public Car(IEngine engine, ILog log)
        {
            _engine = engine;
            _log = log;
        }

        public void Move()
        {
            if (_moving)
            {
                _log.Print("Car is already moving.");
                return;
            }

            _log.Print("Car is stopped.");
            _engine.Start();
            _log.Print("Car is now moving.");
            _moving = true;
        }
    }

    public class DomainObject
    {
        private readonly int _value;

        public delegate DomainObject Factory(int value);

        public DomainObject(int value)
        {
            _value = value;
        }

        public String StrVal()
        {
            return $"value = {_value}";
        }
    }

    public interface IDisplay
    {
        string Name { get; }

        void Display(string value);
    }
    
    public class MonitorDisplay : IDisplay
    {
        public string Name => "Monitor";
        public void Display(string value)
        {
            Console.WriteLine($"Display{value}");
        }
    }

    public class ConsoleDisplay : IDisplay
    {
        public string Name => "Console";
        public void Display(string value)
        {
            Console.WriteLine($">>{value}");
        }
    }

    public class LogHandler
    {
        private readonly IEnumerable<IDisplay> _displays;

        public LogHandler(IEnumerable<IDisplay> displays)
        {
            _displays = displays;
        }

        public void Log(string value)
        {
            foreach (var display in _displays)
            {
                display.Display(value);
            }
        }
    }
    
    public class Reporting
    {
        private Lazy<LogHandler> _handler;

        
        public Reporting(Lazy<LogHandler> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Test()
        {
            _handler.Value.Log("test");
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));
            builder.RegisterType<ConsoleLog>().As<ILog>().SingleInstance();
            builder.RegisterType<DieselEngine>().As<IEngine>().InstancePerLifetimeScope();
            builder.RegisterType<MonitorDisplay>().As<IDisplay>().SingleInstance();
            builder.RegisterType<ConsoleDisplay>().As<IDisplay>().SingleInstance();
            builder.RegisterType<LogHandler>().SingleInstance();
            
            //builder.Register(c => new Car(c.Resolve<IEngine>(), c.Resolve<ILog>()));
            builder.RegisterType<Car>();
            builder.RegisterType<DomainObject>();

            var container = builder.Build();

            var handler = container.Resolve<LogHandler>();
            
            handler.Log("OK. Found.");
            
            using (var scope = container.BeginLifetimeScope())
            {
                var listInt = scope.Resolve<IList<int>>();
                listInt.Add(2);
                Console.WriteLine("test : " + listInt.Count);

                var listInt2 = scope.Resolve<IList<int>>();
                listInt.Add(3);
                Console.WriteLine("test2 : " + listInt2.Count);

                var listString = scope.Resolve<IList<string>>();
                listString.Add("test");
                Console.WriteLine("test3 : " + listString.Count);

                var car1 = scope.Resolve<Car>();
                car1.Move();

                var car2 = scope.Resolve<Car>();
                car2.Move();

                var domObjFactory = scope.Resolve<DomainObject.Factory>();

                var obj = domObjFactory(3);
                Console.WriteLine(obj.StrVal());
            }
            
            var car3 = container.Resolve<Car>();
            car3.Move();
            
        }
    }
}