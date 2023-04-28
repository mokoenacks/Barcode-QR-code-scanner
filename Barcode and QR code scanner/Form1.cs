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
using ZXing.Presentation;
using BitMiracle.LibTiff.Classic;


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
        {//S
            textBox_display.Clear();
            if (filter.Count > 0)
            {
                captureDevice = new VideoCaptureDevice(filter[comboBox_cam.SelectedIndex].MonikerString);
                captureDevice.NewFrame += CaptureDevice_NewFrame;
                captureDevice.Start();
                timer1.Start();
            }
else
            {
                MessageBox.Show("No video devices found.");
            }

        }

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();// QR code

        }

        private void Form1_Load(object sender, EventArgs e)
        {//structure preparing the picture box

            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filter)
            {
                comboBox_cam.Items.Add(filterInfo.Name);
            }
            comboBox_cam.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {//closing the form ,prevants from accidentally closing the form as the select button is close to the X button
            DialogResult msg = MessageBox.Show("Are you sure you want to close window", "Warning !", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (msg == DialogResult.Yes)
            {
                try
                {
                    if (captureDevice != null && captureDevice.IsRunning)
                    {
                        captureDevice.Stop();
                    }
                }
                finally
                {
                    Application.Exit();
                }
            }
            else
            {
                e.Cancel = true;

            }
        }

        public static bool IsValidUrl(string url)
        {
  //checking whether its a URL link
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
 //video scanner ,using the webcame or connected camera to scan QR code
            if (pictureBox.Image != null)
            {
   //calling packages
                ZXing.BarcodeReader barcode = new ZXing.BarcodeReader();
                Result result = barcode.Decode((Bitmap)pictureBox.Image);
                if (result != null) 
                {
                    textBox_display.Text = result.ToString();
                    if (IsValidUrl(textBox_display.Text) == true)
                    {
                        DialogResult result1 = MessageBox.Show("Open Web Browser?", "Permission?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result1 == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(textBox_display.Text);//open link if there is one

                        }
                        else if (result1 == DialogResult.No)
                        {
                            result1 = DialogResult.Cancel;//prevants from closing the entire application only the messageBox
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

        private void button1_Click_1(object sender, EventArgs e)
        {
             textBox_display.Clear();
//QR pictures ,open files for you to select a picture with a QR code
            if (captureDevice!=null && captureDevice.IsRunning )
            {
                captureDevice.Stop();
                timer1.Stop();  
            }
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.png;*.jpg;*.bmp)|*.png;*.jpg;*.bmp|All files (*.*)|*.*";
            dialog.Title = "Select QR code image to scan";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //going through the files to open QR code
               
                    var reader = new ZXing.BarcodeReader();
                    var imageFile = Image.FromFile(dialog.FileName) as Bitmap;
                    pictureBox.Image = imageFile;
                    var results = reader.Decode(imageFile);
                    if (results != null)
                    {
                            textBox_display.Text = results.ToString();
                            if (IsValidUrl(textBox_display.Text) == true)
                            {
                            //basically same as webcame section
                                 DialogResult result1 = MessageBox.Show("Open Web Browser?", "Permission?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                 if (result1 == DialogResult.Yes)
                                 {
                                    System.Diagnostics.Process.Start(textBox_display.Text);//open link if there is one
                                    
                                 }
                                 else if (result1 == DialogResult.No)
                                 {
                                    result1 = DialogResult.Cancel;
                                 }
                            }
                            else
                            {
                                MessageBox.Show("There is no link");
                            }
                            
                    }
                    else
                    {
                         MessageBox.Show("QR Image not found", "Not QR image", MessageBoxButtons.OK);    
              
                    }
            }
           

        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}
