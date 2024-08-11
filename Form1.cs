using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MasterZeka
{
    public partial class Form1 : Form
    {
        int i = 0;
        string imageurl = "";
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private Dictionary<int, Image> imageCache = new Dictionary<int, Image>();

        public Form1()
        {
            InitializeComponent();
            _ = LoadImageAsync(); // Baþlatma sýrasýnda resmi yükle
            _ = PreloadNextImages(); // Baþlatma sýrasýnda diðer resimleri önbelleðe al
            label1.Text = i.ToString();

            groupBox1.MouseDown += groupBox1_MouseDown;
            groupBox1.MouseMove += groupBox1_MouseMove;
            groupBox1.MouseUp += groupBox1_MouseUp;
        }

        private async Task LoadImageAsync()
        {
            // Öncelikle, önbellekten kontrol et
            if (imageCache.ContainsKey(i))
            {
                pictureBox1.Image = imageCache[i];
                return;
            }

            imageurl = "https://firebasestorage.googleapis.com/v0/b/masterzekasorubankasi.appspot.com/o/pages%2Fpage_" + i + ".png?alt=media";
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Resmi asenkron olarak indirin
                    var imageBytes = await client.GetByteArrayAsync(imageurl);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        var img = Image.FromStream(ms);
                        pictureBox1.Image = img;
                        imageCache[i] = img; // Önbelleðe al
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim yüklenemedi: " + ex.Message);
                }
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private async Task PreloadNextImages()
        {
            for (int j = 1; j <= 5; j++)
            {
                int nextIndex = (i + j) % 323;
                if (!imageCache.ContainsKey(nextIndex))
                {
                    string nextImageUrl = "https://firebasestorage.googleapis.com/v0/b/masterzekasorubankasi.appspot.com/o/pages%2Fpage_" + nextIndex + ".png?alt=media";
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            var imageBytes = await client.GetByteArrayAsync(nextImageUrl);
                            using (var ms = new System.IO.MemoryStream(imageBytes))
                            {
                                imageCache[nextIndex] = Image.FromStream(ms);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Önbelleðe alma baþarýsýz: " + ex.Message);
                        }
                    }
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (i == 322)
            {
                i = 0;
            }
            else
            {
                i++;
            }
            await LoadImageAsync();
            await PreloadNextImages();
            label1.Text = i.ToString();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (i == 0)
            {
                i = 322;
            }
            else
            {
                i--;
            }
            await LoadImageAsync();
            await PreloadNextImages();
            label1.Text = i.ToString();
        }
        private async void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private async void button4_Click(object sender, EventArgs e)
        {
            int sayfa;
            if (int.TryParse(textBox1.Text, out sayfa))
            {
                if (sayfa > 322) sayfa = 322;
                if (sayfa < 0) sayfa = 0;

                i = sayfa;
                await LoadImageAsync();
                await PreloadNextImages();
                label1.Text = i.ToString();
            }
        }

        private void groupBox1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = groupBox1.Location;
        }

        private void groupBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                groupBox1.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void groupBox1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
    }
}
