using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace DatabaseUtilities.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Core.ViewModel VM = null;

        public MainWindow()
        {
            InitializeComponent();

            VM = (Core.ViewModel)DataContext;

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            VM.Initialize();
        }



        private void SP_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SP_List_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SP_List_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Table_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Table_List_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Table_List_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            VM.GenerateSP_Select();
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {

            VM.GenerateSP_UpdateInsert();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            VM.GenerateSP_Delete();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Databases_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            VM.GetCSharpCodeForStoredProcedure(true);
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            var window = new About();
            window.ShowDialog();
            window.Close();
        }

        private void Menu_License_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://raw.github.com/AlanCS/Database-Utilities/master/license.txt");
        }

        private void btnOpenSQL_Click(object sender, RoutedEventArgs e)
        {
            VM.OpenSql();
        }
    }


    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                        System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
