using CPU_Preference_Changer.Core;
using CPU_Preference_Changer.Core.SingleTonTemplate;
using CPU_Preference_Changer.WinAPI_Wrapper;
using System.Windows;

namespace CPU_Preference_Changer
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Check My Process having run
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //소스 코드 위치 이동으로 인해.. Main파일 위치를 상대경로로 지정해주었음.
            this.StartupUri = new System.Uri("UI/MainUI/MainWindow.xaml", System.UriKind.Relative);
            base.OnStartup(e);

            bool createNew;
            string myProcessName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            new System.Threading.Mutex(true, myProcessName, out createNew);
            if (createNew == false)
            {
                System.IntPtr wHandle = WinAPI.FindWindow(null, myProcessName);
                WinAPI.ShowWindow(wHandle, SwindOp.SW_SHOWNORMAL);
                WinAPI.SetForegroundWindow(wHandle);
                Shutdown();
            }

            /*프로그램 실행인자가 있다면 적절히 파싱한다.*/
            if( e.Args.Length != 0) {
                foreach ( string x in e.Args ) {
                    string upper = x.ToUpper();
                    if (upper.Equals("DEBUG_RUN")) {
                        MMHGlobalInstance<MMHGlobal>.GetInstance().bDebugModeRun = true;
                    }
                }
            }
        }
    }
}
