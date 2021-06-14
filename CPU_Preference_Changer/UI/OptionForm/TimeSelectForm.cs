using System;
using System.Windows.Forms;

namespace CPU_Preference_Changer.UI.OptionForm {
    public partial class TimeSelectForm : Form {

        public DateTime selTime { get; private set; }

        public TimeSelectForm(string titleName)
        {
            Application.EnableVisualStyles();
            this.Text = titleName;
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now;
            this.DialogResult = DialogResult.Cancel;
        }

        private void btOk_Click(object sender, EventArgs e)
        {
            selTime = dateTimePicker1.Value;
            if ( selTime.CompareTo(DateTime.Now) <= 0) {
                MessageBox.Show("현재보다 과거 시간을 선택할 수는 없습니다!","안내");
                return;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
