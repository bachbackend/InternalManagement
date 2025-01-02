namespace BachBinHoangManagement.DTO
{
    public class TransportDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string SpecificAddress { get; set; } = null!;

        public string Contact { get; set; } = null!;

        public string? Notes { get; set; }

        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; }
        public List<ServiceDTO> Services { get; set; }
    }

    public class ServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class PagingReturn
    {
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
    }

    public class TransportCompanyRequest
    {
        public string Name { get; set; } = null!;

        public string SpecificAddress { get; set; } = null!;

        public string Contact { get; set; } = null!;

        public string? Notes { get; set; }

        public int DistrictId { get; set; }

        //public int ServiceId { get; set; }

        public List<int>? ServiceIds { get; set; }
    }

    public class NotesRequest
    {
        public string Notes { get; set; }
    }

}
