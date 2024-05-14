using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Profile
{
    public Guid ProfileId { get; set; }

    public Guid UserId { get; set; }

    public string? Education { get; set; }

    public string? WorkExperience { get; set; }

    public string? Skills { get; set; }

    public byte[] Resume { get; set; } = null!;

    public string? CompanyName { get; set; }

    public string? CompanyDescription { get; set; }

    public string? Mission { get; set; }

    public virtual UserCredential User { get; set; } = null!;
}
