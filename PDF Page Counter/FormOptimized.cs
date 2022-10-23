using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDF_Page_Counter
{
    public partial class FormOptimized : Form
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public delegate void AddItemList(ListViewItem item);
        public int numberOfPagesTotal = 0;
        public int numberOfFiles = 0;
        static readonly object Identity = new object();

        public FormOptimized()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    txt_path.Text = fbd.SelectedPath;
                }
            }

            numberOfPagesTotal = 0;
            numberOfFiles = 0;
            btn_run.Text = "Running...";
            btn_run.Enabled = false;
            string pathTxt = txt_path.Text;
            Task taskA = new Task(() => ProcessFiles(pathTxt));
            taskA.Start();
            
        }

        private void ProcessFiles(string dirBase)
        {
            var di = new DirectoryInfo(dirBase);
            var files = di.GetFiles("*.pdf", checkBox1.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            numberOfFiles = files.Count();

            var listitems = new List<ListViewItem>();
            var result = Parallel.ForEach(files, file =>
            {
                Console.WriteLine("Sequential iteration on item '{0}' running on thread {1}.", file,
                Thread.CurrentThread.ManagedThreadId);
                if (File.Exists(file.FullName))
                {
                    var item = ProcessFile(file.FullName);
                    listitems.Add(item);
                }  
            });

            while (!result.IsCompleted)
            {
                Thread.Sleep(1000);
            }
            UpdateValues(numberOfPagesTotal, numberOfFiles);
            UpdateListItems(listitems);

                
        }
        private void UpdateValues(int pages, int files)
        {

            if (lbl_pages.InvokeRequired)
            {
                lbl_pages.BeginInvoke(new MethodInvoker(() => lbl_pages.Text = pages.ToString()));

            }
            else
                lbl_pages.Text = pages.ToString();

            if (lbl_files.InvokeRequired)
            {
                lbl_files.BeginInvoke(new MethodInvoker(() => lbl_files.Text = files.ToString()));

            }
            else
                lbl_files.Text = files.ToString();

        }

        private void UpdateListItems(List<ListViewItem> listitems)
        {
            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(new MethodInvoker(() => listView1.Items.AddRange(listitems.ToArray()))); 

            }
            else
                listView1.Items.AddRange(listitems.ToArray());
        }

        private ListViewItem ProcessFile(string fullFilePath)
        {

            var fileName = Path.GetFileName(fullFilePath);
            var dirName = Path.GetDirectoryName(fullFilePath);
            if (dirName != null && dirName.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)))
                dirName = dirName.Substring(0, dirName.Length - 1);
            var itm = new ListViewItem(fileName);
            var length = new FileInfo(fullFilePath).Length;

            //size column
            itm.SubItems.Add(SizeSuffix(length));

            //catch file problems
            try
            {
                var pdfReader = new PdfReader(fullFilePath);
                var numberOfPages = pdfReader.NumberOfPages;
                lock (Identity)
                {
                    numberOfPagesTotal += numberOfPages;
                }
                itm.SubItems.Add("Good");
                itm.SubItems.Add(numberOfPages.ToString());
                itm.SubItems.Add(dirName);
            }
            catch (Exception e)
            {
                itm.SubItems.Add("Corrupted File");
                itm.SubItems.Add("0");
                itm.SubItems.Add(dirName);
                itm.SubItems.Add(e.Message);
            }
            return itm;
        }

        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (value < 0) return "-" + SizeSuffix(-value);
            if (value == 0) return "0.0 bytes";

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            var mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            // ReSharper disable once FormatStringProblem
            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        private void FormOptimized_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            lbl_files.Text = @"0";
            lbl_pages.Text = @"0";
        }
    }
}
