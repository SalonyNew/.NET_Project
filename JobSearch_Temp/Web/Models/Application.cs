using System;
using System.Collections.Generic;

namespace Web.Models;

public partial class Application
{
    public Guid ApplicationId { get; set; }

    public Guid JobPostId { get; set; }

    public Guid UserId { get; set; }

    public byte[]? Resume { get; set; }

    public byte[]? CoverLetter { get; set; }

    public DateTime ApplicationDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual JobPost JobPost { get; set; } = null!;

    public virtual UserCredential User { get; set; } = null!;
}
