using System;

namespace MainScreenUI
{
    class User
    {
        public String Username { get; set; }

        public String Password { get; set; }

        public String Message { get; set; }

        public String Name { get; set; }

        public int Age { get; set; }

        public Double Height { get; set; }

        public Double Weight { get; set; }

        public int Points { get; set; }

        public int MaxPoints { get; set; }

        public User(string user, string pass, string name, string message, int age, double height, double weight, int points, int maxPoints)
        {
            Username = user;
            Password = pass;
            Name = name;
            Message = message;
            Age = age;
            Height = height;
            Weight = weight;
            Points = points;
            MaxPoints = maxPoints;
        }
    }
}
