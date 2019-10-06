﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainScreenUI
{
    class User
    {

        public String Username { get; set; }

        public String Password { get; set; }

        public String Name { get; set; }

        public int Age { get; set; }

        public Double Height { get; set; }

        public Double Weight { get; set; }

        public int Points { get; set; }

        public User(string user, string pass, string name, int age, double height, double weight, int points)
        {
            Username = user;
            Password = pass;
            Name = name;
            Age = age;
            Height = height;
            Weight = weight;
            Points = points;
        }

    }
}
