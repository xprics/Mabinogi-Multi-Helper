using System;
using System.Threading;
using System.Windows;

namespace CPU_Preference_Changer
{
    public partial class MainWindow : Window
    {
        private object lockObj = new object();
        private const int refreshTerm = 5; // 5 second
        private Timer timer = null;

        private void initTimer()
        {
            this.CloseRefreshTimer();
            this.SetTimer();
        }

        private void SetTimer()
        {
            int interval = 1000;
            int refreshTick = 0;

            Func<int> localCallback = () => {
                if (refreshTick <= 0)
                {
                    this.RefresMabiProcess();
                    refreshTick = refreshTerm;
                }
                else
                {
                    refreshTick--;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        this.refreshTimeLabel.Content = refreshTick.ToString();
                    }));
                }

                return 0;
            };

            timer = new Timer((object state) => { (state as Func<int>)?.Invoke(); }, localCallback, 0, interval);
            this.RefresMabiProcess();
        }

        /// <summary>
        /// 프로세스 목록 다시 가져오기
        /// </summary>
        private void RefresMabiProcess()
        {
            // lock
            lock(this.lockObj)
            {
                // work thread invoke UI thread
                Dispatcher.Invoke(new Action(delegate
                {
                    MabiProcessListView.LvMabiDataCollection lvItm = new MabiProcessListView.LvMabiDataCollection();
                    object param = lvItm; /*함수인자에서 바로 object로 캐스팅하면 에러 발생한다.*/
                    MabiProcess.getAllTargets(CB_FindMabiProcess, ref param);
                    lvMabiProcess.setDataSoure(lvItm);
                }));
            }
        }

        /// <summary>
        /// 프로세스 목록 다시 가져오기 타이머 종료
        /// </summary>
        private void CloseRefreshTimer()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
            }
            this.timer = null;
        }
    }
}
