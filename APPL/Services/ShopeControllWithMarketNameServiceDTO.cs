namespace APPL.Services
{
    public class ShopeControllWithMarketNameServiceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Market_Code { get; set; }
        public string Last_VendorCode { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        public string NerestPoint { get; set; }
        public long NVistor { get; set; }
        public int Vendor_Id { get; set; }
        public bool IsLock { get; set; }
        public string StartWork { get; set; }
        public string EndWork { get; set; }
        public TimeOnly? StartWorkTime { get; set; } // القيم المحولة
        public TimeOnly? EndWorkTime { get; set; }   // القيم المحولة
        public string Notes { get; set; } = string.Empty;
        public string MarketName { get; set; }
        public List<string> Images { get; set; }
        public string Path { get; set; }
    }
}
