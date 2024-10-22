namespace KL.Manager.API.Application.Models.Responses.UserFilter;

public class FilterResponse
{
    public string Name { get; set; } = null!;
    public FilterFieldsResponse[] Fields { get; set; } = Array.Empty<FilterFieldsResponse>();
}

public class FilterFieldsResponse
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string FieldName { get; set; } = null!;
}