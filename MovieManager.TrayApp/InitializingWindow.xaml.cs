using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace MovieManager.TrayApp
{
    /// <summary>
    /// Interaction logic for InitializingWindow.xaml
    /// </summary>
    public partial class InitializingWindow : Window
    {
        public InitializingWindow()
        {
            InitializeComponent();
            this.Closing += InitializingWindow_Closing;
        }

        public void SetText(string text)
        {
            Text.Text = text;
        }

        private void InitializingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Prevent the window from being closed
            e.Cancel = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hWnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hWnd)?.AddHook(new HwndSourceHook(WindowProc));
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (msg == WM_SYSCOMMAND && (int)wParam == SC_CLOSE)
            {
                handled = true; // Prevent the window from being closed
            }

            return IntPtr.Zero;
        }
    }
}
