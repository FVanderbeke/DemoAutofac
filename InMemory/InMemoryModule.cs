﻿using Autofac;
using InMemory.Infrastructure;

namespace InMemory
{
    public class InMemoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryServiceB>();
        }
    }
}