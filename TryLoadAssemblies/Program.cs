﻿// See https://aka.ms/new-console-template for more information
using System.Reflection;
using PlatformInterfaces;

Console.WriteLine("Hello, World!");
Assembly assembly = Assembly.LoadFrom("ToBeLoaded2.dll");
// Type myType = assembly.GetType("ToBeLoaded.Class1")!;
// var instanceOfMyType = Activator.CreateInstance(myType)!;
// var method = myType.GetMethod("ItWorks");
// method!.Invoke(instanceOfMyType, null);
Console.WriteLine(assembly.GetName().Name);
foreach (Type type in assembly.GetTypes())
{
    if (typeof(IShowMessage).IsAssignableFrom(type))
    {
        var instanceOfMyType = (IShowMessage)Activator.CreateInstance(type)!;
        instanceOfMyType.ShowMessage();
    }

    if (typeof(IPlatformAbstractStatic).IsAssignableFrom(type))
    {
        var instanceOfMyType = (IPlatformAbstractStatic)Activator.CreateInstance(type)!;
        instanceOfMyType.ShowMessage();
    }
}