using System;
using System.Windows.Forms;

namespace ConsoleApp1
{
    public partial class Form1 : Form
    {
        private THM thm;
        private int alt_texture_type = 0;
        private bool need_alt_texture_type = false;
        public bool need_update_values = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            thm = new THM(this);
        }

        public void Form_Update()
        {
            try
            {
                comboBox1.SelectedIndex = (int)thm.type;
                need_alt_texture_type = false;
            }
            catch(ArgumentOutOfRangeException)
            {
                alt_texture_type = (int)thm.type;
                need_alt_texture_type = true;
            }
            comboBox2.SelectedIndex = (int)thm.fmt;
            comboBox3.SelectedIndex = (int)thm.bump_mode;
            comboBox4.SelectedIndex = (int)thm.mip_filter;

            textBox4.Text = thm.border_color.ToString();
            textBox6.Text = thm.fade_color.ToString();
            textBox5.Text = thm.fade_amount.ToString();
            textBox8.Text = thm.width.ToString();
            textBox9.Text = thm.height.ToString();
            textBox10.Text = thm.fade_delay.ToString();
            textBox11.Text = thm.material_weight.ToString();
            textBox12.Text = thm.bump_virtual_height.ToString();
            textBox1.Text = thm.detail_name;
            textBox2.Text = thm.bump_name;
            textBox3.Text = thm.ext_normal_map_name;
        }
        public void Values_Update()
        {
            thm.type = need_alt_texture_type ? (THM.ETType)alt_texture_type : (THM.ETType)comboBox1.SelectedIndex;
            thm.fmt = (THM.ETFormat)comboBox2.SelectedIndex;
            thm.bump_mode = (THM.ETBumpMode)comboBox3.SelectedIndex;
            thm.mip_filter = (THM.EMIPFilters)comboBox4.SelectedIndex;

            thm.border_color = Convert.ToUInt32(textBox4.Text);
            thm.fade_color = Convert.ToUInt32(textBox6.Text);
            thm.fade_amount = Convert.ToUInt32(textBox5.Text);
            thm.width = Convert.ToUInt32(textBox8.Text);
            thm.height = Convert.ToUInt32(textBox9.Text);
            thm.fade_delay = Convert.ToByte(textBox10.Text);
            thm.material_weight = Convert.ToSingle(textBox11.Text);
            thm.bump_virtual_height = Convert.ToSingle(textBox12.Text);
            thm.detail_name = textBox1.Text;
            thm.bump_name = textBox2.Text;
            thm.ext_normal_map_name = textBox3.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();

            try
            {
                thm.Load(openFileDialog1.FileName);
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();

            try
            {
                thm.Save(saveFileDialog1.FileName);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            thm.soc_cop_repair();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }
        private void label5_Click(object sender, EventArgs e)
        {
        }
        private void label8_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (need_update_values)
                Values_Update();
            thm.OnTypeChange();
        }
    }
}
