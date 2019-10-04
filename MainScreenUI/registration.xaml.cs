﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FireSharp.Config;
using FireSharp.Response;
using FireSharp.Interfaces;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for registration.xaml
    /// </summary>
    public partial class registration : Page
    {
        public registration()
        {
            InitializeComponent();
        }

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "5JF2869ie6NEZOnxh2YPqEVnvoa9UdttEdaSeKAG",
            BasePath = "https://motiontracker-dd816.firebaseio.com/"
        };


        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegUserName.Text) && string.IsNullOrWhiteSpace(RegPass.Text))
            {
                MessageBox.Show("Please Fill all the fields");
                return;
            }
            else
            {
                User newUser = new User(RegUserName.Text, RegPass.Text);

                SetResponse set = client.Set(@"Users/" + RegUserName.Text, newUser);

                MessageBox.Show("Successfully Registered");
            }
        }

        IFirebaseClient client;
     
        private void RegForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new FireSharp.FirebaseClient(ifc);
            }
            catch
            {
                MessageBox.Show("No Internet or Connection Problem");
            }
        }
    }
}