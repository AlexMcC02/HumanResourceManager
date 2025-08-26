using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using HumanResourceManager.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }

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