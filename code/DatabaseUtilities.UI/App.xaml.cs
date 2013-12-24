using DatabaseUtilities.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace DatabaseUtilities.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
         public App() : base()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException; 
        }

         void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
         {
             Logger.Log(e.Exception);
             MessageBox.Show(e.Exception.ToString(), "Unexpected error (check log)", MessageBoxButton.OK, MessageBoxImage.Error);

             e.Handled = true;
         }
    }
}
