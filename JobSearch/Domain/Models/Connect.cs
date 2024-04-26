using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Connect
{
    public Guid ConnectionId { get; set; }

    public Guid SenderId { get; set; }

    public Guid ReceiverId { get; set; }

    public string? Status { get; set; }

    public DateTime ConnectionDate { get; set; }

    public virtual ICollection<ChatSection> ChatSections { get; set; } = new List<ChatSection>();

    public virtual UserCredential Receiver { get; set; } = null!;

    public virtual UserCredential Sender { get; set; } = null!;
}
