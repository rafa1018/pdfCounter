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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PDF_Page_Counter
{
    public partial class FormOptimized : Form
    {
        private static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        public delegate void AddItemList(ListViewItem item);
        public int numberOfPagesTotal = 0;
        public int numberOfFiles = 0;
        static readonly object Identity = new object();
        int countTotalFolios = 0;
        int countTotalExpedientes = 0;

        public FormOptimized()
        {
            InitializeComponent();
            listView1.ColumnClick += new ColumnClickEventHandler(lvItem_ColumnClick);
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
            btn_run.Text = "Listando...";
            btn_run.Enabled = false;
            string pathTxt = txt_path.Text;
            Task taskA = new Task(() => ProcessFiles(pathTxt));
            taskA.Start();
            
        }

        private void ProcessFiles(string dirBase)
        {
            var di = new DirectoryInfo(dirBase);
            try
            {
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

                listView1.Sort();

                // Set the ListViewItemSorter property to a new ListViewItemComparer object.
                listView1.ListViewItemSorter = new ListViewItemDateTimeComparer(5, SortOrder.Ascending);

            }
            catch
            {
                throw;
            }     
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
                    countTotalFolios += numberOfPages;
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

            // Se agrega numeroExpediente
            try
            {
                string[] parts = dirName.Split('\\');

                string expedienteNumero = parts[5].Split('\\')[0];
                itm.SubItems.Add(Convert.ToString(expedienteNumero));
            }
            catch
            {
                throw;
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
            btn_run.Text = "Buscar directorio...";
            btn_run.Enabled = true;
        }

        private void linkLabel1_LinkClicked(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", ValidateNames = true })

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(Microsoft.Office.Interop.Excel.XlSheetType.xlWorksheet);

                    Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)app.ActiveSheet;

                    app.Visible = false;
                    ws.Cells[1, 1] = "Archivo";
                    ws.Cells[1, 2] = "FileSize";
                    ws.Cells[1, 3] = "FileStatus";
                    ws.Cells[1, 4] = "PagesCount";
                    ws.Cells[1, 5] = "FilePath";
                    ws.Cells[1, 6] = "Numero Expediente";
                    ws.Cells[1, 7] = "Total";
                    ws.Cells[1, 8] = "Total Folios";
                    ws.Cells[1, 9] = "Total Expedientes";
                    int i = 2;
                    string oldNumeroExpediente = null;

                    foreach (ListViewItem item in listView1.Items)
                    {
                        ws.Cells[i, 1] = item.SubItems[0].Text;
                        ws.Cells[i, 2] = item.SubItems[1].Text;
                        ws.Cells[i, 3] = item.SubItems[2].Text;
                        ws.Cells[i, 4] = item.SubItems[3].Text;
                        ws.Cells[i, 5] = item.SubItems[4].Text;
                        ws.Cells[i, 6].NumberFormat = "@";
                        ws.Cells[i, 6] = item.SubItems[5].Text;

                        int total = 0;
                        bool flag = false;
                        string numeroExpediente = item.SubItems[5].Text;

                        if (oldNumeroExpediente != numeroExpediente)
                        {
                            flag = true;
                            countTotalExpedientes++;
                        }

                        var query = listView1.Items.Cast<ListViewItem>().Where(items => items.SubItems[5].Text == numeroExpediente);
                        foreach (var data in query)
                        {
                            total += Convert.ToInt32(data.SubItems[3].Text);

                        }
                        oldNumeroExpediente = numeroExpediente;
                        if (flag)
                        {
                            ws.Cells[i, 7] = total;
                        }
                        i++;
                    }
                    ws.Cells[2, 8] = countTotalFolios;
                    ws.Cells[2, 9] = countTotalExpedientes;


                    wb.SaveAs(sfd.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing, true, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlLocalSessionChanges, Type.Missing, Type.Missing);
                    app.Quit();
                    MessageBox.Show("Tus datos han sido exportados con éxito.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);


                }
        }

        private int _sortColumnIndex = -1;
        void lvItem_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != _sortColumnIndex)
            {
                // Set the sort column to the new column.
                _sortColumnIndex = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView1.Sort();

            // Set the ListViewItemSorter property to a new ListViewItemComparer object.
            if (e.Column == listView1.Columns[5].Index)
                listView1.ListViewItemSorter = new ListViewItemDateTimeComparer(e.Column, listView1.Sorting);
            else
                listView1.ListViewItemSorter = new ListViewItemStringComparer(e.Column, listView1.Sorting);
        }

    }
}
