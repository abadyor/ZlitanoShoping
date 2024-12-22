namespace APPL.DTOs.Request.Shops
{
    public class ShopsUpdateLockDTO
    {
        public string TableName { get; set; }
        public int Id { get; set; }
        public bool IsLock { get; set; }
    }
}
