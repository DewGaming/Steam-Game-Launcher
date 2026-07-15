using System.Collections;
using System.Diagnostics;
using System.Net;

namespace PhysicalSteamGameLauncher
{
    public partial class SteamGameLauncher : Form
    {
        private string path;

        public SteamGameLauncher()
        {
            InitializeComponent();
        }

        private void SteamGameLaucnher_Load(object sender, EventArgs e)
        {
            int windowWidth = 300;
            int windowHeight = 450;
            PictureBox pictureBox;
            string imagesPath = path + "\\Images";
            List<string> gamesList = File.ReadAllLines($"{path}\\games.txt").ToList();
            for (int i = gamesList.Count - 1; i >= 0; i--)
            {
                if (gamesList[i].Length == 0)
                {
                    gamesList.RemoveAt(i);
                }
            }

            if (!Directory.Exists(imagesPath) && IsDirectoryWritable(path))
            {
                Directory.CreateDirectory(imagesPath);
            }

            foreach (var game in gamesList)
            {
                pictureBox = new PictureBox();
                if (IsDirectoryWritable(path))
                {
                    if (!File.Exists($"{imagesPath}\\{game}.jpg"))
                    {
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(new Uri($"https://steamcdn-a.akamaihd.net/steam/apps/{game}/library_600x900_2x.jpg"), $"{imagesPath}\\{game}.jpg");
                            };
                        }
                        catch
                        {
                            MessageBox.Show($"An image for the game with the app Id of {game} was not able to be downloaded. Please download the 600x900 separately and add it alongside the other images within the Images folder.", "Game ID Graphic Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }

                pictureBox.ImageLocation = $"{imagesPath}\\{game}.jpg";
                pictureBox.Location = new Point(gamesList.IndexOf(game) * windowWidth, 0);
                pictureBox.Name = $"pictureBox{game}";
                pictureBox.Size = new Size(windowWidth, windowHeight);
                pictureBox.MouseEnter += PictureBox_MouseEnter;
                pictureBox.MouseLeave += PictureBox_MouseLeave;
                pictureBox.Click += PictureBox_Click;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Cursor = Cursors.Hand;
                this.Controls.Add(pictureBox);
            }

            if (gamesList.Count > 4)
            {
                windowWidth = 1350;
                windowHeight = 500;
            }
            else
            {
                windowWidth = 300 * gamesList.Count;
            }

            ClientSize = new Size(windowWidth, windowHeight);
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox)sender;
            Image image = picture.Image;
            if (image != null)
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    Pen pen = new Pen(Color.FromArgb(100, 0, 0, 0), 6000);
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
            string steamUrl = $"steam://rungameid/{gameId}";
            Process.Start(new ProcessStartInfo
            {
                FileName = steamUrl,
                UseShellExecute = true
            });

            this.Close();
        }

        private bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                {
                    throw;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
