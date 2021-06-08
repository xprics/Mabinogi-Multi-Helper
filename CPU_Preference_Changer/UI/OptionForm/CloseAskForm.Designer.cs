
namespace CPU_Preference_Changer.UI.OptionForm {
    partial class CloseAskForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloseAskForm));
            this.bt_GoTray = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bt_GoTray
            // 
            this.bt_GoTray.Location = new System.Drawing.Point(29, 47);
            this.bt_GoTray.Name = "bt_GoTray";
            this.bt_GoTray.Size = new System.Drawing.Size(193, 25);
            this.bt_GoTray.TabIndex = 0;
            this.bt_GoTray.Text = "Tray로 이동~";
            this.bt_GoTray.UseVisualStyleBackColor = true;
            this.bt_GoTray.Click += new System.EventHandler(this.bt_GoTray_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(202, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "프로그램을 종료하시겠습니까?";
            // 
            // bt_Close
            // 
            this.bt_Close.Location = new System.Drawing.Point(29, 92);
            this.bt_Close.Name = "bt_Close";
            this.bt_Close.Size = new System.Drawing.Size(193, 25);
            this.bt_Close.TabIndex = 0;
            this.bt_Close.Text = "종료~~!";
            this.bt_Close.UseVisualStyleBackColor = true;
            this.bt_Close.Click += new System.EventHandler(this.bt_Close_Click);
            // 
            // CloseAskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 135);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_Close);
            this.Controls.Add(this.bt_GoTray);
            this.Font = new System.Drawing.Font("굴림", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CloseAskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "확인";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_GoTray;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_Close;
    }
}