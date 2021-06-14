using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CPU_Preference_Changer.Core.SingleTonTemplate;
using CPU_Preference_Changer.WinAPI_Wrapper;

namespace CPU_Preference_Changer.Core.BackgroundFreqTaskManager {

    /// <summary>
    /// Handle Background Freq Task 을 줄여서 HBFT라했다.
    /// </summary>
    class HBFT {
        private int _taskID = -1;
        public int taskID {
            get {
                return _taskID;
            }
            set {
                //최초의 SETTER만 허용 (외부에서 값 덮어쓰기 불가)
                if (_taskID == -1) _taskID = value;
            }
        }
        public HBFT(int taskID) { this.taskID = taskID; }
    }

    /// <summary>
    /// by LT인척하는엘프 2021.06.11
    /// 백그라운드에서 주기적으로 작업을 실행할 관리자 클래스
    /// 10미리보다 짧은 주기로는 작동 불가능 함.
    /// </summary>
    class BackgroundFreqTaskMgmt {
        /// <summary>
        /// 작업 정보 클래스
        /// </summary>
        private class TaskInfo {
            /// <summary>
            /// 주기적으로 작동해야할 작업
            /// </summary>
            public IBackgroundFreqTask task { get; set; }

            /// <summary>
            /// 혹시 유저가 Parameter를 지정하여 넣었었다면 그것을 그대로 넣어주기위해 보관해둠.
            /// </summary>
            public object taskParam;

            /// <summary>
            /// 마지막으로 실행한 시간
            /// </summary>
            public ulong lastRunTime { get; set; }

            /// <summary>
            /// 작업 실행중인지 아닌지 기록해둔다...
            /// </summary>
            public bool runState { get; set; }

            /// <summary>
            /// Task핸들 보관용도...
            /// </summary>
            public HBFT taskHandle { get; set; }

            public TaskInfo(IBackgroundFreqTask task,object param) { 
                this.task = task; this.taskParam = param; this.lastRunTime = 0; 
            }
        }

        private int taskID;
        private Dictionary<int, TaskInfo> taskDict;
        private Mutex dickLock;
        private bool bRun = false;
        private bool bManagerStop;

        public BackgroundFreqTaskMgmt()
        {
            taskDict = new Dictionary<int, TaskInfo>();
            taskID = 0; // TASK별로 붙일 아이디 값... 그냥 귀찮으니 0~순차적으로 증가하는 value를 키값으로 쓴다.
            dickLock = new Mutex();
        }

        /// <summary>
        /// 주어진 작업을 주기적으로 실행 할 작업에 등록한다.
        /// </summary>
        /// <param name="task">작업</param>
        /// <param name="param">작업 Parameter</param>
        /// <returns>작업 핸들</returns>
        public HBFT addFreqTask(IBackgroundFreqTask task, object param)
        {
            if (task == null) return null;
            dickLock.WaitOne();

            // oldTask가 이미 존재하는 이런 일은 일어날 수 없지만
            // 나중에 taskID쪽 코드를 수정하게 되면 발생 가능하니 일단 예외처리는 해둔다.
            if(taskDict.ContainsKey(taskID)) {
                dickLock.ReleaseMutex();
                return null;
            }
            /*-------------------------------------*/
            var taskInfo = new TaskInfo(task, param);
            taskInfo.taskHandle = new HBFT(taskID);
            taskDict.Add(taskID, taskInfo);
            /*-------------------------------------*/
            taskID++;
            dickLock.ReleaseMutex();
            return taskInfo.taskHandle;
        }

        /// <summary>
        /// 주어진 작업을 주기적으로 실행 할 작업에 등록한다.
        /// </summary>
        /// <param name="task">작업</param>
        /// <returns>작업 핸들</returns>
        public HBFT addFreqTask(IBackgroundFreqTask task)
        {
            return addFreqTask(task, null);
        }

        public void removeFreqTask(HBFT hBFT)
        {
            if (hBFT == null) return;
            dickLock.WaitOne();
            taskDict.Remove(hBFT.taskID);
            dickLock.ReleaseMutex();
        }

        public void startTaskManager()
        {
            if (bRun) return;
            Thread th = new Thread(taskManagerWorkThread);
            th.Start();
            bRun = true;
        }

        public void stopFreqTaskManager()
        {
            bRun = false;
        }

        private void taskManagerWorkThreadSubProc()
        {
            while (bRun) {
                Thread.Sleep(10);
                if (taskDict.Count == 0) continue;

                dickLock.WaitOne();
                foreach (var cur in taskDict) {
                    var curInfo = cur.Value;

                    var curTask = curInfo.task;
                    /*-------------------------------------------------------*/
                    /*여전히 저번에 실행 했던것이 실행중이라면 아무것도 안한다*/
                    if (curInfo.runState) continue;
                    curInfo.runState = true;
                    /*-------------------------------------------------------*/
                    /*현재 시간과 비교하여 실행 주기를 초과하였다면 실행시킨다.*/
                    ulong curTime = WinAPI.GetTickCount64();
                    if (curTime >= (curInfo.lastRunTime + curTask.getFreqTick())) {
                        curInfo.lastRunTime = curTime;
                        /*이 작업이 오래걸리면 다른 작업들도 지연되기때문에 Thread로 실행한다...*/
                        var th = new Thread(() =>
                        {
                            if(false==curTask.runFreqWork(curInfo.taskHandle, curInfo.taskParam)) {
                                removeFreqTask(curInfo.taskHandle);
                            }
                            curInfo.runState = false;
                        });
                        th.SetApartmentState(ApartmentState.STA);
                        th.Start();
                    } else {
                        curInfo.runState = false;
                    }
                }
                dickLock.ReleaseMutex();
            }
        }

        private void taskManagerWorkThread()
        {
            bManagerStop = true;
            /*실행 해야하는 상태라면 계-속*/
            while (bRun) { 
                try {
                    taskManagerWorkThreadSubProc();
                    /*정상적이라면 이 함수가 종료되고 일로 빠지는 것은
                     bRun==false가 되어 종료할 때 뿐이지만..
                    모종의 이유로 죽어버리면(하위 인터페이스에서 죽여버릴 경우) 매니저가 아무것도 안하게된다.
                    따라서 죽더라도 catch로 빠지게하고, 아직도 실행해야하는 상태라면
                    정상 작동 가능하도록... 무한히 실행하게 함!*/
                } catch (Exception err){
                    /*딱히 예외처리할것이 없음.. 아무것도 안한다.
                     특별히 있다면 죽은 task를 찾아서 
                    그 task는 건너뛰게 할 수 있게 여기서 뭔가 처리하기?*/

                    /*프로그래머에게만 알려주도록하자...*/
                    Debug.WriteLine("######################################################");
                    Debug.WriteLine("         !!!!! ERR !!!!!         ");
                    Debug.WriteLine(err.Message);
                    Debug.WriteLine(err.StackTrace.ToString());
                    Debug.WriteLine("######################################################");
                }
            }
            bManagerStop = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Release()
        {
            stopFreqTaskManager();
            while (bManagerStop) Thread.Sleep(100);
            taskDict.Clear();
            dickLock.Close();
        }
    }
}
