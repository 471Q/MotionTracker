using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainScreenUI
{
    class User
    {
        private static string error;

        public String Username { get; set; }

        public String Password { get; set; }

        public User(string user, string pass)
        {
            Username = user;
            Password = pass;
        }

        public static bool Verify(User databaseUser, User localUSer)
        {
            if (databaseUser == null || localUSer == null)
            {
                return false;
            }
            if (databaseUser.Username != localUSer.Username)
            {
                error = "Username does not exist";
                return false;
            }
            else if (databaseUser.Password != localUSer.Password)
            {
                error = "Username and Password does not match!";
                return false;
            }
            return true;
        }
    }
}
