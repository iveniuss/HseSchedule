using System.ComponentModel.DataAnnotations;

namespace Account.Models;

public class OptGroup
{
    [MaxLength(32)]
    public required string Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<UserOpt> UserOpts { get; set; } = new List<UserOpt>();
}