
namespace CPU_Preference_Changer {
    partial class coreSelectForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(coreSelectForm));
            this.cbCheckLB = new System.Windows.Forms.CheckedListBox();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbCheckLB
            // 
            this.cbCheckLB.CheckOnClick = true;
            this.cbCheckLB.FormattingEnabled = true;
            this.cbCheckLB.Location = new System.Drawing.Point(12, 12);
            this.cbCheckLB.Name = "cbCheckLB";
            this.cbCheckLB.Size = new System.Drawing.Size(222, 212);
            this.cbCheckLB.TabIndex = 0;
            this.cbCheckLB.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.cbCheckLB_ItemCheck);
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(98, 230);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(65, 23);
            this.btOK.TabIndex = 1;
            this.btOK.Text = "확인";
            this.btOK.UseVisualStyleBackColor = false;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(169, 230);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(65, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "취소";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // coreSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(246, 261);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.cbCheckLB);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "coreSelectForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "코어 선택 창";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox cbCheckLB;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
    }
}