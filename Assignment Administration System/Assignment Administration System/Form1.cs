using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace Assignment_Administration_System
{
    public partial class Form1 : Form
    {
        MySqlConnection con;
        public Form1()
        {  
            InitializeComponent();
            con = new MySqlConnection("server=localhost;Allow User Variables=True;user id=root;password=;database=assignments;");
            openCon();
        }
        private void openCon()
        {
            if (con.State == ConnectionState.Closed)
            {
                 con.Open();
            }
        }
        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
            {
                this.label1.Visible = false;
            }
        }

        private void textBox1_MouseLeave(object sender, EventArgs e)
        {
            if (txtUsername.Text == "")
            {
                this.label1.Visible = true;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string Username;
            string Password;
            Username = txtUsername.Text;
            Password = txtPassword.Text;
            if (Username == "" && Password == "")
            {
                MessageBox.Show("Please Enter Username and Password Login", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clear();
            }
            else if (Username == "" && Password != "")
            {
                MessageBox.Show("Please enter Username to login!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtUsername.Text = "";
                txtUsername.Focus();
            }
            else if (Username != "" && Password == "")
            {
                MessageBox.Show("Please enter Password to login!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Text = "";
                txtPassword.Focus();
            }
            else
            {
                MySqlCommand log3 = new MySqlCommand("SELECT * FROM users WHERE pwd=md5('" + Password + "')", con);
                MySqlDataAdapter adplog3 = new MySqlDataAdapter(log3);
                DataTable dtlog3 = new DataTable();
                adplog3.Fill(dtlog3);
                try
                {
                    string b = dtlog3.Rows[0][0].ToString();
                    string a = dtlog3.Rows[0][1].ToString();
                    if (Username == "" && Password == "")
                    {
                        MessageBox.Show("Please Enter Username and Password to Login", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Clear();
                    }
                    if (Username == b)
                    {
                        MySqlCommand cmd51 = new MySqlCommand("update usernow set uname='" + this.txtUsername.Text + "' where id='1'", con);
                        cmd51.ExecuteNonQuery();
                        this.groupBox1.Visible = true;
                        timer1.Enabled = true;

                    }
                    if (Username != b)
                    {
                        MessageBox.Show("Invalid Username, Please retry!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtUsername.Text = "";
                        txtUsername.Focus();
                    }
                }
                catch
                {
                    MessageBox.Show("For security reasons this application has to close. Try again with correct credentials.", "Have a nice day!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Application.Exit();
                }
            }
        }
        private void Clear()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void txtPassword_MouseEnter(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                this.label2.Visible = false;
            }
        }

        private void txtPassword_MouseLeave(object sender, EventArgs e)
        {
            if (txtPassword.Text == "")
            {
                this.label2.Visible = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Opacity <= 10)
            {
                Form2 admin = new Form2();
                admin.Show();
                this.Hide();
                timer1.Enabled = false;
            }
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                if (e.KeyChar.Equals(Convert.ToChar(13)))
                {
                    btnLogin_Click(sender, e);
                }
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                if (e.KeyChar.Equals(Convert.ToChar(13)))
                {
                    btnLogin_Click(sender, e);
                }
            }
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnclr_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            this.label1.Visible = false;
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            this.label2.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
