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
            materialTextField1 = new MaterialWinForms.Components.Inputs.MaterialTextField();
            SuspendLayout();
            // 
            // materialButton1
            // 
            materialButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            materialButton1.BackColor = Color.Transparent;
            materialButton1.Elevation = 2;
            materialButton1.Location = new Point(12, 352);
            materialButton1.Name = "materialButton1";
            materialButton1.Size = new Size(776, 86);
            materialButton1.TabIndex = 0;
            materialButton1.Type = MaterialWinForms.Components.Buttons.MaterialButton.ButtonType.Contained;
            // 
            // materialTextField1
            // 
            materialTextField1.BackColor = Color.White;
            materialTextField1.HintText = "";
            materialTextField1.IsFloatingLabel = true;
            materialTextField1.Location = new Point(191, 218);
            materialTextField1.Name = "materialTextField1";
            materialTextField1.Size = new Size(485, 74);
            materialTextField1.TabIndex = 1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(materialTextField1);
            Controls.Add(materialButton1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private MaterialWinForms.Components.Buttons.MaterialButton materialButton1;
        private MaterialWinForms.Components.Inputs.MaterialTextField materialTextField1;
    }
}
