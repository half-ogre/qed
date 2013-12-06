namespace qed
{
    public class User
    {
        public string EmailAddress { get; set; }
        public int Id { get; set; }
        public bool IsAdministrator { get; set; }
        public string PasswordHash { get; set; }
        public string Username { get; set; }
    }
}