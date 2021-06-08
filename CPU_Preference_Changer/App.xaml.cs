using System.Windows;

namespace CPU_Preference_Changer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        System.Threading.Mutex mutex;

        [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        private static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(System.IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern bool ShowWindow(System.IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// Check My Process having run
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool createNew;
            string myProcessName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            mutex = new System.Threading.Mutex(true, myProcessName, out createNew);
            if (createNew == false)
            {
                System.IntPtr wHandle = FindWindow(null, myProcessName);
                ShowWindow(wHandle, 1);
                SetForegroundWindow(wHandle);
                Shutdown();
            }
        }
    }
}
