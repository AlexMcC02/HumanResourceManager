public class Employee
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string JobRole { get; set; }
    public Band Band { get; set; }
    public decimal Salary { get; set; }
}