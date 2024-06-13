using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class UserCredential
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public Guid RoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedOn { get; set; }

    public string? PhoneNo { get; set; }

    public DateTime Dob { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Connect> ConnectReceivers { get; set; } = new List<Connect>();

    public virtual ICollection<Connect> ConnectSenders { get; set; } = new List<Connect>();

    public virtual ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();

    public virtual Role Role { get; set; } = null!;
}
