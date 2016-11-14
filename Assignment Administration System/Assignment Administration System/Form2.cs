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
using System.Configuration;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms.DataVisualization.Charting;
using System.Security;
using System.Security.Permissions;

namespace Assignment_Administration_System
{
    public partial class Form2 : Form
    {

        MySqlConnection con;
        public Form2()
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

        private void btnGO_Click(object sender, EventArgs e)
        {
            string Administrator = "Administrator";
            if (txtUinput.Text == "")
            {
                MessageBox.Show("You need to provide administrator password to unlock these features.", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtUinput.Focus();
                this.groupBox2.Enabled = false;
                this.groupBox3.Enabled = false;
            }
            else
            {
                refCon();
                string userinput = txtUinput.Text;
                MySqlCommand cmd40 = new MySqlCommand("SELECT * FROM users WHERE pwd=md5('" + userinput + "')", con);
                MySqlDataAdapter adp40 = new MySqlDataAdapter(cmd40);
                DataTable dt40 = new DataTable();
                adp40.Fill(dt40);

                string a = dt40.Rows[0][0].ToString();
                
                if (Administrator == a)
                {
                    adminlog();
                }
                if (Administrator != a)
                {
                    MessageBox.Show("Password is incorrect, please try again!", "Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtUinput.Clear();
                    txtUinput.Focus();
                    this.groupBox2.Enabled = false;
                    this.groupBox3.Enabled = false;
                }
            }
        }

        private void adminlog()
        {
            this.groupBox2.Enabled = true;
            this.groupBox3.Enabled = true;
            txtUname.Focus();
            updateDataGridView();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            refCon();
            string uname = txtUname.Text;
            string newpw = txtPwd.Text;
            string newcpw = txtCpwd.Text;

            if (uname == "" | newpw == "" | newcpw == "")
            {
                MessageBox.Show("Fill All Fields To Register", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                if (newpw != newcpw)
                {
                    MessageBox.Show("Password Mismatch", "Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txtCpwd.Clear();
                    txtPwd.Clear();
                    txtPwd.Focus();
                }
                else
                {
                    try
                    {
                        if (MessageBox.Show("Are you sure about registering '" + uname + "' as a new user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            MySqlCommand up = new MySqlCommand("insert into users(uname,pwd) values('" + uname + "',md5('" + newpw + "'))", con);
                            up.ExecuteNonQuery();
                            MessageBox.Show("New registration successful", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtUname.Clear();
                            txtPwd.Clear();
                            txtCpwd.Clear();
                            txtUname.Focus();
                            upCboUname();

                            updateDataGridView();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("This username already exists, or Password guideline violation. Please try again. Error : "+ex.Message, "Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txtUname.Clear();
                        txtUname.Focus();
                    }
                }
            }
        }

        private void updateDataGridView()
        {
            refCon();
            MySqlCommand cmd2 = new MySqlCommand("SELECT uname FROM users", con);
            cmd2.ExecuteNonQuery();
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd2);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            UpdateCBox();
            txtreg2.Focus();
            upCboUname();
            try 
            {
                refCon();
                MySqlCommand cmd50 = new MySqlCommand("SELECT * FROM usernow where id='1'", con);
                MySqlDataAdapter adp50 = new MySqlDataAdapter(cmd50);
                DataTable dt50 = new DataTable();
                adp50.Fill(dt50);
                string cu = dt50.Rows[0][1].ToString();
                if (cu == "Administrator")
                {
                    adminlog();
                    txtUinput.Enabled = false;
                    label5.Enabled = false;
                    btnGO.Enabled = false;
                    button2.Enabled = false;
                    label49.Text = "Welcome " + cu + "..!";
                    txtUname.Focus();
                }
                else
                {
                    label49.Text = "Welcome " + cu + "..!";
                }
            }
            catch
            {
                label49.Text = "Welcome..!";
            }
        }
        
        private void upCboUname()
        {
            refCon();
            MySqlCommand cmd16 = new MySqlCommand("SELECT uname FROM users", con);
            MySqlDataReader reader4;
            reader4 = cmd16.ExecuteReader();
            while (reader4.Read())
            {

                if (!cboun.Items.Contains(reader4.GetString("uname")))
                {
                    cboun.Items.Add(reader4.GetString("uname"));
                }
            }
        }

        private void btnDeluser_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDel.Text == "")
                {
                    MessageBox.Show("Enter an username to delete", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtDel.Focus();
                }
                else
                {
                    if (txtDel.Text == "Administrator")
                    {
                        MessageBox.Show("You cannot delete the built in Administrator account!", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else if (MessageBox.Show("The user '" + this.txtDel.Text + "' will be deleted permanently, are you sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MySqlCommand cmd1 = new MySqlCommand("SELECT * FROM users WHERE uname='" + this.txtDel.Text + "';", con);
                        MySqlDataAdapter adp1 = new MySqlDataAdapter(cmd1);
                        DataTable dt1 = new DataTable();
                        adp1.Fill(dt1);
                        string del = dt1.Rows[0][1].ToString();
                        MySqlCommand cmd2 = new MySqlCommand("delete from users where uname='" + this.txtDel.Text + "';", con);
                        cmd2.ExecuteNonQuery();
                        MessageBox.Show("User Deleted", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtDel.Clear();
                        txtDel.Focus();
                        updateDataGridView();
                    }
                    else
                    {
                        MessageBox.Show("Error occurred while executing the process. This is most likely because the system do not found username you entered or the current user cancelled the deleting process, Please retry!", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDel.Clear();
                        txtDel.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There is no user listed in that username, Please retry! Error : "+ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txtDel.Clear();
                txtDel.Focus();
            }
        }

        private void txtDel_MouseEnter(object sender, EventArgs e)
        {
            txtDel.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUname.Clear();
            txtPwd.Clear();
            txtCpwd.Clear();
            txtUname.Focus();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            openFileDialog1.Filter = "CSV (Comma Delimited) (*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            refCon();
            if (txtBatch.Text != "")
            {
                string folderPath = textBox1.Text;
                string batch = txtBatch.Text;
                try
                {
                    if (txtBatch.Text != "Type Batch Number Here")
                    {
                        if (textBox1.Text != "Browse For Your CSV File")
                        {
                            int pv;
                            if (!int.TryParse(txtBatch.Text, out pv))
                            {
                                MessageBox.Show("Batch number not valid!", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
                            }
                            else
                            {
                                folderPath = importData(folderPath, batch);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter batch number first!", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
                    }
                }
                catch(Exception ex)
                {
                    if (MessageBox.Show("There is already a table in that name, would you like to empty that table and add these new records? Error: "+ex.Message, "Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Hand) == DialogResult.Yes)
                    {
                        delTable(batch);
                        folderPath = importData(folderPath, batch);
                    }
                }
            }
            else
            {
                MessageBox.Show("Enter batch number first!", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Stop);
            }
        }

        private void delTable(string batch)
        {
            refCon();
            MySqlCommand cmd4 = new MySqlCommand("DROP TABLE IF EXISTS `" + batch + "`", con);
            cmd4.ExecuteNonQuery();
            MySqlCommand cmd9 = new MySqlCommand("DELETE FROM `batches` WHERE `bname` = '" + batch + "'", con);
            cmd9.ExecuteNonQuery();
        }

        private string importData(string folderPath, string batch)
        {
            MySqlCommand cmd2 = new MySqlCommand("CREATE TABLE `" + batch + "`(Ref_No int(2),Stu_ID varchar(10),Reg_No varchar(13) NOT NULL,Student_Name varchar(100),Student_Address varchar(100),Tell varchar(20),Student_Email varchar(50),Batch varchar(15) NOT NULL,Reg_Status varchar(10),A1 int(3),A2 int(3),A3 int(3),A4 int(3),A5 int(3),A6 int(3),A7 int(3),A8 int(3),A9 int(3),A10 int(3),Blog_Address varchar(50),PRIMARY KEY(Reg_No))", con);
            cmd2.ExecuteNonQuery();
            folderPath = folderPath.Replace("\\", "/");
            MySqlCommand cmd1 = new MySqlCommand("LOAD DATA LOCAL INFILE '" + folderPath + "' INTO TABLE `" + batch + "` FIELDS TERMINATED BY ',' ENCLOSED BY '\"' LINES TERMINATED BY '\r\n' (Ref_No,Stu_ID,Reg_No,Student_Name,Student_Address,Tell,Student_Email,Batch,Reg_Status)", con);
            cmd1.ExecuteNonQuery();
            MySqlCommand cmd10 = new MySqlCommand("DELETE FROM `" + batch + "` WHERE `Reg_No` = 'Reg No'", con);
            cmd10.ExecuteNonQuery();
            MySqlCommand cmd13 = new MySqlCommand("update `" + batch + "` set A1='0',A2='0',A3='0',A4='0',A5='0',A6='0',A7='0',A8='0',A9='0',A10='0';", con);
            cmd13.ExecuteNonQuery();
            MySqlCommand cmd3 = new MySqlCommand("SELECT * FROM `" + batch + "`", con);
            cmd3.ExecuteNonQuery();
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd3);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            dataGridView3.DataSource = dt;
            this.label24.Visible = false;
            MessageBox.Show("Successfully imported!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            try
            {
                MySqlCommand cmd6 = new MySqlCommand("insert into batches(bname) values('" + this.txtBatch.Text + "')", con);
                cmd6.ExecuteNonQuery();
                MessageBox.Show("New batch created", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateCBox();
                txtreg2.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while creating a new batch,this is most likely because you haven't specified a valid CSV file, batch number is not in correct format or the given batch number already exists, contact your system administrator. Error : " + ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                UpdateCBox();
            }
            UpdateCBox();
            textBox1.Text = "Browse For Your CSV File";
            txtBatch.Text = "Type Batch Number Here";
            return folderPath;
        }

        private void UpdateCBox()
        {
            refCon();
            MySqlCommand cmd7 = new MySqlCommand("SELECT * FROM batches", con);
            MySqlDataReader reader;
            reader = cmd7.ExecuteReader();

            while (reader.Read())
            {     
                if (!cbobatch3.Items.Contains(reader.GetString("bname")))
                {
                    cbobatch3.Items.Add(reader.GetString("bname"));
                }
                if (!cboin.Items.Contains(reader.GetString("bname")))
                {
                    cboin.Items.Add(reader.GetString("bname"));
                }
                if (!cbodelbatch.Items.Contains(reader.GetString("bname")))
                {   
                    cbodelbatch.Items.Add(reader.GetString("bname"));
                }
                if (!cbobatch4.Items.Contains(reader.GetString("bname")))
                {
                    cbobatch4.Items.Add(reader.GetString("bname"));
                }
                if (!cbobatch5.Items.Contains(reader.GetString("bname")))
                {
                    cbobatch5.Items.Add(reader.GetString("bname"));
                }
                if (!cbobatch6.Items.Contains(reader.GetString("bname")))
                {
                    cbobatch6.Items.Add(reader.GetString("bname"));
                }
                if (!cbobatch7.Items.Contains(reader.GetString("bname")))
                {
                    cbobatch7.Items.Add(reader.GetString("bname"));
                }
                if (!cbobatch8.Items.Contains(reader.GetString("bname")))
                {
                    cbobatch8.Items.Add(reader.GetString("bname"));
                }
            }
        }

        private void txtA_MouseEnter(object sender, EventArgs e)
        {
            txtBatch.Text = "";
            txtBatch.Focus();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtreg2.Text != "")
                {
                    if (cbobatch3.Text != "Batch")
                    {
                        Search();
                    }
                    else
                    {
                        MessageBox.Show("Enter valid batch number to search", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        UpdateCBox();
                    }  
                }
                else
                {
                    MessageBox.Show("Enter Registration Number to search", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    UpdateCBox();
                }
                UpdateCBox();
            }
            catch (Exception)
            {
                UpdateCBox();
                if(MessageBox.Show("Error occurred, this is most likely because no record was found within the given criteria, would you like to add a new record in this registration number?","Error",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show("Fill necessary fields for this new record and press Update Record button", "New", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Clear();
                }
            }
        }

        private void Search()
        {
            refCon();
            MySqlCommand cmd8 = new MySqlCommand("SELECT * FROM `" + cbobatch3.Text + "` WHERE `Reg_No`='" + textBox2.Text + txtreg2.Text + "'", con);
            cmd8.ExecuteNonQuery();
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd8);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            txtnic.Text = dt.Rows[0][1].ToString();
            txtstname2.Text = dt.Rows[0][3].ToString();
            txtaddress.Text = dt.Rows[0][4].ToString();
            txtemail.Text = dt.Rows[0][6].ToString();
            txttp.Text = dt.Rows[0][5].ToString();
            txtbatch2.Text = dt.Rows[0][7].ToString();
            cbostatus.Text = dt.Rows[0][8].ToString();

            txt01.Text = dt.Rows[0][9].ToString();
            txt02.Text = dt.Rows[0][10].ToString();
            txt03.Text = dt.Rows[0][11].ToString();
            txt04.Text = dt.Rows[0][12].ToString();
            txt05.Text = dt.Rows[0][13].ToString();
            txt06.Text = dt.Rows[0][14].ToString();
            txt07.Text = dt.Rows[0][15].ToString();
            txt08.Text = dt.Rows[0][16].ToString();
            txt09.Text = dt.Rows[0][17].ToString();
            txt10.Text = dt.Rows[0][18].ToString();

            string ba = dt.Rows[0][19].ToString();
            
            if (ba == "")
            {
                linkLabel1.Text = "No Blog Address";
            }
            else
            {
                linkLabel1.Text = ba;
                txtba.Text = dt.Rows[0][19].ToString();
                txtba.Text = txtba.Text.Remove(txtba.Text.Length - 13);
            }
            UpdateCBox();
        }

        private void txtbatch2_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void btnSearch_MouseEnter(object sender, EventArgs e)
        {
            this.lbls.Visible = true;
        }

        private void btnSearch_MouseLeave(object sender, EventArgs e)
        {
            this.lbls.Visible = false;
        }

        private void btnupdate_MouseEnter(object sender, EventArgs e)
        {
            this.lblu.Visible = true;
        }

        private void btnupdate_MouseLeave(object sender, EventArgs e)
        {
            this.lblu.Visible = false;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                refCon();
                if (txtreg2.Text != "" && cbobatch3.Text != "" | cbobatch3.Text != "Batch")
                {
                    string delrec = this.textBox2.Text + this.txtreg2.Text;
                    string delbatch = this.cbobatch3.Text;
                    Search();
                    if (MessageBox.Show("This record will be deleted permanently, would you like to continue?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
                    {
                        refCon();
                        MySqlCommand cmd5 = new MySqlCommand("DELETE FROM `" + delbatch + "` WHERE `Reg_No` = '" + delrec + "'", con);
                        cmd5.ExecuteNonQuery();
                        MessageBox.Show("Record has been deleted", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Clear();
                    }
                }
                else
                    MessageBox.Show("Please complete the batch number and registration number fields to continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error occurred"+ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
            }
        }

        private void refCon()
        {
            con.Close();
            con.Open();
        }

        private void btnDel_MouseEnter(object sender, EventArgs e)
        {
            this.lbld.Visible = true;
        }

        private void btnDel_MouseLeave(object sender, EventArgs e)
        {
            this.lbld.Visible = false;
        }

        private void btnexport_Click(object sender, EventArgs e)
        {
            try
            {
                string cb = cboin.Text;
                string reg = cboreg3.Text;
                reg = reg.Replace("/", "-");
                string ddte = this.dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
                string dtme = this.dateTimePicker2.Value.TimeOfDay.ToString("hh\\-mm\\-ss");

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                sfd.Filter = "Microsoft Excel 97-2003 Worksheet (*.xls)|*.xls";
                sfd.FileName = "ESOFT GAMPAHA AAS_" + cb + "_" + reg + "_" + ddte + "_" + dtme + ".xls";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ToCsV(dataGridView2, sfd.FileName);
                    if(MessageBox.Show("Export completed, Do you want to open the exported file now?", "Done",MessageBoxButtons.YesNo,MessageBoxIcon.Information)==DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while exporting!" + "Error :" + ex.Message, "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        private void ToCsV(DataGridView dGV, string filename)
        {
            string stOutput = "";
            // Export titles:
            string sHeaders = "";

            for (int j = 0; j < dGV.Columns.Count; j++)
                sHeaders = sHeaders.ToString() + Convert.ToString(dGV.Columns[j].HeaderText) + "\t";
            stOutput += sHeaders + "\r\n";
            // Export data.
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                string stLine = "";
                for (int j = 0; j < dGV.Rows[i].Cells.Count; j++)
                    stLine = stLine.ToString() + Convert.ToString(dGV.Rows[i].Cells[j].Value) + "\t";
                stOutput += stLine + "\r\n";
            }
            Encoding utf16 = Encoding.GetEncoding(1254);
            byte[] output = utf16.GetBytes(stOutput);
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(output, 0, output.Length); //write the encoded file
            bw.Flush();
            bw.Close();
            fs.Close();
        } 
        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboreg3.Visible == true)
                {
                    refCon();
                    MySqlCommand cmd3 = new MySqlCommand("SELECT * FROM `" + cboin.Text + "` WHERE Reg_No='" + cboreg3.Text + "'", con);
                    cmd3.ExecuteNonQuery();
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    dataGridView2.DataSource = dt;
                }
                if (cboreg3.Visible == false && cboName.Visible == false)
                {
                    refCon();
                    MySqlCommand cmd3 = new MySqlCommand("SELECT * FROM `" + cboin.Text + "`", con);
                    cmd3.ExecuteNonQuery();
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    dataGridView2.DataSource = dt;
                }
                if (cboName.Visible == true)
                {
                    refCon();
                    MySqlCommand cmd3 = new MySqlCommand("SELECT * FROM `" + cboin.Text + "` WHERE Student_Name='" + cboName.Text + "'", con);
                    cmd3.ExecuteNonQuery();
                    MySqlDataAdapter adp = new MySqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    adp.Fill(dt);
                    dataGridView2.DataSource = dt;
                }
                if (cboreg3.Text == "" && cboget.Text == "Registration Number")
                {
                    MessageBox.Show("Enter Registration Number first", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                if (cboin.Text == "" && cboreg3.Visible == true | cboreg3.Text == "" && cboget.Text == "Registration Number")
                {
                    MessageBox.Show("Enter Registration Number and Batch Number first", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                if (cboin.Text == "" && cboreg3.Visible == false | cboreg3.Text == "" && cboget.Text == "Batch Number")
                {
                    MessageBox.Show("Enter Batch Number first", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
            }
            catch
            {
                MessageBox.Show("Error occurred, this is most likely because of an unknown criteria. Specify the criteria which you want this report generated first.", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
        }

        private void cboget_SelectedIndexChanged(object sender, EventArgs e)
        {
            refCon();
            this.cboreg3.Visible = false;
            this.cboName.Visible = false;
            try
            {
                if (cboget.Text == "Batch Number")
                { 
                    selectbatch();
                }
                if(cboget.Text == "Registration Number")
                {
                    refCon();
                    this.cboreg3.Visible = true;
                    selectbatch();
                }
                if (cboget.Text == "Student Name")
                {
                    refCon();
                    this.cboName.Visible = true;
                    selectbatch();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown error occurred Error : "+ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
            }
        }

        private void selectbatch()
        {
            refCon();
            MySqlCommand cmd7 = new MySqlCommand("SELECT * FROM batches", con);
            MySqlDataReader reader;
            reader = cmd7.ExecuteReader();

            while (reader.Read())
            {
                if (!cboin.Items.Contains(reader.GetString("bname")))
                {
                    cboin.Items.Add(reader.GetString("bname"));
                }
            }
        }

        private void cboin_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.cboreg3.Items.Clear();
                this.cboName.Items.Clear();
                this.cboreg3.Text = "";
                this.cboName.Text = "";
                refCon();
                MySqlCommand cmd8 = new MySqlCommand("SELECT * FROM `" + cboin.Text + "`", con);
                MySqlDataReader reader2;
                reader2 = cmd8.ExecuteReader();

                while (reader2.Read())
                {
                    if (!cboreg3.Items.Contains(reader2.GetString("Reg_No")))
                    {
                        cboreg3.Items.Add(reader2.GetString("Reg_No"));
                    }
                    if (!cboName.Items.Contains(reader2.GetString("Student_Name")))
                    {
                        cboName.Items.Add(reader2.GetString("Student_Name"));
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error : "+ex.Message,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void txtba_MouseEnter(object sender, EventArgs e)
        {
            txtba.SelectAll();
           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkLabel1.Text == "No Blog Address")
            {
                MessageBox.Show("Error occurred, URL is not valid, please try again.", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else if (linkLabel1.Text == "Blog Address")
            {
            MessageBox.Show("Error occurred, URL is not valid, Please try again.", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }
            else
            {
                string url;
                if (e.Link.LinkData != null)
                    url = e.Link.LinkData.ToString();
                else
                    url = linkLabel1.Text.Substring(e.Link.Start, e.Link.Length);

                if (!url.Contains("://"))
                    url = "http://" + url;

                var si = new ProcessStartInfo(url);
                Process.Start(si);
                linkLabel1.LinkVisited = true;
            }
        }

        private void cboreg3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnCan_MouseEnter(object sender, EventArgs e)
        {
            this.lblc.Visible = true;
        }

        private void btnCan_MouseLeave(object sender, EventArgs e)
        {
            this.lblc.Visible = false;
        }

        private void btnupdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Record will be updated with new records and the existing values will be deleted, Continue?", "Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                string uprec = this.textBox2.Text + this.txtreg2.Text;
                string upbatch = this.cbobatch3.Text;
                if (upbatch != "Batch" && txtreg2.Text != "" && upbatch != "")
                {
                    refCon();
                    try
                    {
                        int a1 = int.Parse(txt01.Text);
                        int a2 = int.Parse(txt02.Text);
                        int a3 = int.Parse(txt03.Text);
                        int a4 = int.Parse(txt04.Text);
                        int a5 = int.Parse(txt05.Text);
                        int a6 = int.Parse(txt06.Text);
                        int a7 = int.Parse(txt07.Text);
                        int a8 = int.Parse(txt08.Text);
                        int a9 = int.Parse(txt09.Text);
                        int a10 = int.Parse(txt10.Text);
                        if (a1 <= 100 && a2 <= 100 && a3 <= 100 && a4 <= 100 && a5 <= 100 && a6 <= 100 && a7 <= 100 && a8 <= 100 && a9 <= 100 && a1 <= 100)
                        {
                            MySqlCommand cmd8 = new MySqlCommand("SELECT * FROM `" + cbobatch3.Text + "` WHERE `Reg_No`='" + textBox2.Text + txtreg2.Text + "'", con);
                            cmd8.ExecuteNonQuery();
                            MySqlDataAdapter adp = new MySqlDataAdapter(cmd8);
                            DataTable dt = new DataTable();
                            adp.Fill(dt);
                            txtnic.Text = dt.Rows[0][1].ToString();

                            refCon();
                            if (linkLabel1.Text == "No Blog Address" | linkLabel1.Text == "Blog Address" && txtba.Text == "Update Blog Address" | txtba.Text == "")
                            {
                                MySqlCommand cmd10 = new MySqlCommand("update `" + upbatch + "` set Stu_ID='" + this.txtnic.Text + "',Student_Name='" + this.txtstname2.Text + "',Student_Address='" + this.txtaddress.Text + "',Tell='" + this.txttp.Text + "',Student_Email='" + this.txtemail.Text + "',Batch='" + this.txtbatch2.Text + "',Reg_Status='" + this.cbostatus.Text + "',A1='" + this.txt01.Text + "',A2='" + this.txt02.Text + "',A3='" + this.txt03.Text + "',A4='" + this.txt04.Text + "',A5='" + this.txt05.Text + "',A6='" + this.txt06.Text + "',A7='" + this.txt07.Text + "',A8='" + this.txt08.Text + "',A9='" + this.txt09.Text + "',A10='" + this.txt10.Text + "',Blog_Address='No Blog Address' where Reg_No='" + uprec + "';", con);
                                cmd10.ExecuteNonQuery();
                                MessageBox.Show("Data Updated", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Search();
                                txtreg2.SelectAll();
                            }
                            else
                            {
                                MySqlCommand cmd10 = new MySqlCommand("update `" + upbatch + "` set Stu_ID='" + this.txtnic.Text + "',Student_Name='" + this.txtstname2.Text + "',Student_Address='" + this.txtaddress.Text + "',Tell='" + this.txttp.Text + "',Student_Email='" + this.txtemail.Text + "',Batch='" + this.txtbatch2.Text + "',Reg_Status='" + this.cbostatus.Text + "',A1='" + this.txt01.Text + "',A2='" + this.txt02.Text + "',A3='" + this.txt03.Text + "',A4='" + this.txt04.Text + "',A5='" + this.txt05.Text + "',A6='" + this.txt06.Text + "',A7='" + this.txt07.Text + "',A8='" + this.txt08.Text + "',A9='" + this.txt09.Text + "',A10='" + this.txt10.Text + "',Blog_Address='" + this.txtba.Text + this.lblblogspot.Text + "' where Reg_No='" + uprec + "';", con);
                                cmd10.ExecuteNonQuery();
                                MessageBox.Show("Data Updated", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Search();
                                txtreg2.SelectAll();
                            }
                        }
                        else
                            MessageBox.Show("Marks should be within 0 and 100", "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
                    }

                    catch (Exception ex)
                    {
                        if (MessageBox.Show("An error occurred, Make sure the assignment marks are in number format and the record " + uprec + " exists, would you like to add this as a new record? Error : " + ex.Message, "Wait!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            try
                            {
                                int a1 = int.Parse(txt01.Text);
                                int a2 = int.Parse(txt02.Text);
                                int a3 = int.Parse(txt03.Text);
                                int a4 = int.Parse(txt04.Text);
                                int a5 = int.Parse(txt05.Text);
                                int a6 = int.Parse(txt06.Text);
                                int a7 = int.Parse(txt07.Text);
                                int a8 = int.Parse(txt08.Text);
                                int a9 = int.Parse(txt09.Text);
                                int a10 = int.Parse(txt10.Text);
                                if (a1 <= 100 && a2 <= 100 && a3 <= 100 && a4 <= 100 && a5 <= 100 && a6 <= 100 && a7 <= 100 && a8 <= 100 && a9 <= 100 && a1 <= 100)
                                {
                                    refCon();
                                    MySqlCommand cmd11 = new MySqlCommand("insert into `" + cbobatch3.Text + "`(Stu_ID,Reg_No,Student_Name,Student_Address,Tell,Student_Email,Batch,Reg_Status,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,Blog_Address) values('" + this.txtnic.Text + "','" + textBox2.Text + txtreg2.Text + "','" + this.txtstname2.Text + "','" + this.txtaddress.Text + "','" + this.txttp.Text + "','" + this.txtemail.Text + "','" + this.txtbatch2.Text + "','" + this.cbostatus.Text + "','" + this.txt01.Text + "','" + this.txt02.Text + "','" + this.txt03.Text + "','" + this.txt04.Text + "','" + this.txt05.Text + "','" + this.txt06.Text + "','" + this.txt07.Text + "','" + this.txt08.Text + "','" + this.txt09.Text + "','" + this.txt10.Text + "','" + this.txtba.Text + this.lblblogspot.Text + "')", con);
                                    cmd11.ExecuteNonQuery();
                                    MessageBox.Show("Data added to the user specified field successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    UpdateCBox();
                                    Search();
                                    txtreg2.SelectAll();
                                }
                                else
                                    MessageBox.Show("Marks should be within 0 and 100", "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
                            }
                            catch (Exception ex1)
                            {
                                MessageBox.Show("Error occurred, Please enter valid batch number and valid registration number before adding!, Make sure marks are in number format. Error: " + ex1.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                UpdateCBox();
                                txtreg2.SelectAll();
                            }
                        }
                    }
                }
                else
                    MessageBox.Show("Please enter registration number and batch number first.", "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
            }
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView2.Rows[e.RowIndex].ReadOnly = true;
                if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dataGridView2.Rows[e.RowIndex].ReadOnly = false;
                }
            }
            catch
            {
                MessageBox.Show("Not allowed", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView1.Rows[e.RowIndex].ReadOnly = true;
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                {
                    dataGridView1.Rows[e.RowIndex].ReadOnly = false;
                }
            }
            catch
            {
                MessageBox.Show("Not allowed", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
            dataGridView3.Rows[e.RowIndex].ReadOnly = true;
            if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                dataGridView3.Rows[e.RowIndex].ReadOnly = false;
            }
            }
            catch
            {
                MessageBox.Show("Not allowed", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            {
                dateTimePicker2.Value = DateTime.Now;
            }
        }

        private void dataGridView2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnCan_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            txtreg2.Clear();
            txtbatch2.Clear();
            txtnic.Clear();
            txtstname2.Clear();
            txttp.Clear();
            txtemail.Clear();
            cbobatch3.Text = "Batch";
            cbostatus.Text = "";
            txtaddress.Clear();
            linkLabel1.Text = "Blog Address";
            txtba.Text = "Update Blog Address";
            txtZero();
        }

        private void txtZero()
        {
            txt01.Text = "0";
            txt02.Text = "0";
            txt03.Text = "0";
            txt04.Text = "0";
            txt05.Text = "0";
            txt06.Text = "0";
            txt07.Text = "0";
            txt08.Text = "0";
            txt09.Text = "0";
            txt10.Text = "0";
        }

        private void txtba_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnDelBatch_Click(object sender, EventArgs e)
        {
            textBox4.Visible = true;
            label48.Visible = true;
        }

        private void cbodelbatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string delbatch = cbodelbatch.Text;
                refCon();
                MySqlCommand cmd15 = new MySqlCommand("SELECT * FROM `" + delbatch + "`", con);
                cmd15.ExecuteNonQuery();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd15);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                this.pictureBox4.Visible = false;
                this.pictureBox2.Visible = false;
                this.pictureBox3.Visible = false;
                dataGridView4.DataSource = dt;
                UpdateCBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred, maybe because that batch is already deleted. Error: " + ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
                UpdateCBox();
            }
        }

        private void cbodelbatch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnDelBatch_Click(sender, e);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnImport_Click(sender, e);
            }
        }

        private void txtBatch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnBrowse_Click(sender, e);
            }
        }

        private void txt01_Click(object sender, EventArgs e)
        {
            txt01.SelectAll();
        }

        private void txt02_Click(object sender, EventArgs e)
        {
            txt02.SelectAll();
        }

        private void txt03_Click(object sender, EventArgs e)
        {
            txt03.SelectAll();
        }

        private void txt04_Click(object sender, EventArgs e)
        {
            txt04.SelectAll();
        }

        private void txt05_Click(object sender, EventArgs e)
        {
            txt05.SelectAll();
        }

        private void txt06_Click(object sender, EventArgs e)
        {
            txt06.SelectAll();
        }

        private void txt07_Click(object sender, EventArgs e)
        {
            txt07.SelectAll();
        }

        private void txt08_Click(object sender, EventArgs e)
        {
            txt08.SelectAll();
        }

        private void txt09_Click(object sender, EventArgs e)
        {
            txt09.SelectAll();
        }

        private void txt10_Click(object sender, EventArgs e)
        {
            txt10.SelectAll();
        }

        private void txtreg2_Click(object sender, EventArgs e)
        {
            txtreg2.SelectAll();
        }

        private void txtnic_Click(object sender, EventArgs e)
        {
            txtreg2.SelectAll();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtDel.Text = this.dataGridView1.CurrentCell.Value.ToString();
        }

        private void txtreg2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnSearch_Click(sender, e);
            }
        }

        private void txt01_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt02_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt03_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt04_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt05_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt06_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt07_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt08_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt09_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void txt10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnupdate_Click(sender, e);
            }
        }

        private void cbobatch3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnSearch_Click(sender, e);
            }
        }

        private void txtUinput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnGO_Click(sender, e);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            refCon();
            this.chart1.Series["Marks"].Points.Clear();
            string batch4 = this.cbobatch4.Text;
            string y = this.cboAS.Text;
            string x = this.cboX.Text;
            if (batch4 == "" | y == "" | x == "")
            {
                MessageBox.Show("Fill all fields correctly before loading chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    MySqlCommand cmd16 = new MySqlCommand("SELECT * FROM `" + batch4 + "`", con);
                    MySqlDataReader reader3;
                    reader3 = cmd16.ExecuteReader();
                    while (reader3.Read())
                    {
                        this.chart1.Series["Marks"].Points.AddXY(reader3.GetString(x), reader3.GetInt32(y));
                        this.button1.Enabled = true;
                        this.pictureBox6.Visible = false;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error occurred while loading, Error : "+ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string bat2 = cbobatch4.Text;
            string As = cboAS.Text;
            string x = cboX.Text;

            string ddte = this.dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
            string dtme = this.dateTimePicker2.Value.TimeOfDay.ToString("hh\\-mm\\-ss");

            if (bat2 == "" | As == "" | x == "" | bat2 == "Batch Number" | As == "Assignment" | bat2 == "Batch Number" && As == "Assignment" && x == "Reg_No")
            {
                MessageBox.Show("You can't save an empty chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    SaveFileDialog sfd1 = new SaveFileDialog();
                    sfd1.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                    sfd1.Filter = "JPG File (*.jpg)|*.jpg";
                    sfd1.FileName = "ESOFT GAMPAHA AAS-CHART_via" + x + "_" + As + "_" + bat2 + "_" + ddte + "_" + dtme + ".jpg";
                    if (sfd1.ShowDialog() == DialogResult.OK)
                    {
                        string savep = sfd1.FileName;
                        this.chart1.SaveImage(savep, ChartImageFormat.Jpeg);
                        this.button1.Enabled = false;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error occurred while saving, Error : " + ex.Message, "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnchange_Click(object sender, EventArgs e)
        {
            refCon();

            string cupwd = txtccpwd.Text;
            string newpw = txtnewp.Text;
            string newcpw = txtcnewp.Text;
            string un = cboun.Text;
            if (cupwd == "" | newpw == "" | newcpw == "")
            {
                MessageBox.Show("Fill all fields before changing password", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    refCon();
                    MySqlCommand log4 = new MySqlCommand("SELECT * FROM users WHERE pwd=md5('" + cupwd + "')", con);
                    MySqlDataAdapter adplog3 = new MySqlDataAdapter(log4);
                    DataTable dtlog3 = new DataTable();
                    adplog3.Fill(dtlog3);
                    string c = dtlog3.Rows[0][0].ToString();
                    if (c == un)
                    {
                        if (newpw != newcpw)
                        {
                            MessageBox.Show("Confirmation password is not identical.", "Error occurred!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
                            txtCpwd.Clear();
                            txtPwd.Clear();
                            txtPwd.Focus();
                        }
                        else
                        {
                            try
                            {
                                if (MessageBox.Show("Are you sure about changing your password, this process is not reversible.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    refCon();

                                    MySqlCommand up1 = new MySqlCommand("update users set pwd=md5('" + newpw + "') where uname='" + un + "';", con);
                                    up1.ExecuteNonQuery();
                                    MessageBox.Show("Successfully changed your password", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    refnewpw();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Password guideline violation. : " + ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                refnewpw();
                            }
                        }
                    }
                    else
                        MessageBox.Show("Something's wrong, please type your current password correctly.", "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Following error occurred. : " + ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    refnewpw();
                }
            }
        }

        private void refnewpw()
        {
            txtccpwd.Clear();
            txtnewp.Clear();
            txtcnewp.Clear();
            upCboUname();
            updateDataGridView();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            refCon();
            this.chart2.Series["Average"].Points.Clear();
            string batch4 = this.cbobatch5.Text;
            string f = this.cboX2.Text;
            if (batch4 == "" | f == "")
            {
                MessageBox.Show("Fill all fields correctly before loading chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    MySqlCommand cmd17 = new MySqlCommand("Select " + f + ", (A1+A2+A3+A4+A5+A6+A7+A8+A9+A10)/10 as Average from `" + batch4 + "` Group by " + f + "", con);
                    MySqlDataReader reader3;
                    reader3 = cmd17.ExecuteReader();
                    while (reader3.Read())
                    {

                        this.chart2.ChartAreas["ChartArea1"].AxisX.Interval = 1; 
                        this.chart2.Series["Average"].Points.AddXY(reader3.GetString(f), reader3.GetInt32("Average"));
                        this.chart2.Series["Average"].Color = System.Drawing.Color.Aqua;
                        this.button3.Enabled = true;
                        this.pictureBox9.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while loading, Error : " + ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void cbobatch5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string bat3 = cbobatch5.Text;
            string x2 = cboX.Text;

            string ddte = this.dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
            string dtme = this.dateTimePicker2.Value.TimeOfDay.ToString("hh\\-mm\\-ss");

            if (bat3 == "" | x2 == "" | bat3 == "Batch Number" | bat3 == "Batch Number" && x2 == "Reg_No")
            {
                MessageBox.Show("You can't save an empty chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    SaveFileDialog sfd2 = new SaveFileDialog();
                    sfd2.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                    sfd2.Filter = "JPG File (*.jpg)|*.jpg";
                    sfd2.FileName = "ESOFT GAMPAHA AAS-CHART_via" + x2 + "_" + bat3 + "_" + ddte + "_" + dtme + ".jpg";
                    if (sfd2.ShowDialog() == DialogResult.OK)
                    {
                        string savep = sfd2.FileName;
                        this.chart2.SaveImage(savep, ChartImageFormat.Jpeg);
                        this.button3.Enabled = false;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while saving, Error : " + ex.Message, "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                }
            }
        }
        int i = 0;
        private void pictureBox5_Click(object sender, EventArgs e)
        { 
            try
            {
                if (txtreg2.Text != "")
                {
                    if (cbobatch3.Text != "Batch")
                    {
                        refCon();
                        MySqlCommand cmd18 = new MySqlCommand("SELECT * FROM `" + cbobatch3.Text + "`", con);
                        cmd18.ExecuteNonQuery();
                        MySqlDataAdapter adp5 = new MySqlDataAdapter(cmd18);
                        DataTable dt5 = new DataTable();
                        adp5.Fill(dt5);
                        i++;
                        if (i < dt5.Rows.Count)
                        {
                            ShowData(dt5);
                            txtreg2.Text = txtreg2.Text.Remove(0, 8);
                        }
                        else
                        {
                            MessageBox.Show("End of records!", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            i = dt5.Rows.Count - 1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter valid batch number to search", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        UpdateCBox();
                    }
                }
                else
                {
                    MessageBox.Show("Enter Registration Number to search", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    UpdateCBox();
                }
                UpdateCBox();
            }
            catch (Exception)
            {
                UpdateCBox();
                if (MessageBox.Show("Error occurred, this is most likely because no record was found within the given criteria, would you like to add a new record in this registration number?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show("Fill necessary fields for this new record and press Update Record button", "New", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Clear();
                }
            }
        }

        private void ShowData(DataTable dt)
        {
            txtreg2.Text = dt.Rows[i][2].ToString();
            txtnic.Text = dt.Rows[i][1].ToString();
            txtstname2.Text = dt.Rows[i][3].ToString();
            txtaddress.Text = dt.Rows[i][4].ToString();
            txtemail.Text = dt.Rows[i][6].ToString();
            txttp.Text = dt.Rows[i][5].ToString();
            txtbatch2.Text = dt.Rows[i][7].ToString();
            cbostatus.Text = dt.Rows[i][8].ToString();

            txt01.Text = dt.Rows[i][9].ToString();
            txt02.Text = dt.Rows[i][10].ToString();
            txt03.Text = dt.Rows[i][11].ToString();
            txt04.Text = dt.Rows[i][12].ToString();
            txt05.Text = dt.Rows[i][13].ToString();
            txt06.Text = dt.Rows[i][14].ToString();
            txt07.Text = dt.Rows[i][15].ToString();
            txt08.Text = dt.Rows[i][16].ToString();
            txt09.Text = dt.Rows[i][17].ToString();
            txt10.Text = dt.Rows[i][18].ToString();
            string ba = dt.Rows[i][19].ToString();
            if (ba == "")
            {
                linkLabel1.Text = "No Blog Address";
            }
            else
            {
                linkLabel1.Text = ba;

            }
            UpdateCBox();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtreg2.Text != "")
                {
                    if (cbobatch3.Text != "Batch")
                    {
                        refCon();
                        MySqlCommand cmd19 = new MySqlCommand("SELECT * FROM `" + cbobatch3.Text + "`", con);
                        cmd19.ExecuteNonQuery();
                        MySqlDataAdapter adp6 = new MySqlDataAdapter(cmd19);
                        DataTable dt6 = new DataTable();
                        adp6.Fill(dt6);
                        i--;             
                        if (i >= 0)             
                        {
                            ShowData(dt6);
                            txtreg2.Text = txtreg2.Text.Remove(0, 8);
                        }             
                        else             
                        {
                            MessageBox.Show("End of records!", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            i = 0;             
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter valid batch number to search", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        UpdateCBox();
                    }
                }
                else
                {
                    MessageBox.Show("Enter Registration Number to search", "Wait!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    UpdateCBox();
                }
                UpdateCBox();
            }
            catch (Exception)
            {
                UpdateCBox();
                if (MessageBox.Show("Error occurred, this is most likely because no record was found within the given criteria, would you like to add a new record in this registration number?", "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MessageBox.Show("Fill necessary fields for this new record and press Update Record button", "New", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Clear();
                }
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(openFileDialog1.FileName);
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void picgmail_Click(object sender, EventArgs e)
        {
            
        }

        private void picblogger_Click(object sender, EventArgs e)
        {
        }

        private void picgoogle_Click(object sender, EventArgs e)
        {
            
        }

        private void picfb_Click(object sender, EventArgs e)
        {
        }

        private void picgo_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void picback_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoBack)
                webBrowser1.GoBack();
        }

        private void txturl_MouseEnter(object sender, EventArgs e)
        {
            txturl.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(this.checkBox1.Checked==true)
            {
                chart1.Series["Marks"].IsValueShownAsLabel = true;
            }
            if (this.checkBox1.Checked == false)
            {
                chart1.Series["Marks"].IsValueShownAsLabel = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == true)
            {
                chart2.Series["Average"].IsValueShownAsLabel = true;
            }
            if (this.checkBox2.Checked == false)
            {
                chart2.Series["Average"].IsValueShownAsLabel = false;
            }
        }

        private void btnload3_Click(object sender, EventArgs e)
        {
            this.chart3.Series["Average (Assignment wise)"].Points.Clear();
            string batch5 = this.cbobatch6.Text;
            if (batch5 == "")
            {
                MessageBox.Show("Select a batch number before loading chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    refCon();
                    MySqlCommand cmd18 = new MySqlCommand("update `tblassign` set avere=(Select avg(A1) from `" + batch5 + "`) where AsID='Assignment-01';", con);
                    cmd18.ExecuteNonQuery();
                    MySqlCommand cmd19 = new MySqlCommand("update `tblassign` set avere=(Select avg(A2) from `" + batch5 + "`) where AsID='Assignment-02';", con);
                    cmd19.ExecuteNonQuery();
                    MySqlCommand cmd20 = new MySqlCommand("update `tblassign` set avere=(Select avg(A3) from `" + batch5 + "`) where AsID='Assignment-03';", con);
                    cmd20.ExecuteNonQuery();
                    MySqlCommand cmd21 = new MySqlCommand("update `tblassign` set avere=(Select avg(A4) from `" + batch5 + "`) where AsID='Assignment-04';", con);
                    cmd21.ExecuteNonQuery();
                    MySqlCommand cmd22 = new MySqlCommand("update `tblassign` set avere=(Select avg(A5) from `" + batch5 + "`) where AsID='Assignment-05';", con);
                    cmd22.ExecuteNonQuery();
                    MySqlCommand cmd23 = new MySqlCommand("update `tblassign` set avere=(Select avg(A6) from `" + batch5 + "`) where AsID='Assignment-06';", con);
                    cmd23.ExecuteNonQuery();
                    MySqlCommand cmd24 = new MySqlCommand("update `tblassign` set avere=(Select avg(A7) from `" + batch5 + "`) where AsID='Assignment-07';", con);
                    cmd24.ExecuteNonQuery();
                    MySqlCommand cmd25 = new MySqlCommand("update `tblassign` set avere=(Select avg(A8) from `" + batch5 + "`) where AsID='Assignment-08';", con);
                    cmd25.ExecuteNonQuery();
                    MySqlCommand cmd26 = new MySqlCommand("update `tblassign` set avere=(Select avg(A9) from `" + batch5 + "`) where AsID='Assignment-09';", con);
                    cmd26.ExecuteNonQuery();
                    MySqlCommand cmd27 = new MySqlCommand("update `tblassign` set avere=(Select avg(A10) from `" + batch5 + "`) where AsID='Assignment-10';", con);
                    cmd27.ExecuteNonQuery();
                    MySqlCommand cmd28 = new MySqlCommand("SELECT * FROM `tblassign`", con);
                    MySqlDataReader reader5;
                    reader5 = cmd28.ExecuteReader();

                    while (reader5.Read())
                    {
                        this.chart3.Series["Average (Assignment wise)"].Points.AddXY(reader5.GetString("AsID"), reader5.GetInt32("avere"));
                        this.chart3.Series["Average (Assignment wise)"].Color = System.Drawing.Color.DarkRed;
                        this.btnsave.Enabled = true;
                        this.pictureBox1.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while loading, Error : " + ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnsave_Click(object sender, EventArgs e)
        {
            string bat4 = cbobatch6.Text;
            string ddte = this.dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
            string dtme = this.dateTimePicker2.Value.TimeOfDay.ToString("hh\\-mm\\-ss");

            if (bat4 == "" | bat4 == "Batch Number")
            {
                MessageBox.Show("You can't save an empty chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    SaveFileDialog sfd3 = new SaveFileDialog();
                    sfd3.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                    sfd3.Filter = "JPG File (*.jpg)|*.jpg";
                    sfd3.FileName = "ESOFT GAMPAHA AAS-CHART_AVG_" + bat4 + "_" + ddte + "_" + dtme + ".jpg";
                    if (sfd3.ShowDialog() == DialogResult.OK)
                    {
                        string savep = sfd3.FileName;
                        this.chart3.SaveImage(savep, ChartImageFormat.Jpeg);
                        this.btnsave.Enabled = false;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while saving, Error : " + ex.Message, "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == true)
            {
                chart3.Series["Average (Assignment wise)"].IsValueShownAsLabel = true;
            }
            if (this.checkBox3.Checked == false)
            {
                chart3.Series["Average (Assignment wise)"].IsValueShownAsLabel = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.groupBox2.Enabled = false;
            this.groupBox3.Enabled = false;
            txtUinput.Clear();
            txtUinput.Focus();
        }

        private void btnload4_Click(object sender, EventArgs e)
        {
            try
            {
                refCon();
                this.chart4.Series["Batch wise average"].Points.Clear();
                int batch7 = int.Parse(cbobatch7.Text);
                int batch8 = int.Parse(cbobatch8.Text);
                string k = this.cboAS2.Text;
                if (batch7 == 0 | k == "" | batch8 == 0)
                {
                    MessageBox.Show("Fill all fields correctly before loading chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    try
                    {
                        MySqlCommand cmd30 = new MySqlCommand("TRUNCATE batches_avg", con);
                        cmd30.ExecuteNonQuery();
                        for (int i = batch7; i <= batch8; i = i + 1)
                        {
                            MySqlCommand cmd32 = new MySqlCommand("Select avg(" + k + ") from `" + i + "` as avgb", con);
                            cmd32.ExecuteNonQuery();
                            textBox3.Text = cmd32.ExecuteScalar().ToString();
                            float ave = float.Parse(textBox3.Text);
                            MySqlCommand cmd31 = new MySqlCommand("insert into batches_avg(bname," + k + ") values('" + i + "','" + ave + "')", con);
                            cmd31.ExecuteNonQuery();
                        }
                        MySqlCommand cmd29 = new MySqlCommand("SELECT * FROM batches_avg", con);
                        MySqlDataReader readerAV;
                        readerAV = cmd29.ExecuteReader();
                        while (readerAV.Read())
                        {
                            this.chart4.ChartAreas["ChartArea1"].AxisX.Interval = 1;
                            this.chart4.Series["Batch wise average"].Points.AddXY(readerAV.GetString("bname"), readerAV.GetInt32(k));
                            this.btnsave2.Enabled = true;
                            this.pictureBox10.Visible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error occurred while loading, Error : " + ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while loading, Error : " + ex.Message, "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked == true)
            {
                chart4.Series["Batch wise average"].IsValueShownAsLabel = true;
            }
            if (this.checkBox4.Checked == false)
            {
                chart4.Series["Batch wise average"].IsValueShownAsLabel = false;
            }
        }

        private void btnsave2_Click(object sender, EventArgs e)
        {
            string bat7 = cbobatch7.Text;
            string bat8 = cbobatch8.Text;
            string As4 = cboAS2.Text;
            string ddte = this.dateTimePicker1.Value.Date.ToString("yyyy-MM-dd");
            string dtme = this.dateTimePicker2.Value.TimeOfDay.ToString("hh\\-mm\\-ss");
            if (bat7 == "" | As4 == "" | bat8 == "" )
            {
                MessageBox.Show("You can't save an empty chart", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                try
                {
                    SaveFileDialog sfd4 = new SaveFileDialog();
                    sfd4.InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
                    sfd4.Filter = "JPG File (*.jpg)|*.jpg";
                    sfd4.FileName = "ESOFT GAMPAHA AAS-CHART_batch_" + bat7 + "_to_" + bat8 + "_" + As4 + "_" + ddte + "_" + dtme + ".jpg";
                    if (sfd4.ShowDialog() == DialogResult.OK)
                    {
                        string savep4 = sfd4.FileName;
                        this.chart4.SaveImage(savep4, ChartImageFormat.Jpeg);
                        this.btnsave2.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while saving, Error : " + ex.Message, "Wait!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            label48.Visible = false;
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                string administrator = "Administrator";
                if (textBox4.Text == "")
                {
                    MessageBox.Show("You need to provide administrator password to delete this batch", "Wait!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox4.Focus();
                }
                else
                {
                    string userinput = textBox4.Text;
                    refCon();
                    MySqlCommand cmd33 = new MySqlCommand("SELECT * FROM users WHERE pwd=md5('" + userinput + "')", con);
                    MySqlDataAdapter adp33 = new MySqlDataAdapter(cmd33);
                    DataTable dt33 = new DataTable();
                    adp33.Fill(dt33);
                    string h = dt33.Rows[0][0].ToString();


                    //MySqlCommand cmd40 = new MySqlCommand("SELECT * FROM users WHERE pwd=md5('" + userinput + "')", con);
                    //MySqlDataAdapter adp40 = new MySqlDataAdapter(cmd40);
                    //DataTable dt40 = new DataTable();
                    //adp40.Fill(dt40);

                    //string a = dt40.Rows[0][0].ToString();


                    if (administrator == h)
                    {
                        textBox4.Visible = false;
                        label48.Visible = false;
                        try
                        {
                            string delbatch = cbodelbatch.Text;
                            refCon();
                            if (MessageBox.Show("The batch " + delbatch + " will be deleted permanently and will not be able to recover again, do you still want to continue?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
                            {
                                refCon();
                                MySqlCommand cmd15 = new MySqlCommand("SELECT * FROM `batches` WHERE `bname`='" + cbodelbatch.Text + "'", con);
                                cmd15.ExecuteNonQuery();
                                MySqlDataAdapter adp = new MySqlDataAdapter(cmd15);
                                DataTable dt = new DataTable();
                                adp.Fill(dt);
                                txttest.Text = dt.Rows[0][0].ToString();
                                refCon();
                                MySqlCommand cmd14 = new MySqlCommand("DROP TABLE IF EXISTS `" + delbatch + "`", con);
                                cmd14.ExecuteNonQuery();
                                MySqlCommand cmd12 = new MySqlCommand("DELETE FROM `batches` WHERE `bname` = '" + delbatch + "'", con);
                                cmd12.ExecuteNonQuery();
                                UpdateCBox();
                                MessageBox.Show("User defined batch(" + delbatch + ") has been deleted successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                this.pictureBox4.Visible = true;
                                this.pictureBox2.Visible = true;
                                this.pictureBox3.Visible = true;
                                refCon();
                                UpdateCBox();
                            }
                            else
                            {
                                this.pictureBox4.Visible = true;
                                this.pictureBox2.Visible = true;
                                this.pictureBox3.Visible = true;
                            }

                            UpdateCBox();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error occurred, make sure that you have a batch in that name! Error: " + ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Stop);
                            UpdateCBox();
                            textBox4.Clear();
                            textBox4.Focus();
                        }
                    }
                    if (administrator != h)
                    {
                        MessageBox.Show("Password is incorrect, please try again!", "Error occurred!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox4.Clear();
                        textBox4.Focus();
                    }
                }
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txturl_Click(object sender, EventArgs e)
        {
            txturl.SelectAll();

        }

        private void btnExit2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to exit? All unsaved data will be lost", "Confirmation!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to go back to login page? All unsaved data will be lost", "Confirmation!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Form1 login = new Form1();
                this.Hide();
                login.Show();
            }
        }

        private void txtcnewp_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnchange_Click(sender, e);
            }
        }

        private void txtCpwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                btnAdd_Click(sender, e);
            }
        }

        private void txturl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals(Convert.ToChar(13)))
            {
                picgo_Click_1(sender, e);
            }
        }

        private void picgo_Click_1(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txturl.Text) || txturl.Text.Equals("about:blank"))
            {
                MessageBox.Show("Enter a valid URL", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txturl.Focus();
                return;
            }
            else
            {
                string url = txturl.Text;
                if (!url.Contains("://"))
                    url = "http://" + url;
                webBrowser1.Navigate(url);
            }
        }

        private void picgmail_Click_1(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://mail.google.com");
        }

        private void picgoogle_Click_1(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.google.com");
        }

        private void picfb_Click_1(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.facebook.com");
        }

        private void picblogger_Click_1(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://www.blogger.com");
        }

        private void textBox4_MouseHover(object sender, EventArgs e)
        {
            label48.Visible = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
