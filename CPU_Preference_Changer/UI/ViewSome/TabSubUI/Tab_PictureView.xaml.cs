using CPU_Preference_Changer.WinAPI_Wrapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CPU_Preference_Changer.UI.ViewSome.TabSubUI
{
    /// <summary>
    /// Render순간을 오버라이드하기위해 상속받아 커스텀 이미지 컨트롤 구현...
    /// </summary>
    public class CustImageCtl : System.Windows.Controls.Image
    {
        public byte alphaValue { get; set; }
        protected override void OnRender(DrawingContext dc)
        {
            double v = (alphaValue / 255.0f);
            dc.PushOpacity(v);
            base.OnRender(dc);
        }
    }

    /// <summary>
    /// Tab_PictureView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Tab_PictureView : System.Windows.Controls.UserControl, ITransCanChange, IDestroyControl, INotifyPropertyChanged
    {
        #region 과거에 WinForm환경에서 만들었었던 GifLoader업그레이드..
        /// <summary>
        /// Gif로더...
        /// </summary>
        internal class GifLoader
        {
            private FrameDimension frameDimension;

            private int frameCount;
            private int currentFrame;

            private int duration;
            private List<System.Drawing.Image> frameList = new List<System.Drawing.Image>();

            /// <summary>
            /// GIF로더
            /// </summary>
            /// <param name="img"></param>
            public GifLoader(System.Drawing.Image img)
            {
                frameDimension = new FrameDimension(img.FrameDimensionsList[0]);
                frameCount = img.GetFrameCount(frameDimension);
                currentFrame = 0;

                int index = 0;
                for (int i = 0; i < frameCount; i++) {
                    img.SelectActiveFrame(frameDimension, i);
                    frameList.Add(img.Clone() as System.Drawing.Image);
                    var delay = BitConverter.ToInt32(img.GetPropertyItem(20736).Value, index) * 10;
                    duration += (delay < 100 ? 100 : delay);
                    index += 4;
                }
                img.Dispose();
            }

            /// <summary>
            /// 현재 프레임 얻기
            /// </summary>
            /// <returns></returns>
            private System.Drawing.Image getFrame()
            {
                /*
                gifFile.SelectActiveFrame(frameDimension, currentFrame);
                currentFrame += 1;
                return (System.Drawing.Image)gifFile.Clone();
                */
                return frameList[currentFrame];
            }

            /// <summary>
            /// 현재 프레임(bitmap) 얻기
            /// </summary>
            /// <returns></returns>
            private System.Drawing.Bitmap getFrameBitMap()
            {
                var curImage = getFrame();
                var imageBitmap = new System.Drawing.Bitmap(curImage.Width, curImage.Height);
                using (var graphics = Graphics.FromImage(imageBitmap)) {
                    graphics.DrawImage(curImage, 0, 0, curImage.Width, curImage.Height);
                }
                return imageBitmap;
            }

            /// <summary>
            /// 미리 ImageSource배열을 생성하여 변환 후 담아두어보았으나 잘 안되던..
            /// 그냥 바로바로 만들어서 반환하게 한다.
            /// </summary>
            /// <returns></returns>
            public BitmapImage getNextFrame()
            {
                if ((currentFrame >= frameCount)) {
                    currentFrame = 0;
                }
                var curFrame = getFrameBitMap();
                using (MemoryStream m = new MemoryStream()) {
                    curFrame.Save(m, ImageFormat.Bmp);
                    m.Position = 0;
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.StreamSource = m;
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    currentFrame++;
                    return img;
                }
                /*
                var handle = curFrame.GetHbitmap();
                try {
                    return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                } finally { 
                    WinAPI.DeleteObject(handle);
                    currentFrame++;
                }
                */
            }

            public int getSleepTime()
            {
                int t = ((int)(duration / frameCount));
                if (t > 1)
                    return t;
                else return 1; /*아무리못해도 1미리 슬립 줌*/
            }

        }
        #endregion

        public Tab_PictureView()
        {
            InitializeComponent();
            DataContext = this;
            gifPlay(); /*gif플레이용 스레드 시작*/
        }

        public void setBackgroundTrans(byte alpha, byte r, byte g, byte b)
        {
            imgView.alphaValue = alpha;
            PropChanged("usrImgSrc");
        }

        private bool waitForGifRenderEnd()
        {
            while (renderState == gifRenderState.drawing) {
                bGifPlay = false;
                Thread.Sleep(10);
            }
            return true;
        }

        private async void displayPicture(string fileName)
        {
            var task = Task.Run(() => waitForGifRenderEnd());
            _ = await task;
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fileName);
            bitmap.EndInit();
            usrImgSrc = bitmap;
        }

        private void btnSelPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofi = new OpenFileDialog();
            ofi.InitialDirectory = "c:\\";
            ofi.Filter = "Image files (*.jpg)|*.jpg|Image files (*.png)|*.png|Image files (*.bmp)|*.bmp|Image files (*.gif)|*.gif|All Files (*.*)|*.*";
            ofi.RestoreDirectory = true;
            if (ofi.ShowDialog() == DialogResult.OK) {
                string selectedFileName = ofi.FileName;
                if (selectedFileName.ToUpper().EndsWith("GIF")) {
                    lock (gifFilePath) {
                        gifFilePath=selectedFileName;
                        bGifPlay = true;
                    }
                } else {
                    /*gif플레이 스레드에서 Dispatch를 얻어서 작업하기때문에 여기 While문에서 블락당하면
                     데드락이다.. 회피를 위해 버튼 스레드 처리중인 메인 스레드 종료 시켜주고,
                    별도 Task로 처리*/
                    displayPicture(selectedFileName);
                }
            }
        }

        private volatile bool bEndUI=false;
        private Thread gifPlayThread = null;
        private GifLoader gLoader = null;
        private volatile bool bGifPlay = false;
        private string gifFilePath = string.Empty;
        private volatile gifRenderState renderState = gifRenderState.ready;

        public event PropertyChangedEventHandler PropertyChanged;
        private void PropChanged(string propName) {
            PropertyChangedEventHandler _pc = PropertyChanged;
            if (_pc != null) {
                _pc(this, new PropertyChangedEventArgs(propName));
            }
        }

        public BitmapImage _usrImgSrc;
        public BitmapImage usrImgSrc {
            get { return _usrImgSrc; }
            set {
                _usrImgSrc = value;
                PropChanged("usrImgSrc");
            }

        }


        internal enum gifRenderState
        {
            ready,
            drawing
        }

        /// <summary>
        /// gif플레이를 위한 스레드
        /// </summary>
        private void gifPlay()
        {
            gifPlayThread = new Thread(() =>
            {
                while (bEndUI == false) {
                    if (bGifPlay == false) {
                        Thread.Sleep(100);
                        continue;
                    }
                    lock (gifFilePath) {
                        gLoader = new GifLoader(new System.Drawing.Bitmap(gifFilePath));
                    }
                    if (gLoader == null) {
                        bGifPlay = false;
                        continue;
                    }
                    bool bFirst = false;
                    renderState = gifRenderState.drawing;
                    while (bGifPlay == true && bEndUI==false) {
                        try {
                            if (gLoader != null) {
                                Dispatcher.Invoke(() =>
                                {
                                    if (bFirst) {
                                        PropChanged("usrImgSrc");
                                        bFirst = false;
                                    }
                                    usrImgSrc = gLoader.getNextFrame();
                                });
                            }
                        } catch (Exception err ) {
                            
                        }
                        Thread.Sleep(gLoader.getSleepTime());
                    }
                    renderState = gifRenderState.ready;
                }
                
            });
            gifPlayThread.Start();
        }

        /// <summary>
        /// 컨트롤 Unload상태 ( 탭 이동으로 인해 보여지지 않을 때, 진짜로 창 닫을 때 )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            /*gif그림 렌더링을 멈추거나 할수도 있지만.. 귀찮으니 안한다.........*/
        }

        /// <summary>
        /// 컨트롤 Load상태 (탭 이동으로 인해 보여질 때, 최초로 생성될 때 )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// 외부 Window가 Unload될 때 호출 됨
        /// </summary>
        public void onDestroy()
        {
            bEndUI = true;
        }
    }
}
