using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace File_Server {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        DirectoryInfo folder; //directory with all the files i'm sending
        Server server;
        private void Form1_Load(object sender, EventArgs e) {
            label1.Text = "selected folder: please select a path";
            listBox1.Items.Add("history");
        }
        public void getFileNum(DirectoryInfo dir, ref int i) {
            try {
                FileInfo[] files = dir.GetFiles();
                i += files.Length;
                DirectoryInfo[] dirs = dir.GetDirectories();
                if (dirs.Length > 0) {
                    foreach (DirectoryInfo subDir in dirs) {
                        getFileNum(subDir, ref i);
                    }
                }
            }
            catch(Exception e) {
                listBox1.Items.Add("Unauthorized acces. If you want to scan such files, admin priviliges are required.");
            }
        }
        public void sendFiles(DirectoryInfo dir) {
            
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();
            string path = dir.FullName + "/";
            foreach (FileInfo file in files) {
                server.sendFile(path + file.Name, folder.FullName.Replace(folder.Name,""));
                listBox1.Items.Add("Sent file: " + file.Name);
            }
            foreach (DirectoryInfo subDir in dirs) {
                sendFiles(subDir);
            }

        }

        private void button1_Click(object sender, EventArgs e) {
            folderBrowserDialog1.ShowDialog();
            if (folderBrowserDialog1.SelectedPath != "") {
                label1.Text = folderBrowserDialog1.SelectedPath;
                listBox1.Items.Add("New path selected - " + folderBrowserDialog1.SelectedPath);
                folder = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            if (folder != null) {
                string ip = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + "." + textBox4.Text;
                listBox1.Items.Add("Creating a test server on the machine on port " + 8001 + ". Host ip: " + ip);
                server = new Server(ip, 8001);
                listBox1.Items.Add("Server running.");
                int i = 0;
                listBox1.Items.Add("Fetching files...");
                getFileNum(folder, ref i);
                listBox1.Items.Add("Files scanned. Current number: " + i + " files.");

                //send number of files to be transmitted, do not go over 10k
                listBox1.Items.Add("Getting you files ready... please standby.");
                server.send(i.ToString());
                listBox1.Items.Add("Your files are ready, beggining the transmitting process.");

                //actual method for sending files
                sendFiles(folder);
                listBox1.Items.Add(".");
                listBox1.Items.Add(".");
                listBox1.Items.Add("Files sent!");

            }
            else {
                MessageBox.Show("No directory path selected.", "Error_null_path");
            }
        }
    }
}
