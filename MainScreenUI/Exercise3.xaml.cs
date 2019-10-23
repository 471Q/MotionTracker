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
        List<GestureDetector> gestureDetectorList = new List<GestureDetector>();
        VisualGestureBuilderDatabase database = null;
        GestureDetector detector = null;
        List<FileInfo> selectedFiles = new List<FileInfo>();
        GestureResultView result = new GestureResultView(0, false, false, 0.0f);
        int countOfExerciseCompleted = 0;

        public string gestureText = "";
        private string selectedDb = "";

        public Exercise3()
        {
            InitializeComponent();
            DataContext = result;
        }

        /// <summary>
        /// On Exercise page load, we try to open the kinect sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            _sensor.Open();

            if (_sensor != null)
            {
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
            }
            if (_sensor == null)
                Console.WriteLine("Can't find kinect sensor!");
            detector = new GestureDetector(_sensor, result);
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
                        newCategoryButton.Style = Resources["RoundedBlueButtonRow0Column0"] as Style;
                        newCategoryButton.Content = _file.Name;
                        UICategory.Children.Insert(UICategory.Children.Count - 1, newCategoryButton);
                    }
                }
            }
            catch { }
        }

        private void UICategoryButtonClick(object sender, RoutedEventArgs e)
        {
            UIExercises.Children.RemoveRange(1, UIExercises.Children.Count - 1);
            foreach (FileInfo _file in selectedFiles)
                if (_file.Name.Equals(((Button)e.OriginalSource).Content))
                    using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(_file.FullName))
                    {
                        foreach (Gesture gesture in database.AvailableGestures)
                        {
                            if (gesture.Name.Contains("Progress"))
                            {
                                Button newExerciseButton = new System.Windows.Controls.Button();
                                newExerciseButton.Style = Resources["RoundedBlueButtonRow1Column0ColumnSpan2"] as Style;
                                newExerciseButton.Content = gesture.Name;
                                Console.WriteLine(gesture.Name);
                                UIExercises.Children.Add(newExerciseButton);
                            }
                            else {
                                Console.WriteLine("No files found!");
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
                    countOfExerciseCompleted++;
                    foreach (Body _body in _bodies)
                    {
                        if (_body != null)
                            if (_body.IsTracked && _body.TrackingId != 0)
                            {
                                detector.IsPaused = false;
                                ulong trackingId = _body.TrackingId;
                                detector.TrackingId = trackingId;
                                //detector.IsPaused = trackingId == 0;

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
            Console.WriteLine(String.Concat("Clicked item is ", ((Button)e.OriginalSource).Content));
            foreach (FileInfo _file in selectedFiles)
                using (database = new VisualGestureBuilderDatabase(_file.FullName))
                    foreach (Gesture gesture in database.AvailableGestures)
                        if (gesture.Name.Equals(((Button)e.OriginalSource).Content))
                            selectedDb = _file.FullName;
            if (((Button)e.OriginalSource).Content != null && selectedDb != null)
            {
                detector.VGBPath = selectedDb;
                detector.GestureName = ((Button)e.OriginalSource).Content.ToString();
            }
            //Console.WriteLine(String.Concat("DataContext ", Newtonsoft.Json.JsonConvert.SerializeObject(DataContext.ToString(), Formatting.Indented)));

            //UIGesture = Convert.ToString(UIListBox.SelectedItem);
            try
            {
                _reader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
            catch
            {
                Console.WriteLine("Error in handling _reader");
            }
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