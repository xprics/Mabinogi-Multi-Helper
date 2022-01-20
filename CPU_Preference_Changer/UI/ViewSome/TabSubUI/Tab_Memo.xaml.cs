using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPU_Preference_Changer.UI.ViewSome.TabSubUI
{
    /// <summary>
    /// Tab_Memo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Tab_Memo : System.Windows.Controls.UserControl, INotifyPropertyChanged, ITransCanChange
    {
        private string _memoTxt;

        public event PropertyChangedEventHandler PropertyChanged;

        public string memoTxt {
            get { return _memoTxt; }
            set {
                _memoTxt = value;
                propChanged(value);
            }
        }

        private void propChanged(string propName)
        {
            PropertyChangedEventHandler _PropertyChanged = PropertyChanged;
            if (_PropertyChanged != null) {
                _PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public Tab_Memo()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// 복사 버튼 클릭 (클립보드로 복사)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try {
                System.Windows.Clipboard.SetDataObject(memoTxt);
            } catch {
                return;
            }
        }

        /// <summary>
        /// 배경색 조정 
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void setBackgroundTrans(byte alpha, byte r, byte g, byte b)
        {
            //outGrid.Background = new SolidColorBrush() { Color = Color.FromArgb(alpha, r, g, b) };
            /*투명이라 상위 윈도우 색그대로 출력해줄것이니 딱히 안해도..
             alpha값이 중첩되어 오히려 이상하게보인다*/
        }

        /// <summary>
        /// 색상 선택   언젠가 WPF윈도우로 만들어서 연결..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelColor_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Color sel = System.Drawing.Color.Black;
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK) {
                sel= cd.Color;
            }
            tbMemo.Foreground = new SolidColorBrush() { Color = Color.FromRgb(sel.R, sel.G, sel.B) };
        }

        /// <summary>
        /// 폰트선택.. 언젠가 WPF윈도우로 만들어서 연결..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelFont_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Font sel;

            FontDialog fd = new FontDialog();
            fd.ShowEffects = false;
            fd.ShowColor = true;
            if (fd.ShowDialog() == DialogResult.OK) {
                sel = fd.Font;
            }else {
                return;
            }
            tbMemo.FontFamily = new FontFamily(sel.Name);
            tbMemo.FontSize = sel.Size;
            if (sel.Bold) {
                tbMemo.FontWeight = FontWeights.Bold;
            } else {
                tbMemo.FontWeight = FontWeights.Normal;
            }
        }

    }
}
