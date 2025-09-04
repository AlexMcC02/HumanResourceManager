using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanResourceManager.Models;

[Table("btokens")]
public class BlacklistedToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("token")]
    public required string Token { get; set; }
}