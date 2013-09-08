using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.ServiceModel;
using DatabaseUtilities.Web.Client;

namespace DatabaseUtilities.Web.Client
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;


        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var server = new ServerService.ServerServiceClient();

            
            server.GetCachedSnapshotCompleted += server_GetCachedSnapshotCompleted;
            server.GetCachedSnapshotAsync();

        }

        void server_GetCachedSnapshotCompleted(object sender, ServerService.GetCachedSnapshotCompletedEventArgs e)
        {
        }


    }
}
