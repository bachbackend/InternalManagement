using System;
using System.Collections.Generic;

namespace BachBinHoangManagement.Models;

public partial class TransportCompany
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string SpecificAddress { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string? Notes { get; set; }

    public int DistrictId { get; set; }

    public int ServiceId { get; set; }

    public virtual District District { get; set; } = null!;

    public virtual ICollection<TransportCompanyService> TransportCompanyServices { get; set; } = new List<TransportCompanyService>();
}
