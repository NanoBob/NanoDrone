using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NanoDrone.Controllers;
using System.Diagnostics;
using Windows.Storage;
using System.Text;

namespace NanoDrone
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>

    sealed partial class App : Application
    {
        private Controllers.NanoDrone drone;

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                drone = new Controllers.NanoDrone();
            } catch (Exception exception)
            {
                LogException(exception);
            }
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            Debug.WriteLine("Suspending");
            var deferral = e.SuspendingOperation.GetDeferral();

            drone.Stop();

            deferral.Complete();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("Unhandled exception");
            LogException(e.Exception);
            drone.Stop();
        }

        private void LogException(Exception exception)
        {
            Windows.Storage.StorageFolder storageFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;
            IAsyncOperation<StorageFile> operation = storageFolder.CreateFileAsync("error.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            operation.AsTask().Wait();
            string toWrite = "Message :" + exception.Message + "\n" + Environment.NewLine + "StackTrace :" + exception.StackTrace +
                "" + Environment.NewLine + "Date :" + DateTime.Now.ToString();
            Windows.Storage.FileIO.WriteTextAsync(operation.AsTask().Result, toWrite);
        }
    }
}
