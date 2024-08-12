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

        private bool isSelecting = false;
        private Point startPoint;
        private Rectangle selectedRectangle;

        public Form1()
        {
            InitializeComponent();
            _ = LoadImageAsync();
            _ = PreloadNextImages();
            label1.Text = i.ToString();

            groupBox1.MouseDown += groupBox1_MouseDown;
            groupBox1.MouseMove += groupBox1_MouseMove;
            groupBox1.MouseUp += groupBox1_MouseUp;

            pictureBox1.Paint += pictureBox1_Paint;
        }

        private async Task LoadImageAsync()
        {
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
                    var imageBytes = await client.GetByteArrayAsync(imageurl);
                    using (var ms = new System.IO.MemoryStream(imageBytes))
                    {
                        var img = Image.FromStream(ms);
                        pictureBox1.Image = img;
                        imageCache[i] = img;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim yüklenemedi: " + ex.Message);
                }
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private async Task PreloadNextImages()
        {
            for (int j = 1; j <= 3; j++)
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
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                i++;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            await LoadImageAsync();
            await PreloadNextImages();
            label1.Text = i.ToString();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (i == 0)
            {
                i = 322;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                i--;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            await LoadImageAsync();
            await PreloadNextImages();
            label1.Text = i.ToString();
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
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
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
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

        private void button3_Click(object sender, EventArgs e)
        {
            isSelecting = true;
            startPoint = Point.Empty; // Ýlk týklanan noktayý sýfýrla
            selectedRectangle = Rectangle.Empty; // Seçim alanýný sýfýrla

            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseUp += PictureBox1_MouseUp;

            pictureBox1.Cursor = Cursors.Cross; // Fare imlecini çarpý iþareti yap
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                startPoint = e.Location; // Ýlk týklanan noktayý ata
                selectedRectangle = new Rectangle(startPoint, new Size(0, 0)); // Seçimi baþlat
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelecting && !startPoint.IsEmpty)
            {
                // Seçim alanýný dinamik olarak güncelle
                selectedRectangle = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(startPoint.X - e.X),
                    Math.Abs(startPoint.Y - e.Y));

                // Seçimi görselleþtirmek için pictureBox'ý yeniden çiz
                pictureBox1.Invalidate();
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;

                if (selectedRectangle.Width > 0 && selectedRectangle.Height > 0)
                {
                    Bitmap originalImage = new Bitmap(pictureBox1.Image);

                    // PictureBox boyutlarý
                    float pbWidth = pictureBox1.ClientSize.Width;
                    float pbHeight = pictureBox1.ClientSize.Height;

                    // Resim boyutlarý
                    float imgWidth = originalImage.Width;
                    float imgHeight = originalImage.Height;

                    // Resmin PictureBox içindeki boyutlarý
                    float pictureBoxImgWidth = imgWidth * (pbWidth / imgWidth);
                    float pictureBoxImgHeight = imgHeight * (pbHeight / imgHeight);

                    // Ölçekleme oranlarý
                    float scaleX = imgWidth / pictureBoxImgWidth;
                    float scaleY = imgHeight / pictureBoxImgHeight;

                    // Seçilen alanýn PictureBox içindeki konumunu ve boyutunu hesapla
                    RectangleF pictureBoxRectangle = new RectangleF(
                        selectedRectangle.X * scaleX,
                        selectedRectangle.Y * scaleY,
                        selectedRectangle.Width * scaleX,
                        selectedRectangle.Height * scaleY);

                    // Seçilen alanýn orijinal resim üzerindeki konumunu ve boyutunu hesapla
                    Rectangle originalRectangle = new Rectangle(
                        (int)Math.Round(pictureBoxRectangle.X),
                        (int)Math.Round(pictureBoxRectangle.Y),
                        (int)Math.Round(pictureBoxRectangle.Width),
                        (int)Math.Round(pictureBoxRectangle.Height));

                    // Kýrpma alanýný sýnýrla
                    originalRectangle.X = Math.Max(0, originalRectangle.X);
                    originalRectangle.Y = Math.Max(0, originalRectangle.Y);
                    originalRectangle.Width = Math.Min(originalImage.Width - originalRectangle.X, originalRectangle.Width);
                    originalRectangle.Height = Math.Min(originalImage.Height - originalRectangle.Y, originalRectangle.Height);

                    Bitmap croppedImage = new Bitmap(originalRectangle.Width, originalRectangle.Height);
                    using (Graphics g = Graphics.FromImage(croppedImage))
                    {
                        g.DrawImage(originalImage,
                                    new Rectangle(0, 0, croppedImage.Width, croppedImage.Height),
                                    originalRectangle,
                                    GraphicsUnit.Pixel);
                    }

                    // Kýrpýlmýþ resmi göstermek için yeni bir form aç
                    CroppedImageForm croppedImageForm = new CroppedImageForm(croppedImage);
                    croppedImageForm.Show();
                }

                // Fare olaylarýný kaldýr ve imleci varsayýlan duruma döndür
                pictureBox1.MouseDown -= PictureBox1_MouseDown;
                pictureBox1.MouseMove -= PictureBox1_MouseMove;
                pictureBox1.MouseUp -= PictureBox1_MouseUp;

                pictureBox1.Cursor = Cursors.Default; // Fare imlecini varsayýlan yap
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (selectedRectangle != null && selectedRectangle.Width > 0 && selectedRectangle.Height > 0)
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectedRectangle);
                }
            }
        }
    }
}
