namespace MaterialWinForms_Test
{
    partial class Nav1
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
            materialCard1 = new MaterialWinForms.Components.Containers.MaterialCard();
            SuspendLayout();
            // 
            // materialCard1
            // 
            materialCard1.AllowDrop = true;
            materialCard1.AutoScroll = true;
            materialCard1.AutoScrollMinSize = new Size(0, 0);
            materialCard1.BackColor = Color.Transparent;
            materialCard1.CornerRadius.All = 12;
            materialCard1.CornerRadius.BottomLeft = 12;
            materialCard1.CornerRadius.BottomRight = 12;
            materialCard1.CornerRadius.TopLeft = 12;
            materialCard1.CornerRadius.TopRight = 12;
            materialCard1.Location = new Point(238, 86);
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(16);
            materialCard1.Shadow.Opacity = 30;
            materialCard1.Size = new Size(300, 200);
            materialCard1.SubtitleSettings.FontSize = 12F;
            materialCard1.TabIndex = 0;
            materialCard1.TitleSettings.FontSize = 16F;
            materialCard1.TitleSettings.FontStyle = FontStyle.Bold;
            // 
            // Nav1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(materialCard1);
            Name = "Nav1";
            Text = "Nav1";
            ResumeLayout(false);
        }

        #endregion

        private MaterialWinForms.Components.Containers.MaterialCard materialCard1;
    }
}