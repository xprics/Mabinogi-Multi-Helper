using CPU_Preference_Changer.WinAPI_Wrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CPU_Preference_Changer.UI.ViewSome
{

    public class ProcessDispInfo
    {
        public BitmapImage processImg { get; set; }

        public string processName { get; set; }
    }

    /// <summary>
    /// ProcessSelectWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProcessSelectWindow : Window
    {

        private ObservableCollection<ProcessDispInfo> dispList = new ObservableCollection<ProcessDispInfo>();

        public ProcessSelectWindow()
        {
            InitializeComponent();
            Loaded += ProcessSelectWindow_Loaded;
        }

        private void ProcessSelectWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /**/
            try {
                Process[] list = Process.GetProcesses();
                int idx = 1;
                foreach(var cur in list) {
                    if( cur.MainWindowHandle!= IntPtr.Zero ) {
                        dispList.Add(new ProcessDispInfo(){
                            processName = cur.ProcessName
                           ,processImg = WinAPI.getProcessScreenImg(cur.MainWindowHandle, cur.ProcessName,cur.Id)
                           
                        }) ;
                    }
                    idx++;
                }
            } catch {
            } finally {
                if( dispList.Count==0) {
                    /*목록을 불러오는데 실패하였습ㄴ ㅣ다 언젠가 출력하기*/
                } else {
                    lbProcessList.ItemsSource = dispList;
                }
            }
        }
    }
}
