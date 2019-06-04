using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toaster
{
    public partial class Form1 : Form
    {
        public string lotNumber;
        public Form1()
        {
            InitializeComponent();
        }

        private void ProcessLotButton_CheckedChanged(object sender, EventArgs e)
        {
            LotNumberTextBox.Enabled = true;
        }
        private void ProcessAssetButton_CheckedChanged(object sender, EventArgs e)
        {
            LotNumberTextBox.Enabled = false;
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (LotNumberTextBox.Enabled == true)
            {
                lotNumber = LotNumberTextBox.Text;
            }
            else
            {
                lotNumber = "None";
            }
            this.Hide();
            Form2 f2 = new Form2(lotNumber);
            f2.ShowDialog();
        }
    }
}
