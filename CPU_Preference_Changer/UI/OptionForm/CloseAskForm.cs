using System;
using System.Windows.Forms;

namespace CPU_Preference_Changer.UI.OptionForm {
    public partial class CloseAskForm : Form {

        public enum CloseAskFormResult {
            eClose=1,
            eGoTray=2,
            eCancel=3
        }

        /// <summary>
        /// default value = 취소로 냅둠. (아무것도 안하고 창 닫으면 아무것도 안하게 함)
        /// </summary>
        CloseAskFormResult ret = CloseAskFormResult.eCancel;

        public CloseAskForm()
        {
            Application.EnableVisualStyles();
            InitializeComponent();
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        /// <summary>
        /// 유저가 어떤 버튼 눌렀는지 결과 얻기.
        /// </summary>
        /// <returns></returns>
        public CloseAskFormResult getUsrSelect() { return ret; }

        private void bt_GoTray_Click(object sender, EventArgs e)
        {
            ret = CloseAskFormResult.eGoTray;
            this.Close();
        }

        private void bt_Close_Click(object sender, EventArgs e)
        {
            ret = CloseAskFormResult.eClose;
            this.Close();
        }
    }
}
