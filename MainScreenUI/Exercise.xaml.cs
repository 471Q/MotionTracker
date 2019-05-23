namespace MainScreenUI
{
    using KinectCoordinateMapping;
    using Microsoft.Kinect;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.ComponentModel;
    using System.IO;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Interaction logic for Exercise.xaml
    /// </summary>
    public partial class Exercise : Page, INotifyPropertyChanged
    {
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        List<GestureDetector> gestureDetectorList;

        CameraMode _mode = CameraMode.Color;

        GestureResultView result = null;

        public string statusText;
        public string gestureText;

        List<FileInfo> selectedFiles = new List<FileInfo>();

        public Exercise()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _sensor = KinectSensor.GetDefault();

            _sensor.Open();

            if (_sensor != null)
            {
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
            gestureDetectorList = new List<GestureDetector>();
            int maxBodies = _sensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                result = new GestureResultView(0, false, false, 0.0f);
                GestureDetector detector = new GestureDetector(_sensor, result);
                gestureDetectorList.Add(detector);
                DataContext = new {
                    GestureResultView = result,
                    Exercise = this,
                };
            }
        }

        /// <summary>
        /// Specify the directory which contain the gesture files
        /// </summary>
        private void Add_folder(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            try
            {
                DirectoryInfo _directory = new DirectoryInfo(dialog.SelectedPath.ToString());
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    FileInfo[] _Files = _directory.GetFiles("*.gbd");
                    foreach (FileInfo _file in _Files)
                    {
                        selectedFiles.Add(_file);
                        using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(_file.FullName))
                        {
                            foreach (Gesture gesture in database.AvailableGestures)
                            {
                                Console.WriteLine(String.Concat("Gesture Name", Newtonsoft.Json.JsonConvert.SerializeObject(gesture.Name, Formatting.Indented)));
                                UIListBox.Items.Add(gesture.Name);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string UIGesture
        {
            get
            {
                return gestureText;
            }

            set
            {
                if (gestureText != value)
                {
                    gestureText = value;

                    // notify any bound elements that the text has changed
                    OnPropertyChanged("UIGesture");
                }
            }
        }

        /// <summary>
        /// Handles the body frame data arriving from the sensor and updates the associated gesture detector object for each body
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (_bodies == null)
                    {
                        // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        _bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(_bodies);
                }
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (_mode == CameraMode.Color)
                    {
                        UICameraOutput.Source = frame.ToBitmap();
                    }
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    UICanvasOutput.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body.IsTracked)
                        {
                            //
                            int maxBodies = _sensor.BodyFrameSource.BodyCount;
                            for (int i = 0; i < maxBodies; ++i)
                            {
                                ulong trackingId = body.TrackingId;

                                // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                                if (trackingId != gestureDetectorList[i].TrackingId)
                                {
                                    gestureDetectorList[i].TrackingId = trackingId;

                                    // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                                    // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                                    gestureDetectorList[i].IsPaused = trackingId == 0;
                                }
                            }
                            // COORDINATE MAPPING
                            foreach (Joint joint in body.Joints.Values)
                            {
                                if (joint.TrackingState == TrackingState.Tracked)
                                {
                                    // 3D space point
                                    CameraSpacePoint jointPosition = joint.Position;

                                    // 2D space point
                                    Point point = new Point();

                                    if (_mode == CameraMode.Color)
                                    {
                                        ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);
                                        point.X = (float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X);
                                        point.Y = (float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y);
                                    }
                                    else if (_mode == CameraMode.Depth || _mode == CameraMode.Infrared) // Change the Image and Canvas dimensions to 512x424
                                    {
                                        DepthSpacePoint depthPoint = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(jointPosition);

                                        point.X = float.IsInfinity(depthPoint.X) ? 0 : depthPoint.X;
                                        point.Y = float.IsInfinity(depthPoint.Y) ? 0 : depthPoint.Y;
                                    }

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
                        else
                        {
                            result.UpdateGestureResult(false, false, 0.0f);
                        }
                    }
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_reader != null)
            {
                // BodyFrameReader is IDisposable
                _reader.MultiSourceFrameArrived -= Reader_MultiSourceFrameArrived;
                _reader.Dispose();
                _reader = null;
            }
            if (gestureDetectorList != null)
            {
                foreach (GestureDetector detector in this.gestureDetectorList)
                {
                    detector.Dispose();
                }

                gestureDetectorList.Clear();
                gestureDetectorList = null;
            }
            if (_sensor != null)
            {
                _sensor.Close();
                _sensor = null;
            }
        }

        private void UIListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Console.WriteLine("Clicked");
            Console.WriteLine(String.Concat("Item Name: ", Convert.ToString(UIListBox.SelectedItem)));
            if(UIListBox.SelectedItem != null)
                Console.WriteLine(String.Concat("Selected Object ", Newtonsoft.Json.JsonConvert.SerializeObject(UIListBox.SelectedItem, Formatting.Indented)));
            foreach(FileInfo _file in selectedFiles)
                using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(_file.FullName))
                {
                    foreach (Gesture gesture in database.AvailableGestures)
                    {
                        if (gesture.Name.Equals(Convert.ToString(UIListBox.SelectedItem)))
                        {
                            Console.WriteLine(String.Concat("Gesture ", Convert.ToString(UIListBox.SelectedItem), " is found in ", _file.FullName));
                            UIGesture = Convert.ToString(UIListBox.SelectedItem);
                        }
                    }
                }
        }
    }
}

enum CameraMode
{
    Color,
    Depth,
    Infrared
}