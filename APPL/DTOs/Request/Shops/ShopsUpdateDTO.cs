namespace APPL.DTOs.Request.Shops
{
    public class ShopsUpdateDTO : ShopsCreateDTO
    {
        public string TableName { get; set; }
        public int Id { get; set; }


    }

}
