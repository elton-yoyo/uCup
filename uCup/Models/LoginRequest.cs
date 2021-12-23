namespace uCup.Models
{
    public class LoginRequest
    {
        public LoginRequest(string account, string password)
        {
            Account = account;
            Password = password;
        }

        public string Account { get; private set; }
        public string Password { get; private set; }
    }
}