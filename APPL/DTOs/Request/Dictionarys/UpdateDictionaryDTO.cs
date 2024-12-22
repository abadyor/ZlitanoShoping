namespace APPL.DTOs.Request.Dictionarys
{
    public class UpdateDictionaryDTO
    {
        public int Id { get; set; } // يجب تحديد المعرف لتعديل العنصر
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
    }
}
