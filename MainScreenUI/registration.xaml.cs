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

    public partial class Registration : Page
    {
        FireS fib = new FireS();
        public Registration()
        {
            InitializeComponent();
            fib.SetIFC();
        }
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegUserName.Text) && string.IsNullOrWhiteSpace(RegPass.Password) && string.IsNullOrWhiteSpace(RegFullName.Text) && string.IsNullOrWhiteSpace(RegAge.Text)
                && string.IsNullOrWhiteSpace(RegHeight.Text) && string.IsNullOrWhiteSpace(RegWeight.Text))
            {
                MessageBox.Show("Please Fill all the fields");
                return;
            }
            else
            {
                User newUser = new User(RegUserName.Text, RegPass.Password, RegFullName.Text, int.Parse(RegAge.Text), Double.Parse(RegHeight.Text), Double.Parse(RegWeight.Text), 0);

                SetResponse set = fib.client.Set(@"Users/" + RegUserName.Text, newUser);

                MessageBox.Show("Successfully Registered");

                Login userLogin = new Login();
                NavigationService.Navigate(userLogin);
            }
        }
        private void RegForm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                fib.SetClient();
            }
            catch
            {
                MessageBox.Show("No Internet or Connection Problem");
            }
        }

        private void BacktoLogin(object sender, RoutedEventArgs e)
        {
            Login userLogin = new Login();
            NavigationService.Navigate(userLogin);
        }
    }
}
