using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class JobPost
{
    public Guid JobPostId { get; set; }

    public Guid UserId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string? JobDescription { get; set; }

    public string? QualificationRequired { get; set; }

    public DateTime Deadline { get; set; }

    public string Location { get; set; } = null!;

    public string Industry { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual UserCredential User { get; set; } = null!;
}
