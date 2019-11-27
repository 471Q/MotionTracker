using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FireSharp.Response;
using System.Security.Cryptography;

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
               

                String passw = Encypt(RegPass.Password);

                if (!existUser(RegUserName.Text, passw))
                {
                    User newUser = new User(RegUserName.Text, passw, RegFullName.Text, "No message!", int.Parse(RegAge.Text), Double.Parse(RegHeight.Text), Double.Parse(RegWeight.Text), 0, 100, (@"http://www.gravatar.com/avatar/" + HashUserNameForGravatar(RegUserName.Text) + "?size=100&d=identicon"));
                    //User newUser = new User(RegUserName.Text, passw, RegFullName.Text, "No message!", int.Parse(RegAge.Text), Double.Parse(RegHeight.Text), Double.Parse(RegWeight.Text), 0, 10, (@"http://www.gravatar.com/avatar/" + HashUserNameForGravatar(RegUserName.Text) + "?size=100&d=identicon"));


                    SetResponse set = fib.client.Set(@"Users/" + RegUserName.Text, newUser);

                    MessageBox.Show("Successfully Registered");

                    Login userLogin = new Login();
                    NavigationService.Navigate(userLogin);
                }
                else
                {
                    MessageBox.Show("User already exists");
                }
               
            }
        }

        private Boolean existUser(String userName, String passw)
        {
            FirebaseResponse res = null;
            try
                {
                    res = fib.client.Get(@"Users/" + userName);
                }
                catch
                {
                    Console.WriteLine("No Internet or Connection Problem");
                    MessageBox.Show("Please check your internet connection");
                    return false;
                }
                LogUser resUser = res.ResultAs<LogUser>(); //firebase result

                LogUser currUser = new LogUser(userName, passw);

                if (LogUser.Verify(resUser, currUser))
                {
                    return true;
                }
                else
                {
                    return false;
                 }
        }

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

        private string Encypt(string pass)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(pass));
                return Convert.ToBase64String(data);
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
