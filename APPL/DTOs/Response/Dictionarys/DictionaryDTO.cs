namespace APPL.DTOs.Response.Dictionarys
{
    public class DictionaryDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; } // يمكن التعامل معه كنموذج JSON
    }
}
