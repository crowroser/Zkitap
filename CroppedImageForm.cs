using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MasterZeka
{
    public partial class CroppedImageForm : Form
    {
        private Image _croppedImage;
        private bool _isDrawing = false;
        private Point _lastPoint;
        private Bitmap _drawingBitmap;
        private Graphics _drawingGraphics;

        private bool _dragging;
        private Point _dragStartPoint;
        private GroupBox toolBox;


        public CroppedImageForm(Image croppedImage)
        {
            InitializeComponent();
            _croppedImage = croppedImage;
        }

        private void CroppedImageForm_Load(object sender, EventArgs e)
        {
            // Formu tam ekran yapma
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            // Drawing Bitmap ve Graphics başlatma
            _drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            _drawingGraphics = Graphics.FromImage(_drawingBitmap);
            _drawingGraphics.Clear(Color.Transparent);

            // PictureBox oluşturup ekleme
            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = _drawingBitmap,
                BackColor = Color.Transparent, // Şeffaf arka plan
                SizeMode = PictureBoxSizeMode.Zoom
            };
            pictureBox.Paint += PictureBox_Paint;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            this.Controls.Add(pictureBox);

            // Arka plan resmi PictureBox'un bir parçası olarak eklenir
            if (_croppedImage != null)
            {
                using (Graphics g = Graphics.FromImage(_drawingBitmap))
                {
                    g.DrawImage(_croppedImage, 0, 0, _drawingBitmap.Width, _drawingBitmap.Height);
                }
            }

            // GroupBox oluşturma
            toolBox = new GroupBox
            {
                Dock = DockStyle.Top,
                Height = 60
            };
            this.Controls.Add(toolBox);

            // Kalem butonu oluşturma
            System.Windows.Forms.Button penButton = new System.Windows.Forms.Button
            {
                Text = "Kalem",
                Dock = DockStyle.Left
            };
            penButton.Click += PenButton_Click;
            toolBox.Controls.Add(penButton);

            // Temizle butonu oluşturma
            System.Windows.Forms.Button clearButton = new System.Windows.Forms.Button
            {
                Text = "Temizle",
                Dock = DockStyle.Left
            };
            clearButton.Click += ClearButton_Click;
            toolBox.Controls.Add(clearButton);

            // Çıkış butonu oluşturma
            System.Windows.Forms.Button exitButton = new System.Windows.Forms.Button
            {
                Text = "Çıkış",
                Dock = DockStyle.Right
            };
            exitButton.Click += ExitButton_Click;
            toolBox.Controls.Add(exitButton);

            // GroupBox taşınabilir yapma
            toolBox.MouseDown += ToolBox_MouseDown;
            toolBox.MouseMove += ToolBox_MouseMove;
        }

        private void ToolBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStartPoint = e.Location;
            }
        }

        private void ToolBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point newLocation = toolBox.PointToScreen(e.Location);
                newLocation.Offset(-_dragStartPoint.X, -_dragStartPoint.Y);
                toolBox.Location = this.PointToClient(newLocation);
            }
        }

        private void PenButton_Click(object sender, EventArgs e)
        {
            _isDrawing = !_isDrawing; // Toggle drawing mode
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearDrawing();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close(); // Formu kapat
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _isDrawing)
            {
                _lastPoint = e.Location;
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing && e.Button == MouseButtons.Left)
            {
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    _drawingGraphics.DrawLine(pen, _lastPoint, e.Location);
                }
                _lastPoint = e.Location;
                ((PictureBox)sender).Invalidate(); // Trigger Paint event
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastPoint = Point.Empty;
            }
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_drawingBitmap, 0, 0);
        }

        private void ClearDrawing()
        {
            using (Graphics g = Graphics.FromImage(_drawingBitmap))
            {
                g.Clear(Color.Transparent);
            }
            ((PictureBox)this.Controls.OfType<PictureBox>().FirstOrDefault()).Invalidate(); // Trigger Paint event
        }
    }
}