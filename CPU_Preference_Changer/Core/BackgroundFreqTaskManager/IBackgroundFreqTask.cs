using System;

namespace CPU_Preference_Changer.Core.BackgroundFreqTaskManager {

    /// <summary>
    /// 에러 이벤트 함수 타입 정의
    /// </summary>
    /// <param name="err"></param>
    public delegate void ErrWriteEvent(Exception err);

    /// <summary>
    /// by LT인척하는엘프 2021.06.10
    /// 백그라운드에서 주기적으로 동작 할 작업 인터페이스
    /// </summary>
    interface IBackgroundFreqTask {

        /// <summary>
        /// 얼마마다 반복해야하는지 주기 얻기 (밀리초)
        /// </summary>
        /// <returns>밀리초 단위로 된 주기 반환..</returns>
        ulong getFreqTick();

        /// <summary>
        /// 특정 주기가 되었을 때 실행 할 작업
        /// </summary>
        /// <param name="taskHandle">TASK 핸들</param>
        /// <param name="param">유저 파라메터</param>
        /// <returns>
        /// true : 계속 실행
        /// false : 이제 그만 실행해도 됨 (반복 작업에서 제거)
        /// </returns>
        bool runFreqWork(HBFT taskHandle, object param);

        /// <summary>
        /// 에러 이벤트 핸들러..
        /// </summary>
        event ErrWriteEvent errWriteEventHandler;
    }
}
