using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;


namespace Toaster
{
    public partial class Form2 : Form
    {
        public int DrivesScanned = 0;
        public DetectedDisk Disk;
        delegate void SetTextCallback(string text);
        delegate void SetColorCallback(Color color);
        public string LotNumberToPass, previousSerialNumber;
        public Boolean shouldRun = true, isRunning = false;
        

        public Form2(string lotNumber)
        {
            InitializeComponent();
            LotLabel.Text = lotNumber;
            LotNumberToPass = lotNumber;
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            DrivesScanned = 0;
            DriveNumberLabel.Text = DrivesScanned.ToString();
            StatusLabel.Text = "Scanning...";
            StatusLabel.Refresh();
            Thread thread = new Thread(new ThreadStart(this.Thread_ContinuousChecker))
            {
                IsBackground = true,
                Name = "Toaster Scan"
            };
            thread.Start();

        }
        private void FinishLot_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
        }

        public void Thread_ContinuousChecker()
        {
            isRunning = true;
            while (shouldRun)
            {
                DetectConnections();
                Thread.Sleep(1000);
            }
            isRunning = false;
        }
        
        private void DetectConnections()
        {
            List<DetectedDisk> detectedDisks = null;
            bool hadError = false;
            string errorMessage = null;

            if(0 != (new DiskDetector()).Detect(false,out hadError, out errorMessage).Count) {
                try
                {
                    detectedDisks = (new DiskDetector()).Detect(true,out hadError, out errorMessage);
                }
                catch (Exception ex)
                {
                    hadError = true;
                    errorMessage = ex.Message;
                    if(detectedDisks == null)
                    {
                        detectedDisks = new List<DetectedDisk>();
                    }
                }
            }
            else
            {
                string txt = "Waiting...";
                SetText(txt.ToString());
                SetColor(Color.Black);
                detectedDisks = new List<DetectedDisk>();
            }
            // Tell them about any errors during the detection.
            if (hadError)
            {

                MessageBox.Show("An unexpected error occurred while detecting connections." + Environment.NewLine +
                     Environment.NewLine +
                     ((errorMessage == null) ? "" : errorMessage));
            }

            foreach (DetectedDisk detectedDisk in detectedDisks)
            {
                Disk = detectedDisk;
                string scanningTxt = "Reading Disk...";
                SetText(scanningTxt.ToString());
                if (Disk != null)
                {
                    if (previousSerialNumber != Disk.SerialNumber)
                    {
                        SendData(Disk);

                        previousSerialNumber = Disk.SerialNumber;
                    }
                }
            }
        }

        private void SendData(DetectedDisk DiskToSend)
        {
            string serial = "UNK";
            string make = "UNK";
            string model = "UNK";
            string capacity = "UNK";
            string spinRate = "UNK";
            if (false == String.IsNullOrEmpty(DiskToSend.SmartctlOutput))
            {
                string[] stringSeparators = new string[] { "\r\n" }; 
                string smartoutp = DiskToSend.SmartctlOutput;
                string[] lines = smartoutp.Split(stringSeparators,StringSplitOptions.None);
                foreach (string part in lines)
                {
                    if (part.StartsWith("Model Family:"))
                    {
                        make = part.Substring(("Model Family:").Length).Trim(' ');
                    }
                    if (part.StartsWith("Device Model:"))
                    {
                        model = part.Substring(("Device Model:").Length).Trim(' ');
                    }
                    if (part.StartsWith("User Capacity:"))
                    {
                        capacity = part.Substring(("User Capacity:").Length).Trim(' ');
                    }
                    if (part.StartsWith("Rotation Rate:"))
                    {
                        spinRate = part.Substring(("Rotation Rate:").Length).Trim(' ');
                    }
                    if (part.StartsWith("Serial Number:"))
                    {
                        serial = part.Substring(("Serial Number:").Length).Trim(' ');
                    }
                }
            }
            else
            {
                serial = DiskToSend.SerialNumber;
                make = DiskToSend.Manufacturer;
                model = DiskToSend.Model;
                capacity = DiskToSend.Size.ToString();
            }
                
            Boolean toWipe = true;
            string txt = "Uploading...";
            SetText(txt.ToString());
            Thread.Sleep(1000);
            //TODO: Do work here to send the drive info to the server
             
            PortChat pc = new PortChat();
            bool Portname = pc.FindPort();
            if (Portname)
            {
                string driveInfoToSend = ("Disk: " + CsvEncode(serial) + ", " + CsvEncode(make) + ", " + CsvEncode(model) + ", " + CsvEncode(capacity) + ", " + CsvEncode(spinRate));
                Console.WriteLine(driveInfoToSend);
                pc.SendMessage(driveInfoToSend);
            }
            txt = "Uploaded";
            SetText(txt.ToString());
            DrivesScanned = DrivesScanned + 1;
            //TODO: Do work here to get response on wheather or not to wipe or trash the disk, the set the boolean variable, then set the text color based on that response.
            SetDrivesScannedText(DrivesScanned.ToString());
            if (toWipe == true)
            {
                SetColor(Color.Green); 
            }
            else
            {
                SetColor(Color.Red);
            }  
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            shouldRun = false;
            while (isRunning)
            {
                Thread.Sleep(1000);
            }
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.StatusLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.StatusLabel.Text = text;
            }
        }
        private void SetDrivesScannedText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.StatusLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetDrivesScannedText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.DriveNumberLabel.Text = text;
            }
        }
        private void SetColor(Color color)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.StatusLabel.InvokeRequired)
            {
                SetColorCallback d = new SetColorCallback(SetColor);
                this.Invoke(d, new object[] { color });
            }
            else
            {
                this.StatusLabel.ForeColor = color;
            }
        }
        public static string CsvEncode(string a)
        {
            return "'" + a.Replace("'", "''") + "'";

        }


    }
}
