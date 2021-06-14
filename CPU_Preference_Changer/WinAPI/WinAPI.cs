using System;
using System.Runtime.InteropServices;
using System.Text;

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

    /*
    GW_HWNDFIRST 0   최상위 Window를 찾는다.
    GW_HWNDLAST  1   최하위 Window를 찾는다.
    GW_HWNDNEXT  2   하위 Window를 찾는다.
    GW_HWNDPREV  3   상위 Window를 찾는다.
    GW_OWNER     4   부모 Window를 찾는다.
    GW_CHILD     5   자식 Window를 찾는다.
    */
    public enum GetWindowCmd : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

    public class WinAPI {

        /// <summary>
        /// Win32 API Window process handler 가져오기.
        /// 첫 파라미터는 프로세스 이름(null으로 생략 가능), 두 번째 파라미터는 타이틀 이름(null으로 생략 가능).
        /// 두 파라미터 다 null을 입력할 경우 최상위 root window가 반환됩니다.
        /// </summary>
        /// <param name="strProcessName"></param>
        /// <param name="strWindowTitleName"></param>
        /// <returns></returns>
        [DllImport("User32", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string strProcessName, string strWindowTitleName);

        /// <summary>
        /// 해당 window의 부모님 안부를 묻습니다.
        /// 없으면(최상위 부모라면) IntPtr.Zero를 반환합니다.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        /// <summary>
        /// 지정한 Window와의 관계 Window찾습니다.
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindow
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="wCmd"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd wCmd);

        /// <summary>
        /// Win32 API SetForegroundWindow 선언,, 이걸로해보고 잘안되면 ShowWindow를 써보던가 한다...
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Win32 API ShowWindow
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nCmdShow"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool ShowWindow(IntPtr hwnd, SwindOp nCmdShow);

        /// <summary>
        /// Win32 Api GetTickCount64  =-시간반환 (부팅 후 지금까지 밀리초단위로)
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern ulong GetTickCount64();


        /// <summary>
        /// EnumWindows에 사용 될 콜백 함수 정의
        /// </summary>
        /// <param name="hwnd">윈도우 핸들</param>
        /// <param name="lParam">사용자 Param</param>
        /// <returns></returns>
        public delegate bool EnumWindowsProc(IntPtr hwnd, int lParam);

        /// <summary>
        /// 모든 윈도우를 순회하는 윈도우 API함수.
        /// </summary>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);

        /// <summary>
        /// 윈도우 핸들을 통해 PID값을 얻어오는 WIN32 API 함수
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpdwProcessId"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// GetWindowText Win32 API
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lpString"></param>
        /// <param name="nMaxCount"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}
