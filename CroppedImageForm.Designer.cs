namespace MasterZeka
{
    partial class CroppedImageForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // PictureBox oluştur
            PictureBox pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };
            pictureBox.MouseDown += pictureBox1_MouseDown;
            pictureBox.MouseMove += pictureBox1_MouseMove;
            pictureBox.MouseUp += pictureBox1_MouseUp;
            pictureBox.Paint += pictureBox1_Paint;
            Controls.Add(pictureBox);

            // Kalem butonu oluştur
            Button penButton = new Button
            {
                Text = "Kalem",
                Dock = DockStyle.Left
            };
            penButton.Click += (s, e) =>
            {
                _isDrawing = true;
                _isErasing = false;
            };

            // Silgi butonu oluştur
            Button eraserButton = new Button
            {
                Text = "Silgi",
                Dock = DockStyle.Left
            };
            eraserButton.Click += (s, e) =>
            {
                _isDrawing = false;
                _isErasing = true;
            };

            // Temizleme butonu oluştur
            Button clearButton = new Button
            {
                Text = "Temizle",
                Dock = DockStyle.Left
            };
            clearButton.Click += (s, e) => ClearDrawing();

            // Araç çubuğu oluştur
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Items.Add(new ToolStripControlHost(penButton));
            toolStrip.Items.Add(new ToolStripControlHost(eraserButton));
            toolStrip.Items.Add(new ToolStripControlHost(clearButton));
            Controls.Add(toolStrip);

            // Form özelliklerini ayarla
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "CroppedImageForm";
            Text = "Kırpılmış Resim";
            Load += CroppedImageForm_Load;
            ResumeLayout(false);
        }
    }
}
