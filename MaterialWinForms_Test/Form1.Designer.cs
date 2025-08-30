namespace MaterialWinForms_Test
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            materialButton1 = new MaterialWinForms.Components.Buttons.MaterialButton();
            SuspendLayout();
            // 
            // materialButton1
            // 
            materialButton1.BackColor = Color.Transparent;
            materialButton1.Location = new Point(217, 466);
            materialButton1.Name = "materialButton1";
            materialButton1.Padding = new Padding(4);
            materialButton1.Size = new Size(681, 214);
            materialButton1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1040, 727);
            Controls.Add(materialButton1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private MaterialWinForms.Components.Buttons.MaterialButton materialButton1;
    }
}
