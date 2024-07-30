namespace DotNet.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";
        public bool Active { get; set; }
    }

    public partial class UserDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";
        public bool Active { get; set; }
    }

    public partial class UserRegistrationDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Gender { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordConfirmation { get; set; } = "";
    }

    public partial class UserLoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public partial class UserPasswordConfirmationDto
    {
        public string Password { get; set; } = "";
        public string PasswordConfirmation { get; set; } = "";
    }

    public partial class UserAuth
    {
        public string Email { get; set; } = "";

        public string PasswordHash { get; set; } = "";
        public string PasswordSalt { get; set; } = "";

        // public byte[] PasswordHash { get; set; } = new byte[0];
        // public byte[] PasswordSalt { get; set; } = new byte[0];
    }
}
