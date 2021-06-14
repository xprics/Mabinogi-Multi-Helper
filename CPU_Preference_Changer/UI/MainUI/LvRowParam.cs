using CPU_Preference_Changer.Core.BackgroundFreqTaskManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPU_Preference_Changer.UI.MainUI {
    /// <summary>
    /// 리스트뷰 Row데이터에 사용 할 사용자 정의 Param
    /// </summary>
    class LvRowParam {
        /// <summary>
        /// 해당 데이터의 PID
        /// </summary>
        public int PID { get; set; }

        /// <summary>
        /// 해당 데이터가 예약 종료작업 걸려있다면 그 작업에 대한 핸들.
        /// </summary>
        public HBFT hReservedKillTask { get; set; }
    }
}
