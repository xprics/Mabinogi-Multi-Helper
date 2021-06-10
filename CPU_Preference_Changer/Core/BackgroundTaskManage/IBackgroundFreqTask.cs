using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU_Preference_Changer.Core.BackgroundTaskManage {

    /// <summary>
    /// 2021.06.10 by LT인척하는엘프 
    /// 백그라운드에서 주기적으로 실행되는 작업에 대한 인터페이스
    /// 주기를 최대한 칼같이 챙기지만... 좀 오차가 생길 수 있음에 유의.
    /// </summary>
    interface IBackgroundFreqTask {

        /// <summary>
        /// 어느정도의 주기로 작동하는지 ( 밀리초 ) 얻기
        /// </summary>
        /// <returns></returns>
        int getFreqValue();

        /// <summary>
        /// 특정 작업 수행!
        /// </summary>
        /// <param name="taskParam"></param>
        void runFreqTask(object taskParam);
    }
}
