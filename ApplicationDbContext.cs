using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using HumanResourceManager.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        SeedDatabase();
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<User> Users { get; set; }

    private void SeedDatabase()
    {
        var rnd = new Random();
        string[] firstNames = ["Alex", "Stephen", "Jacob", "Mary", "Bob", "Alice", "Karen", "Samuel", "Lee", "Dylan"];
        string[] secondNames = ["Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson"];
        string[] jobRoles = ["Software Engineer", "Project Manager", "Data Analyst", "Product Owner", "UX Designer", "QA Tester", "DevOps Engineer", "Business Analyst"];
        var enumValues = Enum.GetValues(typeof(Band));

        const int NUM_TO_CREATE = 25;
        const int MIN_SALARY_FACTOR = 19; // 19 * 1000 = $19,000
        const int MAX_SALARY_FACTOR = 450; // 450 * 1000 = $450,000

        foreach (var employee in Employees)
        {
            Employees.Remove(employee);
        }

        for (var i = 0; i < NUM_TO_CREATE; i++)
        {

            Employees.Add(new Employee
            {
                FirstName = firstNames[rnd.Next(0, firstNames.Length - 1)],
                SecondName = secondNames[rnd.Next(0, secondNames.Length - 1)],
                JobRole = jobRoles[rnd.Next(0, jobRoles.Length - 1)],
                Band = (Band)enumValues.GetValue(rnd.Next(enumValues.Length)),
                Salary = rnd.Next(MIN_SALARY_FACTOR, MAX_SALARY_FACTOR) * 1000
            });
        }

        SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var bandConvertor = new ValueConverter<Band, string>(
            v => v.ToString().ToLower(),
            v => Enum.Parse<Band>(CultureInfo.InvariantCulture.TextInfo.ToTitleCase(v))
        );

        modelBuilder.Entity<Employee>()
            .Property(e => e.Band)
            .HasConversion(bandConvertor);
    }
}