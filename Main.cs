using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

namespace Ebay_View_Bot
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        private static List<string> ebayLinks { get; set; } = new List<string>();
        private static List<int> viewCount { get; set; } = new List<int>();
        private static string ebayName { get; set; }
        private static string ebayProductName { get; set; }
        private static int loopCount { get; set; }
        private static int rowCount { get; set; }
        private static int progressBarCount { get; set; }
        private static int progressBarTotal { get; set; }
        private static bool finished { get; set; } = true;
        private static bool nameLookup { get; set; } = true;
        private static bool randomisedDelay { get; set; } = false;
        private static bool customDelay { get; set; } = false;

        private async void GenerateViews()
        {
            progressBarCount = 0;
            finished = false;
            CalculateProgressBar();
            rowCount = 0;

            foreach (string link in ebayLinks)
            {
                loopCount = 0;
                while (loopCount < viewCount[rowCount])
                {
                    CustomDelay();

                    using (HttpClient httpClient = new HttpClient())
                    {
                        var initiateRequest = await httpClient.GetAsync(link);
                        var requestResponse = await initiateRequest.Content.ReadAsStringAsync();
                    }

                    loopCount++;
                    progressBarCount++;

                    this.Invoke((MethodInvoker)delegate
                    {
                        progressBar1.Value = progressBarCount + 1;
                    });
                }

                rowCount++;
            }

            this.Invoke((MethodInvoker)delegate
            {
                MessageBox.Show("Boosting Complete!");
                finished = true;
            });

            GC.Collect();
        }

        public async void GrabProductNameAsync()
        {
            HttpClient httpClient = new HttpClient();

            try
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
                {
                    var grabHTML = await httpClient.GetStringAsync(ebayName);
                    HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(grabHTML);
                    ebayProductName = document.DocumentNode.SelectSingleNode("//*[@id=\"itemTitle\"]/text()").InnerText;

                    //------

                    dataGridView1.Rows.Add(ebayProductName, textBox2.Text);
                    ebayLinks.Add(textBox1.Text);
                    viewCount.Add(Convert.ToInt32(textBox2.Text));
                    textBox1.Clear();
                    textBox2.Text = Convert.ToString(50);
                }
                else
                {
                    MessageBox.Show("Please make sure both text boxes are filled in!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MessageBox.Show("There is an error with your link.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomDelay()
        {
            Random random = new Random();

            if (randomisedDelay)
            {
                int randomisedDelaySeconds = random.Next(1000, 10000);
                Thread.Sleep(randomisedDelaySeconds);
            }
            else if (customDelay)
            {
                int customDelaySeconds = random.Next((int)numericUpDown1.Value * 1000, (int)numericUpDown2.Value * 1000);
                Thread.Sleep(customDelaySeconds);
            }
        }

        private void EmptyList()
        {
            ebayLinks.Clear();
            viewCount.Clear();
            dataGridView1.Rows.Clear();
        }

        public void CalculateProgressBar()
        {
            foreach (int views in viewCount)
            {
                progressBarTotal += views;
            }

            this.Invoke((MethodInvoker)delegate
            {
                progressBar1.Maximum = progressBarTotal + 1;
            });
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!finished)
            {
                MessageBox.Show("Please let the operation finish first.", "Stop D:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                if (ebayLinks.Count == 0 || viewCount.Count == 0)
                {
                    MessageBox.Show("Please add a product to boost!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Thread thread = new Thread(GenerateViews);
                    thread.Start();
                }
            }

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!finished)
            {
                MessageBox.Show("Please let the operation finish first.", "Stop D:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                ebayName = textBox1.Text;

                if (nameLookup)
                {
                    GrabProductNameAsync();
                }
                else
                {
                    // Some people suffer from "Failed to add product" Error. This fixes that.
                    ebayLinks.Add(textBox1.Text);
                    viewCount.Add(Convert.ToInt32(textBox2.Text));
                    dataGridView1.Rows.Add(textBox1.Text, textBox2.Text);
                }
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Empty_Click(object sender, EventArgs e)
        {
            if (!finished)
            {
                MessageBox.Show("Please let the operation finish first.", "Stop D:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                EmptyList();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            nameLookup = !nameLookup;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (finished)
            {
                progressBar1.Value = 0;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value >= 10)
            {
                numericUpDown1.Value = 9;
            }
            else if (numericUpDown1.Value >= numericUpDown2.Value)
            {
                numericUpDown2.Value = numericUpDown2.Value - 1;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value >= 10)
            {
                numericUpDown2.Value = 10;
            }
            else if (numericUpDown1.Value >= numericUpDown2.Value)
            {
                numericUpDown2.Value = numericUpDown2.Value + 1;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                randomisedDelay = true;

                // Verification
                checkBox2.Checked = false;
                customDelay = false;
            }
            else if (checkBox3.Checked == false)
            {
                randomisedDelay = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                customDelay = true;

                // Verification
                checkBox3.Checked = false;
                randomisedDelay = false;
            }
            else if (checkBox2.Checked == false)
            {
                customDelay = false;
            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            nameLookup = !nameLookup;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://hackforums.net/member.php?action=profile&uid=3790579");
        }
    }
}