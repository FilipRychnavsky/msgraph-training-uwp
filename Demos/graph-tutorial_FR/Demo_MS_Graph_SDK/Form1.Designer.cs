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
			this.m_rTextBoxResult = new System.Windows.Forms.TextBox();
			this.m_rButton_OAuth20 = new System.Windows.Forms.Button();
			this.m_rButtonConnect_InteractiveAuthenticationProvider = new System.Windows.Forms.Button();
			this.m_rButtonExcel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_rTextBoxResult
			// 
			this.m_rTextBoxResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.m_rTextBoxResult.Location = new System.Drawing.Point(1053, 15);
			this.m_rTextBoxResult.Margin = new System.Windows.Forms.Padding(6);
			this.m_rTextBoxResult.Multiline = true;
			this.m_rTextBoxResult.Name = "m_rTextBoxResult";
			this.m_rTextBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.m_rTextBoxResult.Size = new System.Drawing.Size(532, 818);
			this.m_rTextBoxResult.TabIndex = 3;
			// 
			// m_rButton_OAuth20
			// 
			this.m_rButton_OAuth20.BackColor = System.Drawing.Color.PaleGreen;
			this.m_rButton_OAuth20.Location = new System.Drawing.Point(60, 133);
			this.m_rButton_OAuth20.Margin = new System.Windows.Forms.Padding(6);
			this.m_rButton_OAuth20.Name = "m_rButton_OAuth20";
			this.m_rButton_OAuth20.Size = new System.Drawing.Size(398, 92);
			this.m_rButton_OAuth20.TabIndex = 1;
			this.m_rButton_OAuth20.Text = "Connect via OAuth 2.0 (Authorization code provider)";
			this.m_rButton_OAuth20.UseVisualStyleBackColor = false;
			this.m_rButton_OAuth20.Click += new System.EventHandler(this.m_rButton_OAuth20_Click);
			// 
			// m_rButtonConnect_InteractiveAuthenticationProvider
			// 
			this.m_rButtonConnect_InteractiveAuthenticationProvider.BackColor = System.Drawing.Color.PaleGreen;
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Location = new System.Drawing.Point(60, 29);
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Margin = new System.Windows.Forms.Padding(6);
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Name = "m_rButtonConnect_InteractiveAuthenticationProvider";
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Size = new System.Drawing.Size(398, 92);
			this.m_rButtonConnect_InteractiveAuthenticationProvider.TabIndex = 0;
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Text = "Connect via InteractiveAuthenticationProvider";
			this.m_rButtonConnect_InteractiveAuthenticationProvider.UseVisualStyleBackColor = false;
			this.m_rButtonConnect_InteractiveAuthenticationProvider.Click += new System.EventHandler(this.m_rButtonConnect_InteractiveAuthenticationProvider_Click);
			// 
			// m_rButtonExcel
			// 
			this.m_rButtonExcel.BackColor = System.Drawing.Color.PaleGreen;
			this.m_rButtonExcel.Location = new System.Drawing.Point(60, 237);
			this.m_rButtonExcel.Margin = new System.Windows.Forms.Padding(6);
			this.m_rButtonExcel.Name = "m_rButtonExcel";
			this.m_rButtonExcel.Size = new System.Drawing.Size(398, 92);
			this.m_rButtonExcel.TabIndex = 2;
			this.m_rButtonExcel.Text = "New Excel OAuth 2.0 ClientSecret";
			this.m_rButtonExcel.UseVisualStyleBackColor = false;
			this.m_rButtonExcel.Click += new System.EventHandler(this.m_rButtonExcel_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1600, 865);
			this.Controls.Add(this.m_rButtonExcel);
			this.Controls.Add(this.m_rButton_OAuth20);
			this.Controls.Add(this.m_rTextBoxResult);
			this.Controls.Add(this.m_rButtonConnect_InteractiveAuthenticationProvider);
			this.Margin = new System.Windows.Forms.Padding(6);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox m_rTextBoxResult;
		private System.Windows.Forms.Button m_rButton_OAuth20;
		private System.Windows.Forms.Button m_rButtonConnect_InteractiveAuthenticationProvider;
		private System.Windows.Forms.Button m_rButtonExcel;
	}
}

