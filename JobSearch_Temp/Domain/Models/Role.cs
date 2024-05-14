using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Role
{
    public Guid RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserCredential> UserCredentials { get; set; } = new List<UserCredential>();
}
