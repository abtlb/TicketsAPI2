namespace TicketsAPI.Models
{
    public interface Person
    {
        public string username { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}
