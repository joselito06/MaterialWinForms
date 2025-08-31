namespace MaterialWinForms_Test
{
    partial class Nav2
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
            materialButton1 = new MaterialWinForms.Components.Buttons.MaterialButton();
            SuspendLayout();
            // 
            // materialButton1
            // 
            materialButton1.BackColor = Color.Transparent;
            materialButton1.CornerRadius.All = 20;
            materialButton1.CornerRadius.BottomLeft = 20;
            materialButton1.CornerRadius.BottomRight = 20;
            materialButton1.CornerRadius.TopLeft = 20;
            materialButton1.CornerRadius.TopRight = 20;
            materialButton1.Location = new Point(264, 353);
            materialButton1.Name = "materialButton1";
            materialButton1.Padding = new Padding(8);
            materialButton1.Shadow.Blur = 8;
            materialButton1.Shadow.Opacity = 20;
            materialButton1.Shadow.Type = MaterialWinForms.Core.MaterialShadowType.Glow;
            materialButton1.Size = new Size(263, 85);
            materialButton1.TabIndex = 0;
            // 
            // Nav2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(materialButton1);
            Name = "Nav2";
            Text = "Nav2";
            ResumeLayout(false);
        }

        #endregion

        private MaterialWinForms.Components.Buttons.MaterialButton materialButton1;
    }
}