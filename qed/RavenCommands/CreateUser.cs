using System;

namespace qed
{
    public static partial class Functions
    {
        public static User CreateUser(
            string username,
            string password,
            string emailAddress)
        {
            if (String.IsNullOrEmpty(username)) throw new ArgumentException("Username must not be null or empty.", "username");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Password must not be null or empty.", "password");

            using (var ravenSession = OpenRavenSession())
            {
                var user = new User
                {
                    Username = username,
                    PasswordHash = GeneratePasswordHash(password),
                    EmailAddress = emailAddress
                };

                ravenSession.Store(user);
                ravenSession.SaveChanges();

                return user;
            }
        }
    }
}
