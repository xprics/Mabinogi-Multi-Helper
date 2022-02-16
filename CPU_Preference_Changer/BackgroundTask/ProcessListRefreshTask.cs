using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using CPU_Preference_Changer.UI.MainUI;

namespace CPU_Preference_Changer.BackgroundTask {
    /// <summary>
    /// by LT인척하는엘프
    /// 프로세스 갱신 작업 클래스...
    /// 특정 주기마다 클라이언트 프로세스 목록을 갱신한다!
    /// </summary>
    class ProcessListRefreshTask : IBackgroundFreqTask {
        /// <summary>
        /// 몇 초마다 runFreqWork함수를 반복 실행할건지?? (밀리 초 단위)
        /// </summary>
        private const ulong freqTick = 1000;

        /// <summary>
        /// 몇 회마다 실제 목록을 갱신할지??
        /// freqTick * runTerm 만큼의 시간이 지난 후 갱신 된다..
        /// </summary>
        private const int runTerm=5;
        private int runCount = 0;

        /// <summary>
        /// 갱신 대상 화면...
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// 작동 중 에러 발생했을 때 이벤트 핸들러...
        /// </summary>
        public event ErrWriteEvent errWriteEventHandler = (System.Exception err) =>
        {
            throw err;
        };

        public ProcessListRefreshTask(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        /// <summary>
        /// 함수 실행 주기 반환
        /// </summary>
        /// <returns></returns>
        public ulong getFreqTick()
        {
            return freqTick;
        }

        /// <summary>
        /// task작업을 최대한 빨리 실행시키게 한다.
        /// </summary>
        public void taskRunAsPossibleAsFast()
        {
            runCount = 0;
        }

        /// <summary>
        /// 주기적으로 실행 되는 함수... 5번 실행 될 때 마다 목록을 갱신한다
        /// (주기 : 함수 실행 주기 1초, 5회 => 5초마다 갱신)
        /// </summary>
        /// <param name="param"></param>
        public bool runFreqWork(HBFT hTask, object param)
        {
            try
            {
                if (runCount <= 0)
                {
                    //실제로 5회 실행된 후 목록 갱신
                    mainWindow.updateRefreshTimeLabelText("목록 갱신!");
                    mainWindow.RefresMabiProcess();
                    runCount = runTerm;
                }
                else
                {
                    //글자 먼저찍어서 0초 후 고침 방지.
                    //몇초 뒤 갱신작업하는지 화면에 뿌려준다.
                    mainWindow.updateRefreshTimeLabelText(runCount.ToString() + "초 후 새로고침");
                    runCount--;
                }
            }
            catch (System.Exception err)
            {
                errWriteEventHandler(err);
            }

            return true;
        }

    }
}
