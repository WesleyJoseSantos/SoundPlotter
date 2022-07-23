using System;
using System.Windows.Forms;

namespace SoundPlotter
{
    public partial class MainForm : Form
    { 
        Plotter plotter;

        public MainForm()
        {
            InitializeComponent();
            plotter = new Plotter();
            plotter.ValueAvailable += Plotter_ValueAvailable;
            toolStripProgressBar.Minimum = 0;
            toolStripProgressBar.Maximum = 32768;
        }

        private void Plotter_ValueAvailable(object sender, EventArgs e)
        {
            var obj = (Plotter)sender;
            this.Invoke((MethodInvoker)delegate {
                toolStripProgressBar.Value = obj.Sample;
                textBoxPlotter.AppendText(obj.Value.ToString());
                textBoxPlotter.AppendText(Environment.NewLine);
            });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            propertyGrid.SelectedObject = plotter.Data;
        }

        private void BtStart_Click(object sender, EventArgs e)
        {
            plotter.Start();
            toolStripStatusLabel.Text = "START";
        }

        private void BtStop_Click(object sender, EventArgs e)
        {
            plotter.Stop();
            toolStripStatusLabel.Text = "STOP";
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plotter.SaveData();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Save Sound Plotter Settings",
                Filter = "Sound Plotter JSON File | *.json",
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                plotter.SaveData(saveFileDialog.FileName);
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Open Sound Plotter Settings",
                Filter = "Sound Plotter JSON File | *.json",
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                plotter.LoadData(openFileDialog.FileName);
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Export Sound Plotter Buffer",
                Filter = "Sound Plotter Buffer JSON File | *.json",
            };
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                plotter.Export(saveFileDialog.FileName);
            }
        }

        private void importPlotterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Sound Plotter Buffer JSON File | *.json",
                Title = "Import Sound Plotter Buffer",
            };
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                plotter.Import(openFileDialog.FileName);
                textBoxPlotter.Text = "";
                foreach (var data in plotter.Data.Buffer)
                {
                    textBoxPlotter.AppendText(data.ToString());
                    textBoxPlotter.AppendText(Environment.NewLine);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            plotter.SaveData();
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            plotter.Data.Buffer.Clear();
            textBoxPlotter.Text = "";
        }
    }
}
