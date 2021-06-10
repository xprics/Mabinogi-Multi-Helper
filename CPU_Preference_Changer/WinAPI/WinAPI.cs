using System;
using System.Runtime.InteropServices;

namespace CPU_Preference_Changer.WinAPI_Wrapper {

    /*
        *  SW_HIDE	 0	 보이지 않도록 합니다.
        *  SW_SHOWNORMAL	 1	 Window를 보이도록 하되 최대화 또는 최소화 되어 있으면 원래상태로 되돌립니다.
        *  SW_SHOWMINIMIZED	 2	 Window를 활성화 하고 최소화 합니다.
        *  SW_MAXIMIZE	 3	 최대화 합니다.
        *  SW_SHOWNOACTIVATE	 4	 Window를 보이도록 하지만 활성화 하지는 않습니다.
        *  SW_SHOW	 5	 Window를 보이도록 합니다.
        *  SW_MINIMIZE	 6	 최소화 한 후 이전 Window를 활성화 합니다.
        *  SW_SHOWMINNOACTIVE	 7	 Window를 최소화하지만 활성화 하지는 않습니다.
        *  SW_SHOWNA	 8	 Window를 보이도록 하지만 활성화 하지는 않습니다.
        *  SW_RESTORE	 9	 원상태로 되돌립니다.
        *  SW_SHOWDEFAULT	 10	 -
        *  SW_FORCEMINIMIZE	 11	 최소화 합니다.
        */
    public enum SwindOp : uint {
        SW_HIDE = 0
           , SW_SHOWNORMAL = 1
           , SW_SHOWMINIMIZED = 2
           , SW_MAXIMIZE = 3
           , SW_SHOWNOACTIVATE = 4
           , SW_SHOW = 5
           , SW_MINIMIZE = 6
           , SW_SHOWMINNOACTIVE = 7
           , SW_SHOWNA = 8
           , SW_RESTORE = 9
           , SW_SHOWDEFAULT = 10
           , SW_FORCEMINIMIZE = 11
    };

    public class WinAPI {

        /// <summary>
        /// Win32 API Window process handler 가져오기.
        /// 첫 파라미터는 프로세스 이름(null으로 생략 가능), 두 번째 파라미터는 타이틀 이름(null으로 생략 가능).
        /// 파라미터 2개 중 1개는 반드시 입력해주셔야 됩니다.
        /// </summary>
        /// <param name="strProcessName"></param>
        /// <param name="strWindowTitleName"></param>
        /// <returns></returns>
        [DllImport("User32", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string strProcessName, string strWindowTitleName);

        /// <summary>
        /// Win32 API SetForegroundWindow 선언,, 이걸로해보고 잘안되면 ShowWindow를 써보던가 한다...
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Win32 API ShowWindow, 최소화된 프로그램을 강제로 깨우는 함수
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool ShowWindow(IntPtr hwnd, SwindOp nCmdShow);

    }
}
