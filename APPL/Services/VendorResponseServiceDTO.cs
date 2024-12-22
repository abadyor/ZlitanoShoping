namespace APPL.Services
{
    public class VendorResponseServiceDTO
    {
        public int Id { get; set; }

        public string GivenNames { get; set; }

        public string Nickname { get; set; }

        public string Gender { get; set; }

        public string DocId { get; set; }

        public string DocType { get; set; }

        public string EmailAddress { get; set; }

        public string Mobile { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public DateTime TimestampCreate { get; set; }
    }
}
