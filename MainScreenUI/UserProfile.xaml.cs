using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            messageBox.Text = Login.userDetail.Message;
            UIExerciseCompleted.Text = UserUpdatedPoint.Points.ToString();
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
    }

}
