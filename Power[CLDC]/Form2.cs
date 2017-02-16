using System;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Threading;

namespace Power_CLDC_
{
    public partial class Form2 : Form
    {
        #region CONSTRUCTOR
        public Form2()
        {
            InitializeComponent();
            InitializePbox();
        } 
        #endregion

        /// <summary>
        /// //Initialize the textboxes before program runs
        /// </summary>
        private void InitializePbox()
        {
            textBox5.PasswordChar = '*';
            textBox6.PasswordChar = '*';

            //setting max character length
            textBox1.MaxLength = 20;
            textBox2.MaxLength = 3;
            textBox3.MaxLength = 20;
            textBox6.MaxLength = 20;
            textBox7.MaxLength = 20;
            return;
        }
        
        /// <summary>
        /// //Update the username as firstname is changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text.Trim();
                textBox2.Text.Trim();
                textBox3.Text.Trim();

                if (textBox1.Text != string.Empty)
                    textBox4.Text = ("" + textBox1.Text.Trim()[0] + textBox3.Text.Trim()).ToLower();
                else
                    textBox4.Text = ("" + textBox3.Text.Trim()).ToLower();
            }catch(Exception)
            {}
            return;
        }
      
        /// <summary>
        /// //Update the usewrname as lastname is changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if(textBox3.Text == string.Empty && textBox1.Text == string.Empty)
            {
                textBox4.Text = string.Empty;
            }
            try
            {
                textBox1.Text.Trim();
                textBox2.Text.Trim();
                textBox3.Text.Trim();

                textBox4.Text = ("" + textBox1.Text.Trim()[0] + textBox3.Text.Trim()).ToLower();
            }catch(Exception)
            {}
            return;
        }
       
        /// <summary>
        /// //when the "Create button is clicked"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Add_User_Ex();
        }
       
        /// <summary>
        /// //Enumeration  containing user creation information
        /// </summary>
        public enum User_Properties
        {
            SCRIPT = 0x0001,
            ACCOUNTDISABLE = 0x0002,
            HOMEDIR_REQUIRED = 0x0008,
            LOCKOUT = 0x0010,
            PASSWD_NOTREQD = 0x0020,
            PASSWD_CANT_CHANGE = 0x0040,
            ENCRYPTED_TEXT_PWD_ALLOWED = 0x0080,
            TEMP_DUPLICATE_ACCOUNT = 0x0100,
            NORMAL_ACCOUNT = 0x0200,
            INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
            WORKSTATION_TRUST_ACCOUNT = 0x1000,
            SERVER_TRUST_ACCOUNT = 0x2000,
            DONT_EXPIRE_PASSWORD = 0x10000,
            MNS_LOGON_ACCOUNT = 0x20000,
            SMARTCARD_REQUIRED = 0x40000,
            TRUSTED_FOR_DELEGATION = 0x80000,
            NOT_DELEGATED = 0x100000,
            USE_DES_KEY_ONLY = 0x200000,
            DONT_REQ_PREAUTH = 0x400000,
            PASSWORD_EXPIRED = 0x800000,
            TRUSTED_TO_AUTH_FOR_DELEGATION = 0x1000000,
        }
      
        /// <summary>
        /// //The Add user method
        /// </summary>
        private void Add_User()
        {
            string oGUID = string.Empty;
            try
            {
                string connectionPrefix = "LDAP://cldc2.howard.edu/OU=CLDC Users,DC=cldc2,DC=howard,DC=edu";
                DirectoryEntry dirEntry = new DirectoryEntry(connectionPrefix);
                dirEntry.AuthenticationType = AuthenticationTypes.Secure;
                dirEntry.Username = "cldcadmin";
                dirEntry.Password = "CLDC@dm!n";

                DirectoryEntry newUser = dirEntry.Children.Add
                    ("CN=" + (textBox1.Text + " " + textBox3.Text), "user");

                newUser.Properties["samAccountName"].Value = textBox4.Text;
                newUser.Properties["givenName"].Value = textBox1.Text;
                newUser.Properties["sn"].Value = textBox3.Text;

                //Allow for Initialess profiles
                if (textBox2.Text != string.Empty)
                {
                    newUser.Properties["initials"].Value = textBox2.Text;
                }

                newUser.Properties["displayName"].Value = (textBox1.Text + " " + textBox3.Text);
                newUser.Properties["UserPrincipalName"].Value = textBox4.Text + "@cldc2.howard.edu";
                newUser.Properties["DistinguishedName"].Value = (textBox1.Text + " " + textBox3.Text);
                newUser.CommitChanges();

                newUser.Invoke("SetPassword", new object[] { textBox5.Text });
                newUser.CommitChanges();// Create a normal account and enable it - ADS_UF_NORMAL_ACCOUNT

                if (checkBox1.Checked)
                {
                    //Change Password at next logon
                    newUser.Properties["passwordExpired"][0] = 1;
                    newUser.CommitChanges();
                }

                dirEntry.Close();
                newUser.Close();

                ////Clear all the text boxes after creating user
                textBox1.Text = string.Empty;
                textBox2.Text = string.Empty;
                textBox3.Text = string.Empty;
                textBox4.Text = string.Empty;
                textBox5.Text = string.Empty;
                textBox6.Text = string.Empty;
                ////============================================

                MessageBox.Show(textBox4.Text + " created successfully!", "Power[CLDC]", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message.ToString(), "Power[CLDC]", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
      
        /// <summary>
        /// //The add user method helper
        /// </summary>
        private void Add_User_Ex()
        {
            if (textBox1.Text == string.Empty || textBox3.Text == string.Empty || textBox5.Text == string.Empty || textBox6.Text == string.Empty)
            {
                MessageBox.Show("A textbox is Empty", "Power[CLDC]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (textBox5.Text == textBox6.Text)
            {
                Add_User();
                return;
            }
            else
            {
                MessageBox.Show("Passwords don't match", "Power[CLDC]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox5.Text = string.Empty;
                textBox6.Text = string.Empty;
                return;
            }
        }
       
        /// <summary>
        /// //When the enter button is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                Add_User_Ex();
            }
        }

        /// <summary>
        /// Refresh button Functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string strUserADsPath = "LDAP://cldc2.howard.edu/OU=Operators,DC=cldc2,DC=howard,DC=edu";
            DirectoryEntry oUser;
            oUser = new DirectoryEntry(strUserADsPath, "cldcadmin", "CLDC@dm!n");
            SearchResultCollection results;
            DirectorySearcher srch = new DirectorySearcher(oUser);
            results = srch.FindAll();

            foreach (SearchResult result in results)
            {
                DirectoryEntry oUser_ = new DirectoryEntry(result.Path, "cldcadmin", "CLDC@dm!n");
                string Fname = oUser_.Properties["givenName"].Value.ToString();
                string Lname = oUser_.Properties["sn"].Value.ToString();
                string Uname = oUser_.Properties["samAccountName"].Value.ToString();
            }
        }
    }
}
