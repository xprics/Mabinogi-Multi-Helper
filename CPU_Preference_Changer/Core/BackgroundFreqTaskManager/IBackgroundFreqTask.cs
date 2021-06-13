
namespace CPU_Preference_Changer.Core.BackgroundFreqTaskManager {

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
        void runFreqWork(object param);

    }
}
