using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Account.Models;

public class User: IdentityUser
{
    [MaxLength(32)]
    public string? GroupId { get; set; }
    [MaxLength(32)]
    public string? TrackId { get; set; }
    
    public MainGroup? MainGroup { get; set; }
    public Track? Track { get; set; }
    public ICollection<UserOpt> UserOpts { get; set; } = new List<UserOpt>();
    
}