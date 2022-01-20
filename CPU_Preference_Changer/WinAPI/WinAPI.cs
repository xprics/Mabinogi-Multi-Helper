using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

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


        [DllImport("user32")]
        public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parent);

        enum ShowWindowFlag : uint
        {
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            SHOWWINDOW = 0x0040
        }

        /// <summary>
        /// SetWIndowPos API
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hWndInsertAfter"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, ShowWindowFlag flags);

        /// <summary>
        /// 주어진 윈도우를 TopMost으로 Set하거나 UnSet..
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="bTop"></param>
        /// <returns></returns>
        public static bool SetWindowTopMost(IntPtr hWnd,bool bTop)
        {
            IntPtr option;
            if (bTop)
                option = new IntPtr(-1);
            else
                option = new IntPtr(-2);
            return SetWindowPos(hWnd, option, 0, 0, 0, 0, (ShowWindowFlag.NOSIZE | ShowWindowFlag.NOMOVE | ShowWindowFlag.SHOWWINDOW));
        }

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
        public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// 모든 윈도우를 순회하는 윈도우 API함수.
        /// </summary>
        /// <param name="lpEnumFunc"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

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

        /// <summary>
        /// Get Winddow Text Length
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);


        [Flags]
        public enum SYMBOLIC_LINK_FLAG
        {
            File = 0,
            Directory = 1,
            AllowUnprivilegedCreate = 2
        }

        /// <summary>
        /// 심볼릭 링크 생성 api
        /// </summary>
        /// <param name="lpSymlinkFileName"></param>
        /// <param name="lpTargetFileName"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SYMBOLIC_LINK_FLAG dwFlags);

        #region 스크린 캡쳐 기능을 위함... C로 직접 구현하려다 dll 프로젝트 만들기 귀찮아서 안함.....

        /// <summary>
        /// Win32에 넘겨서 사용할 RECT정의
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Width
            {
                get
                {
                    return Right - Left;
                }
            }

            public int Height
            {
                get
                {
                    return Bottom - Top;
                }
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr hDC);


        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr targetHandle, IntPtr sourceObjectHandle);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr DeleteObject(IntPtr hDc);


        public enum ObjectType : uint
        {
            OBJ_PEN = 1,
            OBJ_BRUSH = 2,
            OBJ_DC = 3,
            OBJ_METADC = 4,
            OBJ_PAL = 5,
            OBJ_FONT = 6,
            OBJ_BITMAP = 7,
            OBJ_REGION = 8,
            OBJ_METAFILE = 9,
            OBJ_MEMDC = 10,
            OBJ_EXTPEN = 11,
            OBJ_ENHMETADC = 12,
            OBJ_ENHMETAFILE = 13
        }

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetCurrentObject(IntPtr hdc, ObjectType uObjectType);

        /// <summary>
        /// DC로부터 Bitmap얻기 (width, height알기위함이라던가)
        /// </summary>
        /// <param name="hDC"></param>
        /// <returns></returns>
        public static Bitmap GetBitMapFromHBitmap(IntPtr hDC)
        {
            IntPtr hBitmap = GetCurrentObject(hDC, ObjectType.OBJ_BITMAP);
            if(hBitmap != IntPtr.Zero) {
                return Bitmap.FromHbitmap(hBitmap);
            }
            return null;
        }

        public static Bitmap getProcessBitmap(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
                return null;

            IntPtr hDC = GetWindowDC(hWnd);
            IntPtr memDC = CreateCompatibleDC(hDC);

            RECT rc;
            GetClientRect(hWnd,out rc);

            IntPtr memBitMap = CreateCompatibleBitmap(hDC, rc.Width, rc.Height);

            //메모리 DC가 비트맵 사용하도록 세팅하고.,,
            SelectObject(memDC, memBitMap);

            ///DC복제
            BitBlt(memDC, 0, 0, rc.Width, rc.Height, hDC, 0, 0, TernaryRasterOperations.SRCCOPY | TernaryRasterOperations.CAPTUREBLT);

            //그려진 내용으로부터 비트맵 얻고
            Bitmap retVal = Bitmap.FromHbitmap(memBitMap);

            //메모리 해제
            DeleteObject(memBitMap);
            DeleteDC(memDC);
            ReleaseDC(hWnd, hDC);

            return retVal;
        }


        public static BitmapImage getProcessScreenImg(IntPtr hWnd,string name, int dbgVal = 0)
        {
            try {
                var bitmap = getProcessBitmap(hWnd);
                if (bitmap == null) return null;
                if (dbgVal > 0) {
                    bitmap.Save($"d:\\tmp\\pr\\{name}_{dbgVal}.bmp");
                }
                using (MemoryStream m = new MemoryStream()) {
                    bitmap.Save(m, ImageFormat.Bmp);
                    m.Position = 0;
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = m;
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();

                    return img;
                }
            } catch {
                return null;
            }
        }




        #endregion
    }
}
