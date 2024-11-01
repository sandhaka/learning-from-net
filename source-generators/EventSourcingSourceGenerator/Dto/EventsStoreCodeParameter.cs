namespace EventSourcingSourceGenerator.Dto;

internal record EventsStoreCodeParameter
{
    public string RootNamespace { get; set; }
    public string TypeNamespace { get; set; }
    public string ClassName { get; set; }
    public string EventClassName { get; set; }
    public string DateTimeEventPropertyName { get; set; }
    public string TableOrCollectionName { get; set; }
    public string Template { get; set; }
    public string DatabaseName { get; set; }
    
    // Only support mongodb now: not used, implicit.
    public string Provider { get; set; } = "mongodb";
}