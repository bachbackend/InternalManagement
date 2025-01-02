using System;
using System.Collections.Generic;

namespace BachBinHoangManagement.Models;

public partial class District
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ProvinceId { get; set; }

    public virtual ICollection<ProvidedCompany> ProvidedCompanies { get; set; } = new List<ProvidedCompany>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<TransportCompany> TransportCompanies { get; set; } = new List<TransportCompany>();
}
