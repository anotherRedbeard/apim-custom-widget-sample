namespace APIMCustomerWidget.Models
{
    public class ApimUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public List<string> Groups { get; set; } = new List<string>();
    }
}