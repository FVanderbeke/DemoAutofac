﻿using System;

namespace DemoAutofac
{
    public class DefaultPrinter: IPrinter
    {
        public void Print(string content)
        {
            Console.WriteLine("Printed : " + content);
        }

        public string Name()
        {
            return "Default";
        }
    }
}