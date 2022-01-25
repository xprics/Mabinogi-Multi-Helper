using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPU_Preference_Changer.UI.ViewSome.TabSubUI
{
    /// <summary>
    /// 투명도 조절 가능한지 인터페이스
    /// </summary>
    interface ITransCanChange
    {
        /// <summary>
        /// 투명도 세팅
        /// </summary>
        /// <param name="alpha"></param>
        void setBackgroundTrans(byte alpha, byte r, byte g, byte b);
    }
}
