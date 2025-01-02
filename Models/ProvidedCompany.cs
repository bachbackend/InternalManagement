using System;
using System.Collections.Generic;

namespace BachBinHoangManagement.Models;

public partial class ProvidedCompany
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? JobTitle { get; set; }

    public string CompanyName { get; set; } = null!;

    public string SpecificAddress { get; set; } = null!;

    public string Contact { get; set; } = null!;

    public string KilnCharacteristics { get; set; } = null!;

    public string MaximumCapacity { get; set; } = null!;

    public string Product { get; set; } = null!;

    public string? ProductQuote { get; set; }

    public string? Bill { get; set; }

    public string? Notes { get; set; }

    public int DistrictId { get; set; }

    public virtual District District { get; set; } = null!;
}
