using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Interview
{
    public Guid InterviewId { get; set; }

    public Guid ApplicationId { get; set; }

    public DateTime InterviewDate { get; set; }
    public TimeOnly Time {  get; set; }


    public string? Location { get; set; }

    public virtual Application Application { get; set; } = null!;
}
