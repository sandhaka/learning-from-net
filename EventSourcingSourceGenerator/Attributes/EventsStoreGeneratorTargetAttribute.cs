using System;

namespace EventSourcingSourceGenerator.Attributes;

/// <summary>
/// Represents an attribute that indicates that a class should be considered as a target for the Event Store generator.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class EventsStoreGeneratorTargetAttribute() : Attribute
{
}