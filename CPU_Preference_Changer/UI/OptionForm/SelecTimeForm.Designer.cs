
namespace CPU_Preference_Changer.UI.OptionForm {
    partial class SelecTimeForm {
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
            this.dtp_shutdown = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // dtp_shutdown
            // 
            this.dtp_shutdown.Location = new System.Drawing.Point(133, 89);
            this.dtp_shutdown.Name = "dtp_shutdown";
            this.dtp_shutdown.Size = new System.Drawing.Size(200, 21);
            this.dtp_shutdown.TabIndex = 0;
            // 
            // SelecTimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dtp_shutdown);
            this.Name = "SelecTimeForm";
            this.Text = "SelecTimeForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtp_shutdown;
    }
}