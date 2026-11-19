using System.ComponentModel.DataAnnotations;

namespace Account.Models;

public class MainGroup
{
    [MaxLength(32)]
    public required string Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<User> Users { get; set; } = new List<User>();

}