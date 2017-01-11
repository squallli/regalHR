using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
namespace RegalHROutgoing
{
    public partial class Main : Form
    {
        Uri DefaultUrl;
        int CountDown = 30000;
        public Main()
        {
            InitializeComponent();
          

        }

        private void Main_Load(object sender, EventArgs e)
        {
            btnBack.Location = new Point(this.Width - btnBack.Width, 0);


            BtnClose.Location = new Point(0, this.Height - BtnClose.Height);

            string xmlPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Config.xml";
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlPath);
            XmlNode node = doc.SelectSingleNode("/settings/setup/Url");
            DefaultUrl = new Uri(node.InnerText.ToString());




            node = doc.SelectSingleNode("/settings/setup/CountDown");
            CountDown = int.Parse(node.InnerText.ToString());
            WebTimer.Interval = CountDown;


            //預設首頁
            Browser.Url = DefaultUrl;
            
        }



        private void WebTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Browser.Url = DefaultUrl;
                WebTimer.Stop();
                Browser.Focus();
            }
            catch
            {
                //不做任何變更

            }
 
        }

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            try
            {
                WebTimer.Stop();

                if (Browser.Url == DefaultUrl)
                {
                    WebTimer.Start();

                }
            }
            catch
            {

            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {

                Browser.Url = DefaultUrl;
                Browser.Focus();
            }
            catch
            {

            }
        }


        private void BtnClose_DoubleClick(object sender, EventArgs e)
        {

            //首頁才可以關閉
            if (Browser.Url == DefaultUrl)
            {
                this.Close();
            }
        }
    }
}
