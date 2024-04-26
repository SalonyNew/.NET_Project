using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ChatSection
{
    public Guid ChatId { get; set; }

    public Guid ConnectionId { get; set; }

    public string? Content { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? Status { get; set; }

    public virtual Connect Connection { get; set; } = null!;
}
