using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPU_Preference_Changer.UI.ViewSome.TabSubUI
{
    /// <summary>
    /// ChromeBathCalculator.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ChromeBathCalculator : UserControl, ITransCanChange
    {
        public ChromeBathCalculator()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 숫자만 입력하게,,, ( 음수도 허용 )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbHint_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("[^0-9]+");
            e.Handled = r.IsMatch(e.Text);
            if (e.Text.Equals("-")) {
                TextBox tb = sender as TextBox;
                if(tb != null) {
                    /* 마이너스 기호는 맨 앞에만.*/
                    if(tb.CaretIndex != 0) {
                        e.Handled = true;
                        return;
                    }
                    /* 마이너스기호 있는데 또 넣기 금지*/
                    if ( tb.Text.IndexOf('-')>=0) {
                        e.Handled = true;
                        return;
                    }
                }
                e.Handled = false;
            }
        }

        /// <summary>
        /// hint Value얻기
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool getHintValue(TextBox tb, out int val)
        {
            val = 0;
            if( string.IsNullOrEmpty(tb.Text)==true) {
                return false;
            }
            return int.TryParse(tb.Text, out val);
        }

        private string getSelectedRadioTxt()
        {
            if(rb_lu.IsChecked==true) {
                return rb_lu.Content.ToString();
            }
            if (rb_ba.IsChecked == true) {
                return rb_ba.Content.ToString();
            }
            if (rb_bel.IsChecked == true) {
                return rb_bel.Content.ToString();
            }
            if (rb_sam.IsChecked == true) {
                return rb_sam.Content.ToString();
            }
            if (rb_im.IsChecked == true) {
                return rb_im.Content.ToString();
            }
            /*여기에 도달할수 없겠지만 예외처리*/
            return "";
        }

        /// <summary>
        /// 텍스트 변할 때..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbHint_TextChanged(object sender, TextChangedEventArgs e)
        {
            calcHint();
        }

        private void calcHint()
        {
            int hint1, hint2, hint3;
            if (getHintValue(tbHint1, out hint1) == false) {
                return;
            }
            if (getHintValue(tbHint2, out hint2) == false) {
                return;
            }
            if (getHintValue(tbHint3, out hint3) == false) {
                return;
            }
            string strVal = getSelectedRadioTxt();
            btRet0.Content = $"{(hint1 + hint2 + hint3) + strVal}";
            btRet1.Content = $"{(hint1 + hint2 - hint3) + strVal}";
            btRet2.Content = $"{(hint1 * hint2 * hint3) + strVal}";
            btRet3.Content = $"{(hint1 * hint2 + hint3) + strVal}";
            btRet4.Content = $"{(hint1 * hint2 - hint3) + strVal}";
            btRet5.Content = $"{(hint1 - hint2 + hint3) + strVal}";
            btRet6.Content = $"{(hint1 - hint2 - hint3) + strVal}";
        }

        private void btRet_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;
            Clipboard.SetDataObject(b.Content.ToString());
        }

        /// <summary>
        /// 백그라운드 투명도 조정..
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public void setBackgroundTrans(byte alpha, byte r, byte g, byte b)
        {
            outGrid.Background = new SolidColorBrush() { Color = Color.FromArgb(alpha, r, g, b) };
        }

        /// <summary>
        /// 라디오 체크 변할 때..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            calcHint();
        }
    }
}
