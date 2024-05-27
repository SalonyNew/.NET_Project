using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Application
{
    public Guid ApplicationId { get; set; }

    public Guid JobPostId { get; set; }

    public Guid UserId { get; set; }

    public string? Resume { get; set; }

    public DateTime ApplicationDate { get; set; }

   
    public string? Name { get; set; }
    public string? PhoneNo { get; set; }
    public string? Email {  get; set; }
    public string? Education { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedOn { get; set; }

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual JobPost JobPost { get; set; } = null!;

    public virtual UserCredential User { get; set; } = null!;
}
