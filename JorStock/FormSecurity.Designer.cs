namespace JorStock
{
    partial class FormSecurity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSecurity));
            this.btnIngresar_Security = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnIngresar_Security
            // 
            this.btnIngresar_Security.BackColor = System.Drawing.Color.Transparent;
            this.btnIngresar_Security.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnIngresar_Security.BackgroundImage")));
            this.btnIngresar_Security.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnIngresar_Security.FlatAppearance.BorderSize = 0;
            this.btnIngresar_Security.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIngresar_Security.ForeColor = System.Drawing.Color.Transparent;
            this.btnIngresar_Security.Location = new System.Drawing.Point(396, 438);
            this.btnIngresar_Security.Name = "btnIngresar_Security";
            this.btnIngresar_Security.Size = new System.Drawing.Size(185, 80);
            this.btnIngresar_Security.TabIndex = 0;
            this.btnIngresar_Security.UseVisualStyleBackColor = false;
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCode.Location = new System.Drawing.Point(236, 384);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(502, 24);
            this.txtCode.TabIndex = 1;
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
            // FormSecurity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(975, 675);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.btnIngresar_Security);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSecurity";
            this.Text = "FormSecurity";
            this.Load += new System.EventHandler(this.FormSecurity_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnIngresar_Security;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}