using System;
using System.Collections.Generic;

namespace BachBinHoangManagement.Models;

public partial class TransportCompanyService
{
    public int TransportCompanyId { get; set; }

    public int ServiceId { get; set; }

    public DateTime CreateAt { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual TransportCompany TransportCompany { get; set; } = null!;
}
