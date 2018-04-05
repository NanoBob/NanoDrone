﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Diagnostics;
using NanoDrone.Controllers;
using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NanoDrone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Controllers.NanoDrone drone;
        public MainPage()
        {
            this.InitializeComponent();
            
            Application.Current.Suspending += (ss, ee) =>
            {
                Debug.WriteLine("Suspending");
                drone.Stop();
            };

            Window.Current.Closed += (ss, ee) =>
            {
                Debug.WriteLine("Closing");
                drone.Stop();
            };

            Windows.ApplicationModel.Core.CoreApplication.Exiting += (ss, ee) =>
            {
                Debug.WriteLine("Exiting");
                drone.Stop();
            };

            Window.Current.VisibilityChanged += (ss, ee) =>
            {
                if (!ee.Visible)
                {
                    Debug.WriteLine("No longer visible");
                    drone.Stop();
                }
            };

            Application.Current.EnteredBackground += (ss, ee) =>
            {
                Debug.WriteLine("Going to background");
                drone.Stop();
            };

            drone = new Controllers.NanoDrone();
        }
    }
}
