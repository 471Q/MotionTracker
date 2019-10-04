//------------------------------------------------------------------------------
// <copyright file="GestureResultView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace MainScreenUI
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Stores discrete gesture results for the GestureDetector.
    /// Properties are stored/updated for display in the UI.
    /// </summary>
    public class GestureResultView : INotifyPropertyChanged
    {
        /// <summary> The body index (0-5) associated with the current gesture detector </summary>
        private int bodyIndex = 0;

        /// <summary> Current confidence value reported by the discrete gesture </summary>
        private float confidence = 0.0f;

        /// <summary> True, if the discrete gesture is currently being detected </summary>
        private bool detected = false;

        /// <summary> True, if the body is currently being tracked </summary>
        private bool isTracked = false;

        /// <summary>
        /// Initializes a new instance of the GestureResultView class and sets initial property values
        /// </summary>
        /// <param name="bodyIndex">Body Index associated with the current gesture detector</param>
        /// <param name="isTracked">True, if the body is currently tracked</param>
        /// <param name="detected">True, if the gesture is currently detected for the associated body</param>
        /// <param name="confidence">Confidence value for detection of the 'Seated' gesture</param>
        public GestureResultView(int bodyIndex, bool isTracked, bool detected, float confidence)
        {
            BodyIndex = bodyIndex;
            IsTracked = isTracked;
            Detected = detected;
            UIConfidence = confidence;
        }

        public GestureResultView()
        {
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> 
        /// Gets the body index associated with the current gesture detector result 
        /// </summary>
        public int BodyIndex
        {
            get
            {
                return bodyIndex;
            }

            private set
            {
                if (bodyIndex != value)
                {
                    bodyIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the body associated with the gesture detector is currently being tracked 
        /// </summary>
        public bool IsTracked
        {
            get
            {
                return isTracked;
            }

            private set
            {
                if (isTracked != value)
                {
                    isTracked = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a value indicating whether or not the discrete gesture has been detected
        /// </summary>
        public bool Detected
        {
            get
            {
                return detected;
            }

            private set
            {
                if (detected != value)
                {
                    detected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary> 
        /// Gets a float value which indicates the detector's confidence that the gesture is occurring for the associated body 
        /// </summary>
        public float UIConfidence
        {
            get
            {
                return confidence;
            }

            private set
            {
                if (confidence != value)
                {
                    confidence = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UIDetected
        {
            get
            {
                return detected ? "Match" : "Not Match";
            }

            private set
            {
                if (value.Equals("Match"))
                {
                    detected = true;
                    NotifyPropertyChanged();
                }
                else if (value.Equals("Not Match"))
                {
                    detected = false;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Updates the values associated with the discrete gesture detection result
        /// </summary>
        /// <param name="isBodyTrackingIdValid">True, if the body associated with the GestureResultView object is still being tracked</param>
        /// <param name="isGestureDetected">True, if the discrete gesture is currently detected for the associated body</param>
        /// <param name="detectionConfidence">Confidence value for detection of the discrete gesture</param>
        public void UpdateGestureResult(bool isBodyTrackingIdValid, bool isGestureDetected, float detectionConfidence)
        {
            IsTracked = isBodyTrackingIdValid;
            UIConfidence = 0.0f;

            if (!IsTracked)
            {
                Detected = false;
                UIDetected = "Not Match";
            }
            else
            {
                Detected = isGestureDetected;
                if (Detected)
                {
                    UIConfidence = detectionConfidence;
                    UIDetected = "Match";
                }
                else if (!Detected)
                {
                    UIDetected = "Not Match";
                }
            }
        }

        /// <summary>
        /// Notifies UI that a property has changed
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param> 
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (UIConfidence > 0.35f)
            {
                Console.WriteLine(String.Concat("Body Index ", BodyIndex.ToString()));
                Console.WriteLine(String.Concat("UI Detected ", UIDetected.ToString()));
                Console.WriteLine(String.Concat("UI Confidence ", UIConfidence.ToString()));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
