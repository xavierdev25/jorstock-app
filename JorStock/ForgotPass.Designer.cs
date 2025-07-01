namespace JorStock
{
    partial class ForgotPass
    {
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
            if (disposing && (components != null))
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ForgotPass));
            this.btnSiguiente_Forgot = new System.Windows.Forms.Button();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSiguiente_Forgot
            // 
            this.btnSiguiente_Forgot.BackColor = System.Drawing.Color.Transparent;
            this.btnSiguiente_Forgot.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnSiguiente_Forgot.BackgroundImage")));
            this.btnSiguiente_Forgot.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnSiguiente_Forgot.FlatAppearance.BorderSize = 0;
            this.btnSiguiente_Forgot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSiguiente_Forgot.ForeColor = System.Drawing.Color.Transparent;
            this.btnSiguiente_Forgot.Location = new System.Drawing.Point(395, 428);
            this.btnSiguiente_Forgot.Name = "btnSiguiente_Forgot";
            this.btnSiguiente_Forgot.Size = new System.Drawing.Size(185, 80);
            this.btnSiguiente_Forgot.TabIndex = 0;
            this.btnSiguiente_Forgot.UseVisualStyleBackColor = false;
            // 
            // txtMail
            // 
            this.txtMail.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMail.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMail.Location = new System.Drawing.Point(236, 353);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(498, 24);
            this.txtMail.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(900, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(65, 65);
            this.button2.TabIndex = 2;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Transparent;
            this.button3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button3.BackgroundImage")));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(829, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 65);
            this.button3.TabIndex = 3;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // ForgotPass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(975, 675);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtMail);
            this.Controls.Add(this.btnSiguiente_Forgot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ForgotPass";
            this.Text = "ForgotPass";
            this.Load += new System.EventHandler(this.ForgotPass_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSiguiente_Forgot;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}