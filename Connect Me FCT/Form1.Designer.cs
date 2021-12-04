
namespace Connect_Me_FCT
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.CantidadBox = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ModelCombo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unidades a probar";
            // 
            // CantidadBox
            // 
            this.CantidadBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.CantidadBox.DisplayMember = "1";
            this.CantidadBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CantidadBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CantidadBox.FormattingEnabled = true;
            this.CantidadBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.CantidadBox.Location = new System.Drawing.Point(125, 12);
            this.CantidadBox.Name = "CantidadBox";
            this.CantidadBox.Size = new System.Drawing.Size(227, 21);
            this.CantidadBox.TabIndex = 1;
            this.CantidadBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.CantidadBox_DrawItem);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(385, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(103, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseHover += new System.EventHandler(this.pictureBox1_MouseHover);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Modelo a probar";
            // 
            // ModelCombo
            // 
            this.ModelCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ModelCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ModelCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModelCombo.FormattingEnabled = true;
            this.ModelCombo.Items.AddRange(new object[] {
            "DIT-DC-ME4-01T-C (DIT-55001129-03)",
            "DIT-DC-ME-Y413-LX (DIT-55001129-15)",
            "DIT-DC-ME-01T-C(DIT-55001129-01)",
            "DIT-DC-ME-Y401-C(DIT-55001129-05)",
            "DIT-DC-ME-Y402-LX(DIT-55001129-52)",
            "DIT-DC-ME-Y402-C(DIT-55001129-06)",
            "DIT-DC-ME-Y402-S-UPW(DIT-55001129-54)",
            "DIT-DC-ME-Y402-S(DIT-55001129-52)",
            "DIT-DC-ME4-01T-S-UPW(DIT-55001129-03)",
            "DIT-DC-ME4-01T-S(DIT-55001129-03)",
            "DIT-DC-ME-01T-PC(DIT-55001129-09)",
            "DIT-DC-ME-01T-PS(DIT-55001129-09)",
            "DIT-DC-ME-01T-S(DIT-55001129-01)"});
            this.ModelCombo.Location = new System.Drawing.Point(125, 66);
            this.ModelCombo.Name = "ModelCombo";
            this.ModelCombo.Size = new System.Drawing.Size(227, 21);
            this.ModelCombo.TabIndex = 4;
            this.ModelCombo.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.ModelCombo_DrawItem);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(498, 97);
            this.Controls.Add(this.ModelCombo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.CantidadBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CantidadBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ModelCombo;
    }
}

