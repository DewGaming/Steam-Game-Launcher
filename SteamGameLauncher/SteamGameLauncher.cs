using System.Collections;
using System.Diagnostics;
using System.Net;

namespace PhysicalSteamGameLauncher
{
    public partial class SteamGameLauncher : Form
    {
        private string path = (string)AppContext.BaseDirectory;

        public SteamGameLauncher()
        {
            InitializeComponent();
        }

        private void SteamGameLaucnher_Load(object sender, EventArgs e)
        {
            int windowWidth = 600;
            int windowHeight = 450;
            PictureBox pictureBox;
            List<string> gamesList = File.ReadAllLines($"{path}\\games.txt").ToList();
            for (int i = gamesList.Count - 1; i >= 0; i--)
            {
                if (gamesList[i].Length == 0)
                {
                    gamesList.RemoveAt(i);
                }
            }
            foreach (var game in gamesList)
            {
                pictureBox = new PictureBox();
                if (!File.Exists($"{path}\\{game}.jpg"))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(new Uri($"https://steamcdn-a.akamaihd.net/steam/apps/{game}/library_600x900_2x.jpg"), $"{path}\\{game}.jpg");
                    };
                }

                pictureBox.ImageLocation = $"{path}\\{game}.jpg";
                pictureBox.Location = new Point(gamesList.IndexOf(game) * 300, 0);
                pictureBox.Name = $"pictureBox{game}";
                pictureBox.Size = new Size(300, 450);
                pictureBox.BackColor = Color.Black;
                pictureBox.MouseEnter += PictureBox_MouseEnter;
                pictureBox.MouseLeave += PictureBox_MouseLeave;
                pictureBox.Click += PictureBox_Click;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Cursor = Cursors.Hand;
                this.Controls.Add(pictureBox);
            }

            if (gamesList.Count > 2)
            {
                windowWidth = 300 * gamesList.Count;
            }

            if (gamesList.Count > 4)
            {
                windowWidth = 1350;
                windowHeight = 500;
            }

            ClientSize = new Size(windowWidth, windowHeight);
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            double zoomFactor = 1.05;
            PictureBox picture = (PictureBox)sender;
            Image image = picture.Image;
            if (image != null)
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    Pen pen = new Pen(Color.FromArgb(90, 0, 0, 0), 6000);
                    g.DrawLine(pen, -1, -1, 6000, 9000);
                    g.Save();
                }
            }
            picture.Image = image;
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            picture.ImageLocation = picture.ImageLocation;
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            string gameId = picture.Name.Replace("pictureBox", string.Empty);
            if (File.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\libraryfolders.vdf") && !File.ReadAllText("C:\\Program Files (x86)\\Steam\\steamapps\\libraryfolders.vdf").Contains($"\"{gameId}\""))
            {
                MessageBox.Show("It looks like this game is not installed on your device. Either install the game through Steam or restore the game backup to continue.", "Install Game", MessageBoxButtons.OK, MessageBoxIcon.Exclamation); 
            }
            else
            {
                string steamUrl = $"steam://rungameid/{gameId}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = steamUrl,
                    UseShellExecute = true
                });

                this.Close();
            }
        }
    }
}
