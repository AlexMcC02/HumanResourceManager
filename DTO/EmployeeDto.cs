namespace DTO;

public class EmployeeDto
{
    public required string FirstName { get; set; }
    public required string SecondName { get; set; }
    public required string JobRole { get; set; }
    public Band Band { get; set; }
    public decimal Salary { get; set; }
}
