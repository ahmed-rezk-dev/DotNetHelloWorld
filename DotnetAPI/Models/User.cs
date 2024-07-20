namespace DotNet.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string FirstName = "";
        public string LastName = "";
        public string Email = "";
        public string Gender = "";
        public bool Active;
    }
}
