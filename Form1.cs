using System.Reflection.Emit;
using System.Windows.Forms;

namespace MasterZeka
{
    public partial class Form1 : Form
    {
        int i = 0;
        string imageurl = "";


        public Form1()
        {
            InitializeComponent();
            LoadImage();

        }

        private void LoadImage()
        {
            imageurl = "https://firebasestorage.googleapis.com/v0/b/masterzekasorubankasi.appspot.com/o/pages%2Fpage_" + i + ".png?alt=media";
            pictureBox1.Load(imageurl);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        public void Form1_Load(object sender, EventArgs e)
        {

        }

        public void button5_Click(object sender, EventArgs e)
        {


            this.Close();

        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (i == 322)
            {
                i = 1;
                LoadImage();
                label1.Text = (i - 1).ToString();
            }
            else
            {
                i++;
                LoadImage();
                label1.Text = (i - 1).ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (i == 1)
            {
                i = 322;
                LoadImage();
                label1.Text = (i - 1).ToString();
            }
            else
            {
                i--;
                LoadImage();
                label1.Text = (i - 1).ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int sayfa;
            if (int.TryParse(textBox1.Text, out sayfa))
            {
                if (sayfa > 322)
                {
                    sayfa = 322;
                    i = sayfa;
                    LoadImage();
                    label1.Text = (i - 1).ToString();
                }
                if (sayfa <= 0)
                {
                    sayfa = 0;
                    i = sayfa;
                    LoadImage();
                    label1.Text = (i - 1).ToString();
                }

                else
                {
                    i = sayfa + 1;
                    LoadImage();
                    label1.Text = (i - 1).ToString();
                }
            }
        }

    }
}