using FireSharp.Response;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media.Imaging;
using System;
using FireSharp.Interfaces;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Page
    {
        Thread thread1;
        FireS fib = new FireS();
        public UserProfile()
        {
            InitializeComponent();
        }

   
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            fib.SetIFC();

            FirebaseResponse res = new FireSharp.FirebaseClient(fib.ifc).Get(@"Users/" + Login.userDetail.Username);
            User UserUpdatedPoint = res.ResultAs<User>(); //firebase result

            userName.Text = Login.userDetail.Name;
            userNameNavPanel.Text = Login.userDetail.Username;
            age.Text = Login.userDetail.Age.ToString();
            height.Text = Login.userDetail.Height.ToString();
            weight.Text = Login.userDetail.Weight.ToString();
            Task.Factory.StartNew(() =>
            {
                thread1 = Thread.CurrentThread;
                while (true)
                {
                    res = new FireSharp.FirebaseClient(fib.ifc).Get(@"Users/" + Login.userDetail.Username);
                    UserUpdatedPoint = res.ResultAs<User>(); //firebase result
                    this.Dispatcher.Invoke(() =>
                    {
                        messageBox.Text = UserUpdatedPoint.Message;
                    });
                    System.Threading.Thread.Sleep(1000);
                }
            });
            //UserUpdatedPoint.ProfileHash = (@"http://www.gravatar.com/avatar/" + HashUserNameForGravatar(userName.Text) + "?size=100&d=identicon");
            UIExerciseCompleted.Text = UserUpdatedPoint.Points.ToString();
            UIAvatar.ImageSource = new BitmapImage(new System.Uri(Login.userDetail.ProfileHash, UriKind.Absolute));
            UIAvatarSmall.ImageSource = new BitmapImage(new System.Uri(Login.userDetail.ProfileHash, UriKind.Absolute));
            Console.WriteLine(new System.Uri(Login.userDetail.ProfileHash, UriKind.Absolute));
        }

        /// Hashes an email with MD5.  Suitable for use with Gravatar profile
        /// image urls
        public static string HashUserNameForGravatar(string username)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            //MD5 md5Hasher = MD5.Create();
            SHA256 sha256Hash = SHA256.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = sha256Hash.ComputeHash(Encoding.Default.GetBytes(username));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            thread1.Abort();
        }

        private void GoToExercise(object sender, RoutedEventArgs e)
        {
            Exercise3 exercise = new Exercise3();
            NavigationService.Navigate(exercise);
        }

        private void GoToPoints(object sender, RoutedEventArgs e)
        {
            Points points = new Points();
            NavigationService.Navigate(points);
        }

        private void GoToLogin(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            NavigationService.Navigate(login);
        }
    }

}
