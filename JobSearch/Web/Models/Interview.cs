using System;
using System.Collections.Generic;

namespace Web.Models;

public partial class Interview
{
    public Guid InterviewId { get; set; }

    public Guid ApplicationId { get; set; }

    public DateTime? InterviewDate { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public virtual Application Application { get; set; } = null!;
}
