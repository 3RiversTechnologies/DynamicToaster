namespace Toaster
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
            this.ProcessLotButton = new System.Windows.Forms.RadioButton();
            this.ProcessAssetButton = new System.Windows.Forms.RadioButton();
            this.LotNumberTextBox = new System.Windows.Forms.TextBox();
            this.NextButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ProcessLotButton
            // 
            this.ProcessLotButton.AutoSize = true;
            this.ProcessLotButton.Checked = true;
            this.ProcessLotButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessLotButton.Location = new System.Drawing.Point(12, 36);
            this.ProcessLotButton.Name = "ProcessLotButton";
            this.ProcessLotButton.Size = new System.Drawing.Size(476, 95);
            this.ProcessLotButton.TabIndex = 0;
            this.ProcessLotButton.TabStop = true;
            this.ProcessLotButton.Text = "Process Lot";
            this.ProcessLotButton.UseVisualStyleBackColor = true;
            this.ProcessLotButton.CheckedChanged += new System.EventHandler(this.ProcessLotButton_CheckedChanged);
            // 
            // ProcessAssetButton
            // 
            this.ProcessAssetButton.AutoSize = true;
            this.ProcessAssetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 60F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessAssetButton.Location = new System.Drawing.Point(12, 226);
            this.ProcessAssetButton.Name = "ProcessAssetButton";
            this.ProcessAssetButton.Size = new System.Drawing.Size(605, 95);
            this.ProcessAssetButton.TabIndex = 1;
            this.ProcessAssetButton.Text = "Process Assets";
            this.ProcessAssetButton.UseVisualStyleBackColor = true;
            this.ProcessAssetButton.CheckedChanged += new System.EventHandler(this.ProcessAssetButton_CheckedChanged);
            // 
            // LotNumberTextBox
            // 
            this.LotNumberTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LotNumberTextBox.Location = new System.Drawing.Point(79, 146);
            this.LotNumberTextBox.Name = "LotNumberTextBox";
            this.LotNumberTextBox.Size = new System.Drawing.Size(409, 53);
            this.LotNumberTextBox.TabIndex = 2;
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(12, 347);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(760, 52);
            this.NextButton.TabIndex = 3;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.NextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 411);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.LotNumberTextBox);
            this.Controls.Add(this.ProcessAssetButton);
            this.Controls.Add(this.ProcessLotButton);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton ProcessLotButton;
        private System.Windows.Forms.RadioButton ProcessAssetButton;
        private System.Windows.Forms.TextBox LotNumberTextBox;
        private System.Windows.Forms.Button NextButton;
    }
}

