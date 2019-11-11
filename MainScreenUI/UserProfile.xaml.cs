using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            userNameNavPanel.Text = Login.userDetail.Name;
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
            
            UIExerciseCompleted.Text = UserUpdatedPoint.Points.ToString();
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
