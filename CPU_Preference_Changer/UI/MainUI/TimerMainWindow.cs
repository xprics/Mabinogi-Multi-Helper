using System;
using System.Threading;
using System.Windows;

namespace CPU_Preference_Changer.UI.MainUI 
{
    /// <summary>
    /// by LT골든힐트
    /// 메인화면의 클라이언트 목록을 주기적으로 갱신시킨다.
    /// 현재 값은 5초마다 갱신, 타이머를 이용하여 작동한다!
    /// </summary>
    public partial class MainWindow : Window
    {
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

            //새로고침 먼저 해주고 타이머 등록.
            this.RefresMabiProcess();

            Func<int> localCallback = () => {
                if (refreshTick <= 0)
                {
                    ControlTextUpdateInvoke(refreshTimeLabel, "목록 갱신!");
                    this.RefresMabiProcess();
                    refreshTick = refreshTerm;
                }
                else
                {
                    //글자 먼저찍어서 0초 후 고침 방지.
                    ControlTextUpdateInvoke(refreshTimeLabel, refreshTick.ToString() + "초 후 새로고침");
                    refreshTick--;
                }
                return 0;
            };
            //타이머 실행까지 최초 글자가 없음... 일단 5초후 찍어주고 시작.
            ControlTextUpdateInvoke(refreshTimeLabel, refreshTick.ToString() + "초 후 새로고침");
            timer = new Timer((object state) => { (state as Func<int>)?.Invoke(); }, localCallback, 0, interval);
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
