namespace MasterZeka
{
    public partial class CroppedImageForm : Form
    {
        private Image _croppedImage;
        private bool _isDrawing = false;
        private bool _isErasing = false;
        private Point _lastPoint;
        private Bitmap _drawingBitmap;
        private Graphics _drawingGraphics;

        public CroppedImageForm(Image croppedImage)
        {
            InitializeComponent();
            _croppedImage = croppedImage;
        }

        private void CroppedImageForm_Load(object sender, EventArgs e)
        {
            PictureBox pictureBox = this.Controls.OfType<PictureBox>().FirstOrDefault();
            if (pictureBox != null)
            {
                pictureBox.Image = _croppedImage;
            }

            // Drawing Bitmap and Graphics initialization
            _drawingBitmap = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
            _drawingGraphics = Graphics.FromImage(_drawingBitmap);
            _drawingGraphics.Clear(Color.Transparent);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_isDrawing || _isErasing)
                {
                    _lastPoint = e.Location;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
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
            else if (_isErasing && e.Button == MouseButtons.Left)
            {
                using (Graphics g = Graphics.FromImage(_drawingBitmap))
                {
                    g.FillRectangle(new SolidBrush(Color.Transparent), e.X, e.Y, 10, 10);
                }
                ((PictureBox)sender).Invalidate(); // Trigger Paint event
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _lastPoint = Point.Empty;
            }
        }

        private void ClearDrawing()
        {
            _drawingGraphics.Clear(Color.Transparent);
            ((PictureBox)this.Controls.OfType<PictureBox>().FirstOrDefault()).Invalidate(); // Trigger Paint event
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_croppedImage, 0, 0);
            e.Graphics.DrawImage(_drawingBitmap, 0, 0);
        }
    }
}
