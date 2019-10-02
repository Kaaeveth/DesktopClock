using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using Microsoft.Win32;
using MaterialDesignThemes.Wpf;

namespace DesktopClock
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Properties.Settings _Settings = Properties.Settings.Default;
        private RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public MainWindow()
        {
            InitializeComponent();
            WindowStartup();
            AutoRun();

            DataContext = new Clock(DateDisplay);
        }

        private void ColorZone_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();

            _Settings.Y = Top;
            _Settings.X = Left;
            if (!_Settings.PosSet) _Settings.PosSet = true;
            _Settings.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            regKey.SetValue("DesktopClock", System.Windows.Forms.Application.ExecutablePath);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            regKey.DeleteValue("DesktopClock");
        }

        private void WindowStartup()
        {
            if (!_Settings.PosSet) WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Top = _Settings.Y;
                Left = _Settings.X;
            }
        }

        private void AutoRun()
        {
            if (regKey.GetValue("DesktopClock") == null)
                autoButton.IsChecked = false;
            else
                autoButton.IsChecked = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Topmost = true;
        }
    }

    public class Clock : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Timer _Timer = new Timer();
        public string Time { get; private set; } = DateTime.Now.ToShortTimeString();
        private MaterialDateDisplay MaterialDate { get; set; }

        public Clock(MaterialDateDisplay materialDate)
        {
            MaterialDate = materialDate;

            _Timer.Interval = 1000;
            _Timer.Enabled = true;
            _Timer.Tick += _Timer_Tick;

            MaterialDate.DisplayDate = DateTime.Now.Date;
        }

        private void _Timer_Tick(object sender, EventArgs e)
        {
            Time = DateTime.Now.ToShortTimeString();
            OnPropertyChanged("Time");
            MaterialDate.DisplayDate = DateTime.Now.Date;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
