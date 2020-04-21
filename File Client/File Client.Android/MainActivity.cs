using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

namespace File_Client.Droid
{
    [Activity(Label = "File_Client", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {
        protected override void OnCreate(Bundle savedInstanceState) {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());



            DirectoryInfo check = new DirectoryInfo("/storage/emulated/0/Download/FileSync");
            if (!check.Exists)
                Directory.CreateDirectory("/storage/emulated/0/Download/FileSync");

            StreamReader sr = new StreamReader("/storage/emulated/0/config.txt");
            string ip = sr.ReadLine();
            int port = 8001;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(ip, port);

            //pull number of files (always 4-digit)
            NetworkStream networkStream = new NetworkStream(client);
            byte[] buffer = new byte[4];
            networkStream.Read(buffer, 0, 4);
            string s = new UTF8Encoding(true).GetString(buffer);
            int number = Convert.ToInt32(s);
            client.Close();

            for (int loop = 0; loop < number; loop++) {
                //pull song name length (always 2-digit)
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                networkStream = new NetworkStream(client);
                buffer = new byte[2];
                networkStream.Read(buffer, 0, 2);
                s = new UTF8Encoding(true).GetString(buffer);
                int length = Convert.ToInt32(s);
                client.Close();

                //pull song name
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                networkStream = new NetworkStream(client);
                buffer = new byte[length];
                networkStream.Read(buffer, 0, length);
                s = new UTF8Encoding(true).GetString(buffer);
                //Console.WriteLine("Fetching files... next file:" + s);
                client.Close();

                //pull the song
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                networkStream = new NetworkStream(client);
                createDirs(s, "/storage/emulated/0/Download/FileSync/");
                FileStream fileStream = File.Open("/storage/emulated/0/Download/FileSync/" + s, FileMode.OpenOrCreate);
                networkStream.CopyTo(fileStream);
                fileStream.Close();
                client.Close();
            }
            //Console.WriteLine("\nFile(s) received.");
            this.Dispose();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public void createDirs(string path, string root) {
            string temp = firstNext(path, '/');

            if (temp.EndsWith("/")) {
                DirectoryInfo tempDir = new DirectoryInfo(root + temp.Replace("/", ""));
                if (!tempDir.Exists)
                    Directory.CreateDirectory(root + temp.Replace("/", ""));

                root = root + temp;
                createDirs(path.Replace(temp, ""), root);
            }
        }
        public string firstNext(string path, char token) {
            string temp = "";
            int tempI = 0;
            while (!temp.EndsWith(token.ToString())) {
                temp += path.Substring(tempI, 1);
                tempI++;
                if (path.Substring(tempI, 1) == ".") {
                    return "";
                }
            }
            return temp;
        }

    }
}