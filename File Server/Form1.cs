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
                server.sendFile(path + file.Name, folder.FullName.Replace(folder.Name, ""));
                //listBox1.Items.Add("Sent file: " + file.Name);
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
            push();
        }

        private void print(string msg) {
            listBox1.Items.Add(msg);
        }

        public async Task push() {
            
            if (folder != null) {
                string ip = textBox1.Text + "." + textBox2.Text + "." + textBox3.Text + "." + textBox4.Text;
                print("Creating a test server on the machine on port " + 8001 + ". Host ip: " + ip);
                server = new Server(ip, 8001);
                print("Server running.");
                int i = 0;
                print("Fetching files...");
                getFileNum(folder, ref i);
                print("Files scanned. Current number: " + i + " files.");

                //send number of files to be transmitted, do not go over 10k
                print("Getting your files ready... please standby.");
                await Task.Run(() =>
                {
                    server.send(i.ToString());
                });
                print("Your files are ready, beggining the transmitting process...");
                print("Sending files, please be patient, this may take a few minutes");
                timer1.Start();

                //actual method for sending files
                await Task.Run(() =>
                {
                    sendFiles(folder);
                });

                timer1.Stop();
                listBox1.Items.RemoveAt(listBox1.Items.Count-1);
                print(".");
                print(".");
                print("Files sent!");
            }
            else {
                MessageBox.Show("No directory path selected.", "Error_null_path");
            }
        }
        int x = 0;
        private void timer1_Tick(object sender, EventArgs e) {
            listBox1.Items.RemoveAt(listBox1.Items.Count-1);
            x++;
            switch (x) {
                case 1: {
                        listBox1.Items.Add("Sending files.");
                        break;
                    }
                case 2: {
                        listBox1.Items.Add("Sending files..");
                        break;
                    }
                case 3: {
                        listBox1.Items.Add("Sending files...");
                        break;
                    }
                case 4: {
                        listBox1.Items.Add("Sending files....");
                        break;
                    }
                case 5: {
                        listBox1.Items.Add("Sending files.....");
                        break;
                    }
                case 6: {
                        listBox1.Items.Add("Sending files......");
                        break;
                    }
                case 7: {
                        listBox1.Items.Add("Sending files.......");
                        break;
                    }
                case 8: {
                        listBox1.Items.Add("Sending files......");
                        break;
                    }
                case 9: {
                        listBox1.Items.Add("Sending files.....");
                        break;
                    }
                case 10: {
                        listBox1.Items.Add("Sending files....");
                        break;
                    }
                case 11: {
                        listBox1.Items.Add("Sending files...");
                        break;
                    }
                case 12: {
                        listBox1.Items.Add("Sending files..");
                        x = 0;
                        break;
                    }
                default: {
                        listBox1.Items.Add("Error");
                        break;
                    }
            }
        }
    }
}
