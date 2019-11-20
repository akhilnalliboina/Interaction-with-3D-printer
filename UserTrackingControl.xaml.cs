//------------------------------------------------------------------------------
// <copyright file="UserTrackingControl.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.AdaptiveUI
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Toolkit;
    using Microsoft.Kinect.Toolkit.Controls;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Web;
    using System.Net;
    using System.IO;


    /// <summary>
    /// Control that places itself where the user is.  Pays
    /// attention to the user's zone to change the experience.
    /// </summary>
    public partial class UserTrackingControl
    {


       
        public static readonly DependencyProperty UserDistanceProperty =
            DependencyProperty.Register("UserDistance", typeof(UserDistance), typeof(UserTrackingControl), new PropertyMetadata(UserDistance.Far, (o, args) => ((UserTrackingControl)o).OnUserDistanceChanged((UserDistance)args.OldValue, (UserDistance)args.NewValue)));

        public static readonly DependencyProperty KinectSensorProperty =
            DependencyProperty.Register("KinectSensor", typeof(KinectSensor), typeof(UserTrackingControl), new PropertyMetadata(null, (o, args) => ((UserTrackingControl)o).OnKinectSensorChanged((KinectSensor)args.NewValue)));

        public static readonly DependencyProperty SettingsProperty =
            DependencyProperty.Register("Settings", typeof(Settings), typeof(UserTrackingControl), new PropertyMetadata(null, (o, args) => ((UserTrackingControl)o).OnSettingsChanged((Settings)args.NewValue)));

        public static readonly DependencyProperty SensorToScreenCoordinatesTransformProperty =
            DependencyProperty.Register("SensorToScreenCoordinatesTransform", typeof(Matrix3D), typeof(UserTrackingControl), new PropertyMetadata(Matrix3D.Identity, (o, args) => ((UserTrackingControl)o).OnTransformChanged((Matrix3D)args.NewValue)));

        /// <summary>
        /// The width of this control sized using degrees in the user's field of view.
        /// </summary>
        private const double DegreesWide = 10.0;

        /// <summary>
        /// The height of this control sized using degrees in the user's field of view.
        /// </summary>
        private const double DegreesHigh = 10.0;

        private readonly AdaptiveUIPlacementHelper adaptiveUIPlacementHelper = new AdaptiveUIPlacementHelper();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTrackingControl"/> class. 
        /// </summary>
        public UserTrackingControl()
        {
            this.InitializeComponent();
            this.Visibility = Visibility.Hidden;

            this.NextButton1.Visibility = Visibility.Hidden;
            this.Replay1Button.Visibility = Visibility.Hidden;

            this.adaptiveUIPlacementHelper.DegreesHigh = DegreesHigh;
            this.adaptiveUIPlacementHelper.DegreesWide = DegreesWide;
            this.adaptiveUIPlacementHelper.Target = this;
        }

        

        /// <summary>
        /// Where the user is in relation to the display
        /// </summary>
        public UserDistance UserDistance
        {
            get
            {
                return (UserDistance)this.GetValue(UserDistanceProperty);
            }

            set
            {
                this.SetValue(UserDistanceProperty, value);
            }
        }

        /// <summary>
        /// The sensor we are using
        /// </summary>
        public KinectSensor KinectSensor
        {
            get
            {
                return (KinectSensor)this.GetValue(KinectSensorProperty);
            }

            set
            {
                this.SetValue(KinectSensorProperty, value);
            }
        }

        /// <summary>
        /// Settings object used for placement calculations
        /// </summary>
        public Settings Settings
        {
            get
            {
                return (Settings)this.GetValue(SettingsProperty);
            }

            set
            {
                this.SetValue(SettingsProperty, value);
            }
        }

        /// <summary>
        /// Transform from sensor skeleton space to screen coordinates
        /// </summary>
        public Matrix3D SensorToScreenCoordinatesTransform
        {
            get
            {
                return (Matrix3D)this.GetValue(SensorToScreenCoordinatesTransformProperty);
            }

            set
            {
                this.SetValue(SensorToScreenCoordinatesTransformProperty, value);
            }
        }

        private void OnUserDistanceChanged(UserDistance oldValue, UserDistance newValue)
        {
            this.adaptiveUIPlacementHelper.Parent = this.Parent as FrameworkElement;

            switch (newValue)
            {
                case UserDistance.Unknown:
                case UserDistance.Far:
                    this.Visibility = Visibility.Collapsed;
                    break;

                case UserDistance.Medium:
                    this.Visibility = Visibility.Visible;
                    this.StartingButton.Visibility = Visibility.Visible;

                    if (oldValue != UserDistance.Touch)
                    {
                        this.adaptiveUIPlacementHelper.UpdatePlacement(this.Settings.FarBoundary - this.Settings.BoundaryHysteresis);
                    }

                    break;

                case UserDistance.Touch:
                    this.Visibility = Visibility.Visible;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("newValue");
            }
        }

        private void OnKinectSensorChanged(KinectSensor newSensor)
        {
            this.adaptiveUIPlacementHelper.KinectSensor = newSensor;
        }

        private void OnSettingsChanged(Settings newValue)
        {
            this.adaptiveUIPlacementHelper.Settings = newValue;
        }

        private void OnTransformChanged(Matrix3D newValue)
        {
            this.adaptiveUIPlacementHelper.SensorToScreenCoordinatesTransform = newValue;
        }

        private void StartingButtonClicked(object sender, RoutedEventArgs e)
        {
            this.StartingButton.Visibility = Visibility.Hidden;
            mePlayer.Play();

            this.NextButton1.Visibility = Visibility.Visible;
            this.Replay1Button.Visibility = Visibility.Visible;


        }
        private void NextButton1Clicked(object sender, RoutedEventArgs e)
        {

            mePlayer.Close();
            this.StartingButton.Visibility = Visibility.Hidden;
            mePlayer1.Play();
            this.NextButton1.Visibility = Visibility.Hidden;
            this.NextButton2.Visibility = Visibility.Visible;
        }
        private void NextButton2Clicked(object sender, RoutedEventArgs e)
        {

            mePlayer1.Close();
            this.StartingButton.Visibility = Visibility.Hidden;

            mePlayer2.Play();
            this.NextButton2.Visibility = Visibility.Hidden;
            this.NextButton3.Visibility = Visibility.Visible;

            //Set the extruder to 210
            SetExtruder210();


        }
        private void NextButton3Clicked(object sender, RoutedEventArgs e)
        {

            mePlayer2.Close();
            this.StartingButton.Visibility = Visibility.Hidden;
            mePlayer3.Play();

            this.NextButton3.Visibility = Visibility.Hidden;
            this.NextButton4.Visibility = Visibility.Visible;
        }
        private void NextButton4Clicked(object sender, RoutedEventArgs e)
        {

            mePlayer3.Close();
            this.StartingButton.Visibility = Visibility.Hidden;
            mePlayer4.Play();

            this.NextButton4.Visibility = Visibility.Hidden;
            this.NextButton5.Visibility = Visibility.Visible;
        }
        private void NextButton5Clicked(object sender, RoutedEventArgs e)
        {

            mePlayer4.Close();
            this.StartingButton.Visibility = Visibility.Hidden;
            mePlayer5.Play();

            this.NextButton5.Visibility = Visibility.Hidden;
            this.NextButton6.Visibility = Visibility.Visible;
        }
        private void NextButton6Clicked(object sender, RoutedEventArgs e)
        {

            mePlayer5.Close();
            this.StartingButton.Visibility = Visibility.Hidden;
            mePlayer6.Play();

            this.NextButton6.Visibility = Visibility.Hidden;

            //Set the extruder to 0

            SetExtruder0();
        }
        
    public void SetExtruder210()
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.11.9/api/printer/tool");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        httpWebRequest.Headers.Add("X-Api-Key", "FEDE10B651E049B2AB3F6611121D18E9");
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string payload = "{\"command\":\"target\"," +
                          "\"targets\":{\"tool0\"::\"215\"}}";

            streamWriter.Write(payload);
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
        }
    }
        public void SetExtruder0()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.11.9/api/printer/tool");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add("X-Api-Key", "FEDE10B651E049B2AB3F6611121D18E9");
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string payload = "{\"command\":\"target\"," +
                              "\"targets\":{\"tool0\"::\"0\"}}";

                streamWriter.Write(payload);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
    }

