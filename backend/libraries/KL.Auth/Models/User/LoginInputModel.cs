namespace KL.Auth.Models.User
{
    public class LoginInputModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}