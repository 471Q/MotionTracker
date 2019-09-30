namespace MainScreenUI
{
    using KinectCoordinateMapping;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for Exercise.xaml
    /// </summary>
    public partial class Exercise : Page, INotifyPropertyChanged
    {
        KinectSensor                    _sensor = null;
        MultiSourceFrameReader          _reader = null;
        IList<Body>                     _bodies = null;
        List<GestureDetector>           gestureDetectorList = new List<GestureDetector>();
        VisualGestureBuilderDatabase    database = null;
        GestureResultView               result = new GestureResultView();
        GestureDetector                 detector = null;
        List<FileInfo>                  selectedFiles = new List<FileInfo>();

        public string   gestureText = "";
        private string  selectedGesture = "";
        private string  selectedDb = "";

        public Exercise()
        {
            InitializeComponent();
            DataContext = this;
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
            int maxBodies = _sensor.BodyFrameSource.BodyCount;
            for (int i = 0; i < maxBodies; ++i)
            {
                result = new GestureResultView(i, false, false, 0.0f);
                detector = new GestureDetector(_sensor, result);
                gestureDetectorList.Add(detector);

                // Split gesture results across the rows of the content grid
                ContentControl contentControl = new ContentControl
                {
                    Content = gestureDetectorList[i].GestureResultView
                };
                Grid.SetColumn(contentControl, 1);
                Grid.SetRow(contentControl, i);
                contentGrid.Children.Add(contentControl);
            }
        }

        /// <summary>
        /// Specify the directory which contain the gesture files
        /// </summary>
        private void Add_folder(object sender, RoutedEventArgs e)
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
                        using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(_file.FullName))
                        {
                            foreach (Gesture gesture in database.AvailableGestures)
                            {
                                //Console.WriteLine(String.Concat("Gesture Name", Newtonsoft.Json.JsonConvert.SerializeObject(gesture.Name, Formatting.Indented)));
                                UIListBox.Items.Add(gesture.Name);
                            }
                        }
                    }
                }
            }
            catch { }
            /*var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.MyComputer
            };
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
                                //Console.WriteLine(String.Concat("Gesture Name", Newtonsoft.Json.JsonConvert.SerializeObject(gesture.Name, Formatting.Indented)));
                                UIListBox.Items.Add(gesture.Name);
                            }
                        }
                    }
                }
            }
            catch { }*/
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// To invoke event that handle property change if called
        /// </summary>
        /// <param name="propertyName">Name of the property that need to be handled</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Get or Set the UIGesture Property
        /// </summary>
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
            bool dataReceived = false;
            using (BodyFrame bodyFrame = reference.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this._bodies == null)
                    {
                        // creates an array of 6 bodies, which is the max number of bodies that Kinect can track simultaneously
                        this._bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this._bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                // we may have lost/acquired bodies, so update the corresponding gesture detectors
                if (_bodies != null)
                {
                    // loop through all bodies to see if any of the gesture detectors need to be updated
                    int maxBodies = _sensor.BodyFrameSource.BodyCount;
                    for (int i = 0; i < maxBodies; ++i)
                    {
                        Body body = _bodies[i];
                        ulong trackingId = body.TrackingId;

                        // if the current body TrackingId changed, update the corresponding gesture detector with the new value
                        if (trackingId != gestureDetectorList[i].TrackingId)
                        {
                            //Console.WriteLine(String.Concat("gestureDetectionList ", Newtonsoft.Json.JsonConvert.SerializeObject(gestureDetectorList[0], Formatting.Indented)));
                            gestureDetectorList[i].TrackingId = trackingId;

                            // if the current body is tracked, unpause its detector to get VisualGestureBuilderFrameArrived events
                            // if the current body is not tracked, pause its detector so we don't waste resources trying to get invalid gesture results
                            gestureDetectorList[i].IsPaused = trackingId == 0;
                        }
                    }
                }
            }

            //Draw ellipses correctly
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
                            // COORDINATE MAPPING
                            foreach (Joint joint in body.Joints.Values)
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

        /// <summary>
        /// Handles the left mouse double-clicked gesture from the listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UIListBox_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            selectedGesture = ((ListBox)sender).SelectedItem.ToString();
            //Console.WriteLine(String.Concat("Clicked item is ", selectedGesture));
            foreach (FileInfo _file in selectedFiles)
                using (database = new VisualGestureBuilderDatabase(_file.FullName))
                    foreach (Gesture gesture in database.AvailableGestures)
                        if (gesture.Name.Equals(selectedGesture))
                            selectedDb = _file.FullName;
            if (selectedGesture != null && selectedDb != null)
            {
                int maxBodies = _sensor.BodyFrameSource.BodyCount;
                foreach(GestureDetector gs in gestureDetectorList)
                {
                    gs.VGBPath = selectedDb;
                    gs.GestureName = selectedGesture;
                }
            }
            //Console.WriteLine(String.Concat("DataContext ", Newtonsoft.Json.JsonConvert.SerializeObject(DataContext.ToString(), Formatting.Indented)));

            UIGesture = Convert.ToString(UIListBox.SelectedItem);
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
    }
}