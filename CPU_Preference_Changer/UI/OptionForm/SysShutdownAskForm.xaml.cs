using CPU_Preference_Changer.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CPU_Preference_Changer.UI.OptionForm {
    /// <summary>
    /// 1분간 대기하다가 아무 반응없으면 시스템을 종료시키는 화면.
    /// </summary>
    public partial class SysShutdownAskForm : Window {
        
        private bool bCancel = false;
        private Thread timeTh;

        public SysShutdownAskForm()
        {
            InitializeComponent();
            
            timeTh = new Thread(waitUserSelect);
            timeTh.Start();
        }

        private void waitUserSelect(object param)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            while (bCancel == false) {
                Thread.Sleep(1);
                if (s.ElapsedMilliseconds >= (60 * 1000)) {
                    shutdownWindow();
                }
            }
        }

        private void shutdownWindow()
        {
            SystemProcess.ShutdownNow();
        }

        private void bt_No_Click(object sender, RoutedEventArgs e)
        {
            bCancel = true;
            this.Close();
        }

        private void bt_Yes_Click(object sender, RoutedEventArgs e)
        {
            shutdownWindow();
        }
    }
}
