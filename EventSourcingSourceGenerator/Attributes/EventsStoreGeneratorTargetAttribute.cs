using System;

namespace EventSourcingSourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class EventsStoreGeneratorTargetAttribute : Attribute
{
    
}