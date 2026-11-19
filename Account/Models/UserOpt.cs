using System.ComponentModel.DataAnnotations;

namespace Account.Models;

public class UserOpt
{
    public required string UserId { get; set; }
    [MaxLength(32)]
    public required string OptGroupId { get; set; }
    
    public OptGroup? OptGroup { get; set; }
    public User? User { get; set; }
}