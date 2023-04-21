using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using ZXing;

namespace Barcode_and_QR_code_scanner
{
    public partial class QRscanner : Form
    {
        public QRscanner()
        {
            InitializeComponent();
        }
        FilterInfoCollection filter;
        VideoCaptureDevice captureDevice;
        private void button1_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filter[comboBox_cam.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();// QR code
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filter)
            {
                comboBox_cam.Items.Add(filterInfo.Name);
            }
            comboBox_cam.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
                Application.Exit();
            }

        }
        public static bool IsValidUrl(string url)
        {//checking whether its a URL link
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(pictureBox.Image != null)
            {
                BarcodeReader barcode = new BarcodeReader();
                Result result= barcode.Decode((Bitmap)pictureBox.Image);
                if(result != null)
                {
                    textBox_display.Text = result.ToString();
                    if(IsValidUrl(textBox_display.Text)==true)
                    {
                       DialogResult result1= MessageBox.Show("Open Web Browser?","Permission?",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                        if (result1 == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(textBox_display.Text);//open link if there is one
                            
                        }
                        else if(result1 == DialogResult.No) 
                        {
                            result1 = DialogResult.Cancel;
                        }
                    }
                    else
                    {
                        MessageBox.Show("There is no link");
                    }
                    
                    timer1.Stop();
                    if (captureDevice.IsRunning)
                    {
                        captureDevice.Stop();
                    }
                }
            }
        }
    }
}
