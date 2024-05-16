using System;

namespace EventSourcingSourceGenerator.Attributes;

/// <summary>
/// Specifies the base type and target for storing events in a database table or collection.
/// </summary>
/// <param name="DateTimeEventPropertyName">Name of a DateTime event property used as timestamp</param>
/// <param name="dbName">Name of the database</param>
/// <param name="TableOrCollectionName">Name of database table or collection to store events</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class EventBaseTypeTargetAttribute(string DateTimeEventPropertyName, string dbName, string TableOrCollectionName) : Attribute
{
}