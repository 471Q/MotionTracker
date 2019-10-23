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
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class Login : Page
    {
        FireS fib = new FireS();
        public Login()
        {
            InitializeComponent();
            fib.SetIFC();
        }
        

     /*   IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "5JF2869ie6NEZOnxh2YPqEVnvoa9UdttEdaSeKAG",
            BasePath = "https://motiontracker-dd816.firebaseio.com/"
        };
*/
    

        internal static User userDetail;
        internal static FirebaseResponse res;

        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userName.Text) && string.IsNullOrWhiteSpace(pass.Password))
            {
                MessageBox.Show("Please Fill all the fields");
                return;
            }
            else
            {
                FirebaseResponse res = fib.client.Get(@"Users/" + userName.Text);
                LogUser resUser = res.ResultAs<LogUser>(); //firebase result

                userDetail = res.ResultAs<User>(); //all details

                LogUser currUser = new LogUser(userName.Text, pass.Password);

                if (LogUser.Verify(resUser, currUser))
                {
                    UserProfile userProfile = new UserProfile();
                    NavigationService.Navigate(userProfile);
                    ///rest of the application goes here 
                }
                else
                {
                    MessageBox.Show("Wrong");
                }
            }
        }

        private void SignUP_Click(object sender, RoutedEventArgs e)
        {
            Registration reg = new Registration();
            NavigationService.Navigate(reg);
        }


        private void Login1_Loaded(object sender, RoutedEventArgs e)
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
    }
}
