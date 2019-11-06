using System;
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
using System.Security.Cryptography;

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
        
        internal static User userDetail;

        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            FirebaseResponse res = null;
            if (string.IsNullOrWhiteSpace(userName.Text) && string.IsNullOrWhiteSpace(pass.Password))
            {
                MessageBox.Show("Please Fill all the fields");
                return;
            }
            else
            {
                try
                {
                    res = fib.client.Get(@"Users/" + userName.Text);
                }
                catch
                {
                    Console.WriteLine("No Internet or Connection Problem");
                    MessageBox.Show("Please check your internet connection");
                    return;
                }
                LogUser resUser = res.ResultAs<LogUser>(); //firebase result

                String passw = Encypt(pass.Password);

                userDetail = res.ResultAs<User>(); //all details

                LogUser currUser = new LogUser(userName.Text, passw);

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

        private string Encypt(string pass)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(pass));
                return Convert.ToBase64String(data);
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
