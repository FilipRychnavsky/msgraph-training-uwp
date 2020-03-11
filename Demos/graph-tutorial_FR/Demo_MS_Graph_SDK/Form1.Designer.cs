namespace Demo_MS_Graph_SDK
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
			this.m_rButtonConnect_InteractiveAuthenticationProvider = new System.Windows.Forms.Button();
			this.m_rTextBoxResult = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// m_rButtonConnect_InteractiveAuthenticationProvider
			// 
			this.m_rButtonConnect_InteractiveAuthenticationProvider.BackColor = System.Drawing.Color.PaleGreen;
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Location = new System.Drawing.Point(30, 15);
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Name = "m_rButtonConnect_InteractiveAuthenticationProvider";
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Size = new System.Drawing.Size(199, 48);
			this.m_rButtonConnect_InteractiveAuthenticationProvider.TabIndex = 0;
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Text = "Connect via InteractiveAuthenticationProvider";
			this.m_rButtonConnect_InteractiveAuthenticationProvider.UseVisualStyleBackColor = false;
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Click += new System.EventHandler(this.m_rButtonConnect_InteractiveAuthenticationProvider_Click);
			// 
			// m_rTextBoxResult
			// 
			this.m_rTextBoxResult.Location = new System.Drawing.Point(464, 30);
			this.m_rTextBoxResult.Multiline = true;
			this.m_rTextBoxResult.Name = "m_rTextBoxResult";
			this.m_rTextBoxResult.Size = new System.Drawing.Size(268, 265);
			this.m_rTextBoxResult.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.m_rTextBoxResult);
			this.Controls.Add(this.m_rButtonConnect_InteractiveAuthenticationProvider);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button m_rButtonConnect_InteractiveAuthenticationProvider;
		private System.Windows.Forms.TextBox m_rTextBoxResult;
	}
}

