using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using HtmlAgilityPack;

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

        public static List<string> ebayLinks { get; set; } = new List<string>();
        public static List<int> viewCount { get; set; } = new List<int>();
        public static string ebayName { get; set; }
        public static string ebayProductName { get; set; }
        public static int loopCount { get; set; }
        public static int rowCount { get; set; }
        public static int progressBarCount { get; set; }
        public static int progressBarTotal { get; set; }
        public static bool finished { get; set; } = true;

        private async void GenerateViews()
        {
            progressBarCount = 0;
            finished = false;
            CalculateProgressBar();

            foreach (string link in ebayLinks)
            {
                loopCount = 0;
                rowCount = 0;
                while (loopCount < viewCount[rowCount])
                {
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var initiateRequest = await httpClient.GetAsync(ebayLinks[rowCount]);
                        var requestResponse = initiateRequest.Content.ReadAsStringAsync();
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

        public static HttpClient httpClient = new HttpClient();

        public async void GrabProductNameAsync()
        {
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
                    textBox2.Text = Convert.ToString(100);
                }
                else
                {
                    MessageBox.Show("Please make sure both text boxes are filled in!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                dataGridView1.Rows.Add("Error. Check your internet connection.", textBox2.Text);
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
                GrabProductNameAsync();
            }
        }

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void EmptyList()
        {
            ebayLinks.Clear();
            viewCount.Clear();
            dataGridView1.Rows.Clear();
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
    }
}