using FireSharp.Response;
using KinectCoordinateMapping;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;

namespace MainScreenUI
{
    /// <summary>
    /// Interaction logic for Exercise3.xaml
    /// </summary>
    public partial class Exercise3 : Page
    {
        KinectSensor _sensor = null;
        MultiSourceFrameReader _reader = null;
        IList<Body> _bodies = null;
        VisualGestureBuilderDatabase database = null;
        GestureDetector detector = null;
        List<FileInfo> selectedFiles = new List<FileInfo>();
        GestureResultView result = new GestureResultView(0, false, false, 0.0f);
        FireS fib = new FireS();

        public string gestureText = "";
        private string selectedDb = "";

        public Exercise3()
        {
            InitializeComponent();
            DataContext = result;
        }

        /// <summary>
        /// On Exercise page load, we try to open the kinect sensor and
        /// Fetch user profile icon and name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Connect_Kinnect(false);
            detector = new GestureDetector(_sensor, result);

            fib.SetIFC();

            FirebaseResponse res = new FireSharp.FirebaseClient(fib.ifc).Get(@"Users/" + Login.userDetail.Username);
            User UserUpdatedPoint = res.ResultAs<User>(); //firebase result

            userName.Text = Login.userDetail.Name;
        }

        /// <summary>
        /// Search and try to connect to available Kinect Sensor
        /// </summary>
        /// <param name="attachEvent">Boolean value whether to activate or deativate Kinect's Reader Event to optimize resources usage</param>
        private void Connect_Kinnect(bool attachEvent)
        {
            try
            {
                _sensor = KinectSensor.GetDefault();

                _sensor.Open();

                if (_sensor != null && _reader != null) {
                    _reader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
                    _reader.Dispose();
                    _reader = null;
                    //_reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
                    //_reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
                }

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);

                if(attachEvent)
                    _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
                else
                    _reader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
            }
            catch
            {
                Console.WriteLine("Communication error connecting to Kinect Sensor!");
            }
        }

        /// <summary>
        /// Specify the directory which contain the gesture files
        /// </summary>
        private void Add_file(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog
            {
                InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}"
            };

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            try
            {
                FileInfo _file = new FileInfo(dialog.FileName.ToString());
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    {
                        selectedFiles.Add(_file);

                        //Add Category
                        Button newCategoryButton = new System.Windows.Controls.Button();
                        TextBlock newTextBlock = new System.Windows.Controls.TextBlock();

                        newTextBlock.Style = Resources["WrappedTextBlock"] as Style;
                        newCategoryButton.Style = Resources["RoundedBlueButtonRow0Column0"] as Style;
                        newTextBlock.Text = _file.Name.Remove(_file.Name.Length-4);
                        newCategoryButton.Content = newTextBlock;
                        //newCategoryButton.Content = _file.Name;
                        UICategory.Children.Insert(UICategory.Children.Count - 1, newCategoryButton);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// List all of the gestures available in selected gesture database;
        /// Add buttons with the gesture name available in the selected gesture database respectively
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UICategoryButtonClick(object sender, RoutedEventArgs e)
        {
            UIExercises.Children.RemoveRange(1, UIExercises.Children.Count - 1);
            foreach (FileInfo _file in selectedFiles)
                //var textBlockInButton = ((Button)e.OriginalSource).Content as TextBlock;
                if (_file.Name.Equals(((TextBlock)((Button)e.OriginalSource).Content).Text + ".gbd"))
                    using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(_file.FullName))
                    {
                        foreach (Gesture gesture in database.AvailableGestures)
                        {
                            if (!gesture.Equals(null))
                            {
                                Button newExerciseButton = new System.Windows.Controls.Button();
                                TextBlock newTextBlock = new System.Windows.Controls.TextBlock();

                                newTextBlock.Style = Resources["WrappedTextBlock"] as Style;
                                newExerciseButton.Style = Resources["RoundedBlueButtonRow1Column0ColumnSpan2"] as Style;

                                newTextBlock.Text = gesture.Name.Replace("_", " ");
                                newExerciseButton.Content = newTextBlock;
                                //newExerciseButton.Content = gesture.Name;
                                Console.WriteLine(gesture.Name);
                                UIExercises.Children.Add(newExerciseButton);
                            }
                            else
                            {
                                Console.WriteLine("No gesture found!");
                            }
                        }
                    }
        }

        /// <summary>
        /// Handles the inputs from the kinect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            //Initialization
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                    UICameraOutput.Source = frame.ToBitmap();
            }

            // Gesture detection
            //bool dataReceived = false;
            using (BodyFrame bodyFrame = reference.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    UICanvasOutput.Children.Clear();
                    // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                    _bodies = new Body[bodyFrame.BodyCount];
                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(_bodies);
                    //dataReceived = true;
                    foreach (Body _body in _bodies)
                    {
                        if (_body != null)
                            if (_body.IsTracked && _body.TrackingId != 0)
                            {
                                //detector.IsPaused = false;
                                ulong trackingId = _body.TrackingId;
                                detector.TrackingId = trackingId;
                                detector.IsPaused = trackingId == 0;

                                // COORDINATE MAPPING
                                foreach (Joint joint in _body.Joints.Values)
                                {
                                    if (joint.TrackingState == TrackingState.Tracked)
                                    {
                                        // 3D space point
                                        CameraSpacePoint jointPosition = joint.Position;

                                        // 2D space point
                                        Point point = new Point();

                                        ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
                                        point.X = (float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X);
                                        point.Y = (float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y);

                                        // Draw
                                        Ellipse ellipse = new Ellipse
                                        {
                                            Fill = Brushes.Red,
                                            Width = 20,
                                            Height = 20
                                        };

                                        Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
                                        Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

                                        UICanvasOutput.Children.Add(ellipse);
                                    }
                                }
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Free resources on page unload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_reader != null)
            {
                // BodyFrameReader is IDisposable
                _reader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
                _reader.Dispose();
                _reader = null;
            }
            detector.Dispose();
            if (_sensor != null)
            {
                _sensor.Close();
                _sensor = null;
            }
        }

        private void UIExerciseButtonClick(object sender, RoutedEventArgs e)
        {
            //((System.Windows.Controls.Button)sender).Content;
            //selectedGesture = ((ListBox)sender).SelectedItem.ToString();
            Console.WriteLine(String.Concat("Clicked item is ", ((TextBlock)((Button)e.OriginalSource).Content).Text));
            foreach (FileInfo _file in selectedFiles)
                using (database = new VisualGestureBuilderDatabase(_file.FullName))
                    foreach (Gesture gesture in database.AvailableGestures)
                        if (gesture.Name.Equals(((TextBlock)((Button)e.OriginalSource).Content).Text.Replace(" ", "_").ToString()))
                            selectedDb = _file.FullName;
            if (((TextBlock)((Button)e.OriginalSource).Content).Text != null && selectedDb != null)
            {
                detector.VGBPath = selectedDb;
                detector.GestureName = ((TextBlock)((Button)e.OriginalSource).Content).Text.Replace(" ", "_").ToString();
                //Console.WriteLine(String.Concat("detector.GestureName is ", ((TextBlock)((Button)e.OriginalSource).Content).Text.Replace(" ", "_").ToString()));
                //Console.WriteLine(String.Concat("detector.GestureName is ", detector.GestureName));
            }
            //Console.WriteLine(String.Concat("DataContext ", Newtonsoft.Json.JsonConvert.SerializeObject(DataContext.ToString(), Formatting.Indented)));

            //UIGesture = Convert.ToString(UIListBox.SelectedItem);
            Connect_Kinnect(true);
        }

        private void GoToPoints(object sender, RoutedEventArgs e)
        {
            Points points = new Points();
            NavigationService.Navigate(points);
        }

        private void GoToProfile(object sender, RoutedEventArgs e)
        {
            UserProfile profile = new UserProfile();
            NavigationService.Navigate(profile);
        }
    }
}