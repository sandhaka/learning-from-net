using System;

namespace EventSourcingSourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class EventTypeTargetAttribute : Attribute
{
    
}