namespace APPL.DTOs.Response.Market
{
    public class MarketGetAllResponseDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Mcode { get; set; }

        public string MInstance { get; set; }
        public bool IsLock { get; set; } = false;
    }
}
