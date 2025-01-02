using System;
using System.Collections.Generic;

namespace BachBinHoangManagement.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TransportCompanyService> TransportCompanyServices { get; set; } = new List<TransportCompanyService>();
}
