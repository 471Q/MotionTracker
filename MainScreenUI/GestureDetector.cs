﻿//------------------------------------------------------------------------------
// <copyright file="GestureDetector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace MainScreenUI
{
    using FireSharp.Config;
    using FireSharp.Interfaces;
    using FireSharp.Response;
    using Microsoft.Kinect;
    using Microsoft.Kinect.VisualGestureBuilder;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Gesture Detector class which listens for VisualGestureBuilderFrame events from the service
    /// and updates the associated GestureResultView object with the latest results for the 'Seated' gesture
    /// </summary>
    public class GestureDetector : IDisposable
    {
        Thread thread1 = null, thread2 = null;
        CancellationTokenSource ts1 = new CancellationTokenSource(), ts2 = new CancellationTokenSource();
        float temp = 0f;
        int i = 0, exerciseDone = 0;
        IFirebaseClient client;
        DiscreteGestureResult result = null;
        List<DiscreteGestureResult> results = new List<DiscreteGestureResult>();
        /// <summary> Name of the discrete gesture in the database that we want to track </summary>
        private string gestureName = "";

        private string vGBPath = "";

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;

        /// <summary>
        /// Initializes a new instance of the GestureDetector class along with the gesture frame source and reader
        /// </summary>
        /// <param name="kinectSensor">Active sensor to initialize the VisualGestureBuilderFrameSource object with</param>
        /// <param name="gestureResultView">GestureResultView object to store gesture results of a single body to</param>
        public GestureDetector(KinectSensor kinectSensor, GestureResultView gestureResultView)
        {
            
            //this.GestureName = gesture_name;
            if (kinectSensor == null)
                throw new ArgumentNullException("Kinect Sensor Not Found");
            GestureResultView = gestureResultView ?? throw new ArgumentNullException("Unable to produce detected body result, gestureResultView");

            // create the vgb source. The associated body tracking ID will be set when a valid body frame arrives from the sensor.
            vgbFrameSource = new VisualGestureBuilderFrameSource(kinectSensor, 0);
            vgbFrameSource.TrackingIdLost += this.Source_TrackingIdLost;

            // open the reader for the vgb frames
            vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (vgbFrameReader != null)
            {
                vgbFrameReader.IsPaused = true;
                vgbFrameReader.FrameArrived += this.Reader_GestureFrameArrived;
            }

            //Create a indefinitely looped independent task that wait for every 0.5 seconds; 
            //Calculate the average value of confidence value in list of results;
            //Update the GestureResultView, and in turn update the XAML UI
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    thread1 = Thread.CurrentThread;
                    temp = 0.0f;
                    results.Clear();
                    System.Threading.Thread.Sleep(500);
                    Calculation();
                    System.Threading.Thread.Sleep(50);
                    if ((temp / i) > 0.35f)
                    {
                        exerciseDone++;
                        GestureResultView.UpdateGestureResult(true, true, (temp / i));
                        //Console.WriteLine(String.Concat("Confidence ", (temp / i)));
                    }
                    else
                        GestureResultView.UpdateGestureResult(true, false, (temp / i));
                }
            });

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    thread2 = Thread.CurrentThread;
                    IFirebaseConfig ifc = new FirebaseConfig()
                    {
                        AuthSecret = "5JF2869ie6NEZOnxh2YPqEVnvoa9UdttEdaSeKAG",
                        BasePath = "https://motiontracker-dd816.firebaseio.com/"
                    };
                    try
                    {
                        client = new FireSharp.FirebaseClient(ifc);
                    }
                    catch
                    {
                        Console.WriteLine("No Internet or Connection Problem");
                    }
                    System.Threading.Thread.Sleep(2000);
                    Login.userDetail.Points++;
                    SetResponse set = client.Set(@"Users/" + Login.userDetail.Username, Login.userDetail);
                }
            });

            //Task.Factory.StartNew((RunCalculationEvery550ms));
            //Task.Factory.StartNew((UpdatePointsToDB));
        }

        private void Calculation()
        {
            for (i = 0; i < results.Count; i++)
            {
                temp += results[i].Confidence;
            }
        }

        /// <summary> Gets the GestureResultView object which stores the detector results for display in the UI </summary>
        public GestureResultView GestureResultView { get; private set; }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return vgbFrameSource.TrackingId;
            }

            set
            {
                if (vgbFrameSource.TrackingId != value)
                    vgbFrameSource.TrackingId = value;
            }
        }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public string GestureName
        {
            get
            {
                return gestureName;
            }

            set
            {
                if (gestureName != value)
                    gestureName = value;
                using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(vGBPath))
                {
                    foreach (Gesture gesture in database.AvailableGestures)
                    {
                        //Console.WriteLine(String.Concat("Gesture Name", Newtonsoft.Json.JsonConvert.SerializeObject(gesture.Name, Formatting.Indented)));
                        if (gesture.Name.Equals(gestureName))
                            vgbFrameSource.AddGesture(gesture);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public string VGBPath
        {
            get
            {
                return vGBPath;
            }

            set
            {
                if (vGBPath != value)
                    vGBPath = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return vgbFrameReader.IsPaused;
            }

            set
            {
                if (vgbFrameReader.IsPaused != value)
                    vgbFrameReader.IsPaused = value;
            }
        }

        public int ExerciseDone
        {
            get
            {
                return exerciseDone;
            }

            set
            {
                if (exerciseDone != value)
                    exerciseDone = value;
            }
        }

        /// <summary>
        /// Disposes all unmanaged resources for the class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the VisualGestureBuilderFrameSource and VisualGestureBuilderFrameReader objects
        /// </summary>
        /// <param name="disposing">True if Dispose was called directly, false if the GC handles the disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (vgbFrameReader != null)
                {
                    vgbFrameReader.FrameArrived -= this.Reader_GestureFrameArrived;
                    vgbFrameReader.Dispose();
                    vgbFrameReader = null;
                }

                if (vgbFrameSource != null)
                {
                    vgbFrameSource.TrackingIdLost -= this.Source_TrackingIdLost;
                    vgbFrameSource.Dispose();
                    vgbFrameSource = null;
                }
            }
            Console.WriteLine("GestureDectector Class Disposing.....");
            thread1.Abort();
            thread2.Abort();
        }

        /// <summary>
        /// Handles gesture detection results arriving from the sensor for the associated body tracking Id
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_GestureFrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;
            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
                if (frame != null)
                {
                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                    //IReadOnlyDictionary<Gesture, ContinuousGestureResult> continuousResults = frame.ContinuousGestureResults;

                    //if (continuousResults != null)
                    if (discreteResults != null)
                        // we only have one gesture in this source object, but you can get multiple gestures
                        foreach (Gesture gesture in vgbFrameSource.Gestures)
                            if (gesture.Name.Equals(GestureName))
                            { //&& gesture.GestureType == GestureType.Continuous
                              //ContinuousGestureResult result = null;
                                result = null;
                                //continuousResults.TryGetValue(gesture, out result);
                                discreteResults.TryGetValue(gesture, out result);
                                //For debugging
                                //Console.WriteLine(String.Concat("Frame.discreteResult ", Newtonsoft.Json.JsonConvert.SerializeObject(frame.DiscreteGestureResults, Formatting.Indented)));
                                //Console.WriteLine(String.Concat("continuosResult ", Newtonsoft.Json.JsonConvert.SerializeObject(continuousResults, Formatting.Indented)));
                                //Console.WriteLine(String.Concat("discreteResult ", Newtonsoft.Json.JsonConvert.SerializeObject(discreteResults, Formatting.Indented)));
                                //Console.WriteLine(String.Concat("Gesture ", Newtonsoft.Json.JsonConvert.SerializeObject(gesture, Formatting.Indented)));
                                //Console.WriteLine(String.Concat("Result ", Newtonsoft.Json.JsonConvert.SerializeObject(result, Formatting.Indented)));
                                //Console.WriteLine(String.Concat("Detection ", result.Detected));
                                //Console.WriteLine(String.Concat("Confidence ", result.Confidence));
                                results.Add(result);
                            }
                }
        }


        /// <summary>
        /// Handles the TrackingIdLost event for the VisualGestureBuilderSource object
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Source_TrackingIdLost(object sender, TrackingIdLostEventArgs e)
        {
            // update the GestureResultView object to show the 'Not Tracked' image in the UI
            //this.GestureResultView.UpdateGestureResult(false, false, 0.0f);
            Console.WriteLine("Tracking ID is lost!");
        }
    }
}

