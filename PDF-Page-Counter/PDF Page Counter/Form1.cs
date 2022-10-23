using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Excel;
using static System.Int32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Forms.Application;
using ListView = System.Windows.Forms.ListView;

namespace PDF_Page_Counter
{
    public partial class MainForm : Form
    {
        private static readonly string[] SizeSuffixes = {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};
        private readonly BackgroundWorker _bgw;
        int countTotalFolios = 0;
        int countTotalExpedientes = 0;

        public MainForm()
        {
            Application.VisualStyleState = VisualStyleState.NonClientAreaEnabled;
            InitializeComponent();
            _bgw = new BackgroundWorker();
            _bgw.DoWork += bgw_DoWork;
            _bgw.RunWorkerCompleted += bgw_RunWorkerCompleted;
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var s = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
                _bgw.RunWorkerAsync(s);
            }
            while (_bgw.IsBusy)
            {
                Form overlay = new WorkingOverlay();
                overlay.StartPosition = FormStartPosition.CenterParent;
                overlay.Size = Size;
                overlay.ShowDialog(this);
                Application.DoEvents();
            }
        }

        private void AddFileToListview(string fullFilePath)
        {
            ListViewGroup numeroExpediente = new ListViewGroup("Numero Expediente", HorizontalAlignment.Left);
            Cursor.Current = Cursors.WaitCursor;
            if (!File.Exists(fullFilePath))
                return;
            var fileName = Path.GetFileName(fullFilePath);
            var dirName = Path.GetDirectoryName(fullFilePath);
         
            if (dirName != null && dirName.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)))
                dirName = dirName.Substring(0, dirName.Length - 1);
            var itm = listView1.Items.Add(fileName);
            if (fileName != null)
            {
                // ReSharper disable once UnusedVariable
                var fileInfo = new FileInfo(fileName);
            }
            var length = new FileInfo(fullFilePath).Length;

            //size column
            itm.SubItems.Add(SizeSuffix(length));

            //catch file problems
            try
            {
                var pdfReader = new PdfReader(fullFilePath);
                var numberOfPages = pdfReader.NumberOfPages;
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
          
          
            //calculate items count with linq
            var countItems = listView1.Items.Cast<ListViewItem>().Count();
            toolStripStatusLabel3.Text = countItems.ToString();

            //calculate total pages count with linq
            var countTotalPages = listView1.Items.Cast<ListViewItem>().Sum(item => Parse(item.SubItems[3].Text));
            toolStripStatusLabel4.Text = countTotalPages.ToString();
            countTotalFolios = countTotalPages;
            Cursor.Current = Cursors.Default;
            ListViewGroup expediente = new ListViewGroup("Expediente Numero", HorizontalAlignment.Left);
            
        }

        private static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            try
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
            catch 
            {
      throw;
            }
            
        }

        private void toolStripStatusLabel5_Click(object sender, EventArgs e)
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
                        
                        if(oldNumeroExpediente != numeroExpediente)
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

        // Clears listview items sets counters to zero
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            toolStripStatusLabel3.Text = @"0";
            toolStripStatusLabel4.Text = @"0";
        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                Invoke(new Action<object>(args =>
                {
                    var handles = (string[])e.Argument;
                    foreach (var s in handles)
                        if (File.Exists(s))
                        {
                            if (string.Compare(Path.GetExtension(s), ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
                                AddFileToListview(s);
                        }
                        else if (Directory.Exists(s))
                        {
                            var di = new DirectoryInfo(s);
                            var files = di.GetFiles("*.pdf",
                                checkBox1.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                            foreach (var file in files)
                                AddFileToListview(file.FullName);
                        }
                }), e.Argument);
            }
            catch
            {
                throw;
            }


           
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ActiveForm?.Hide();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            var listView = sender as ListView;
            if (e.Button == MouseButtons.Right)
            {
                var item = listView?.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    item.Selected = true;
                    contextMenuStrip1.Show(listView, e.Location);
                }
            }
        }

        private void openFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(listView1.SelectedItems[0].SubItems[4].Text);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = (Parse(toolStripStatusLabel3.Text) - 1).ToString();
            toolStripStatusLabel4.Text = (Parse(toolStripStatusLabel4.Text) - Parse(listView1.SelectedItems[0].SubItems[3].Text)).ToString();
            listView1.SelectedItems[0].Remove();
        }
    }
}