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

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Page
    {
        public login()
        {
            InitializeComponent();
        }

        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "5JF2869ie6NEZOnxh2YPqEVnvoa9UdttEdaSeKAG",
            BasePath = "https://motiontracker-dd816.firebaseio.com/"
        };

        internal static string UserName = "";

        private void Login1_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userName.Text) && string.IsNullOrWhiteSpace(pass.Text))
            {
                MessageBox.Show("Please Fill all the fields");
                return;
            }
            else
            {
                FirebaseResponse res = client.Get(@"Users/" + userName.Text);
                User resUser = res.ResultAs<User>(); //firebase result

                User currUser = new User(userName.Text, pass.Text);

                if (User.Verify(resUser, currUser))
                {
                    MessageBox.Show("Logged in");

                    UserName = resUser.Username;
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
            registration reg = new registration();
            NavigationService.Navigate(reg);
        }


        IFirebaseClient client;
        private void Login1_Loaded(object sender, RoutedEventArgs e)
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
