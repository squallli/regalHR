namespace RegalHROutgoing
{
    partial class Main
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Browser = new System.Windows.Forms.WebBrowser();
            this.WebTimer = new System.Windows.Forms.Timer(this.components);
            this.btnBack = new System.Windows.Forms.Button();
            this.BtnClose = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Browser
            // 
            this.Browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Browser.Location = new System.Drawing.Point(0, 0);
            this.Browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.Browser.Name = "Browser";
            this.Browser.ScrollBarsEnabled = false;
            this.Browser.Size = new System.Drawing.Size(351, 198);
            this.Browser.TabIndex = 0;
            this.Browser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.Browser_Navigating);
            // 
            // WebTimer
            // 
            this.WebTimer.Interval = 30000;
            this.WebTimer.Tick += new System.EventHandler(this.WebTimer_Tick);
            // 
            // btnBack
            // 
            this.btnBack.BackgroundImage = global::RegalHROutgoing.Properties.Resources.IconIndex;
            this.btnBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBack.Location = new System.Drawing.Point(261, 0);
            this.btnBack.Margin = new System.Windows.Forms.Padding(0);
            this.btnBack.Name = "btnBack";
            this.btnBack.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnBack.Size = new System.Drawing.Size(90, 70);
            this.btnBack.TabIndex = 5;
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // BtnClose
            // 
            this.BtnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(255)))), ((int)(((byte)(170)))));
            this.BtnClose.Location = new System.Drawing.Point(0, 0);
            this.BtnClose.Name = "BtnClose";
            this.BtnClose.Size = new System.Drawing.Size(40, 37);
            this.BtnClose.TabIndex = 6;
            this.BtnClose.DoubleClick += new System.EventHandler(this.BtnClose_DoubleClick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 198);
            this.Controls.Add(this.BtnClose);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.Browser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.Text = "外出打卡";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser Browser;
        private System.Windows.Forms.Timer WebTimer;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Panel BtnClose;


    }
}

