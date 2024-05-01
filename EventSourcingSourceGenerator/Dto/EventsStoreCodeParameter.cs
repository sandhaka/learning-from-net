namespace EventSourcingSourceGenerator.Dto;

internal record EventsStoreCodeParameter
{
    public string RootNamespace { get; set; }
    public string TypeNamespace { get; set; }
    public string ClassName { get; set; }
    public string EventClassName { get; set; }
    public string InterfaceName { get; set; }
    public bool Partial { get; set; }
    
}