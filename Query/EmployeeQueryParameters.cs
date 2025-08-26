namespace HumanResourceManager.Query;

public class EmployeeQueryParameters
{
    public string? JobRole { get; set; }
    public string? Band { get; set; }
    public int? MinSalary { get; set; }
    public int? MaxSalary { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}