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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Nav1));
            materialCard1 = new MaterialWinForms.Components.Containers.MaterialCard();
            materialCard2 = new MaterialWinForms.Components.Containers.MaterialCard();
            materialfab1 = new MaterialWinForms.Components.Buttons.MaterialFAB();
            materialSwitch1 = new MaterialWinForms.Components.Selection.MaterialSwitch();
            materialSwitch2 = new MaterialWinForms.Components.Selection.MaterialSwitch();
            materialCard1.SuspendLayout();
            materialCard2.SuspendLayout();
            SuspendLayout();
            // 
            // materialCard1
            // 
            materialCard1.AllowDrop = true;
            materialCard1.AutoScroll = true;
            materialCard1.AutoScrollMinSize = new Size(0, 0);
            materialCard1.BackColor = Color.Transparent;
            materialCard1.Controls.Add(materialSwitch2);
            materialCard1.CornerRadius.All = 12;
            materialCard1.CornerRadius.BottomLeft = 12;
            materialCard1.CornerRadius.BottomRight = 12;
            materialCard1.CornerRadius.TopLeft = 12;
            materialCard1.CornerRadius.TopRight = 12;
            materialCard1.Location = new Point(128, 102);
            materialCard1.Name = "materialCard1";
            materialCard1.Padding = new Padding(16);
            materialCard1.Shadow.Opacity = 30;
            materialCard1.Size = new Size(259, 229);
            materialCard1.SubtitleSettings.FontSize = 12F;
            materialCard1.TabIndex = 0;
            materialCard1.TitleSettings.FontSize = 16F;
            materialCard1.TitleSettings.FontStyle = FontStyle.Bold;
            // 
            // materialCard2
            // 
            materialCard2.AllowDrop = true;
            materialCard2.AutoScroll = true;
            materialCard2.AutoScrollMinSize = new Size(0, 0);
            materialCard2.BackColor = Color.Transparent;
            materialCard2.Controls.Add(materialSwitch1);
            materialCard2.Controls.Add(materialfab1);
            materialCard2.CornerRadius.All = 12;
            materialCard2.CornerRadius.BottomLeft = 12;
            materialCard2.CornerRadius.BottomRight = 12;
            materialCard2.CornerRadius.TopLeft = 12;
            materialCard2.CornerRadius.TopRight = 12;
            materialCard2.Location = new Point(404, 102);
            materialCard2.Name = "materialCard2";
            materialCard2.Padding = new Padding(16);
            materialCard2.Shadow.Opacity = 30;
            materialCard2.Size = new Size(259, 229);
            materialCard2.SubtitleSettings.FontSize = 12F;
            materialCard2.TabIndex = 1;
            materialCard2.TitleSettings.FontSize = 16F;
            materialCard2.TitleSettings.FontStyle = FontStyle.Bold;
            // 
            // materialfab1
            // 
            materialfab1.BackColor = Color.Transparent;
            materialfab1.CornerRadius.All = 40;
            materialfab1.CornerRadius.BottomLeft = 40;
            materialfab1.CornerRadius.BottomRight = 40;
            materialfab1.CornerRadius.TopLeft = 40;
            materialfab1.CornerRadius.TopRight = 40;
            materialfab1.Elevation = 6;
            materialfab1.Icon = (Image)resources.GetObject("materialfab1.Icon");
            materialfab1.IconSettings.Size = 32;
            materialfab1.Location = new Point(160, 130);
            materialfab1.Name = "materialfab1";
            materialfab1.Padding = new Padding(8);
            materialfab1.Shadow.Blur = 12;
            materialfab1.Shadow.OffsetY = 4;
            materialfab1.Shadow.Opacity = 15;
            materialfab1.Shadow.Type = MaterialWinForms.Core.MaterialShadowType.Glow;
            materialfab1.Size = MaterialWinForms.Components.Buttons.MaterialFAB.FABSize.Normal;
            materialfab1.TabIndex = 0;
            materialfab1.TextContent = "";
            materialfab1.TextSettings.FontSize = 10F;
            materialfab1.TextSettings.FontStyle = FontStyle.Bold;
            // 
            // materialSwitch1
            // 
            materialSwitch1.BackColor = Color.Transparent;
            materialSwitch1.Checked = false;
            materialSwitch1.Location = new Point(19, 19);
            materialSwitch1.Name = "materialSwitch1";
            materialSwitch1.Size = new Size(52, 32);
            materialSwitch1.TabIndex = 1;
            // 
            // materialSwitch2
            // 
            materialSwitch2.BackColor = Color.Transparent;
            materialSwitch2.Checked = false;
            materialSwitch2.Location = new Point(19, 19);
            materialSwitch2.Name = "materialSwitch2";
            materialSwitch2.Size = new Size(52, 32);
            materialSwitch2.TabIndex = 0;
            // 
            // Nav1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(materialCard2);
            Controls.Add(materialCard1);
            Name = "Nav1";
            Text = "Nav1";
            materialCard1.ResumeLayout(false);
            materialCard2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private MaterialWinForms.Components.Containers.MaterialCard materialCard1;
        private MaterialWinForms.Components.Containers.MaterialCard materialCard2;
        private MaterialWinForms.Components.Buttons.MaterialFAB materialfab1;
        private MaterialWinForms.Components.Selection.MaterialSwitch materialSwitch2;
        private MaterialWinForms.Components.Selection.MaterialSwitch materialSwitch1;
    }
}