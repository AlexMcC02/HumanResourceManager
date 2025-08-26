using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanResourceManager.Models;

[Table("employees")]
public class Employee
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("first_name")]
    public required string FirstName { get; set; }
    [Column("second_name")]
    public required string SecondName { get; set; }
    [Column("job_role")]
    public required string JobRole { get; set; }
    [Column("band")]
    public Band Band { get; set; }
    [Column("salary")]
    public decimal Salary { get; set; }
}