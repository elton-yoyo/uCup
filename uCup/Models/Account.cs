namespace uCup.Models
{
    public class Account
    {
        public Account(string phone, string password)
        {
            Phone = phone;
            Password = password;
        }

        public string Phone { get; private set; }
        public string Password { get; private set; }
    }
}