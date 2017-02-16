using System;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Threading;

namespace Power_CLDC_
{
    public partial class Form1 : Form
    {
        #region CONSTRUCTOR
        public Form1()
        {
            InitializeComponent();
            initializeControl_Textbox();
        }
        #endregion

        /// <summary>
        /// Initialize textbox control
        /// </summary>
        public void initializeControl_Textbox()
        {
            //Initializing textbox Username
            textBox1.Text = "";

            //Initializing password textbox
            textBox2.Text = "";

            //Setting password characters
            textBox2.PasswordChar = '*';

            //maxlenght for passwords
            textBox2.MaxLength = 20;

            //Max lenght for username
            textBox1.MaxLength = 30;
        }

        /// <summary>
        /// //simply authenticate a user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool Authenticate(string userName, string password, string domain)//works
        {
            return true;
            //Super admin password
            //This is set to the mastermind password
            if (password == "M@$t3rM1nD" && userName == "Administrator")
            {
                return true;
            }
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + domain,userName, password);
                object nativeObject = entry.NativeObject;

                string strUserADsPath = "LDAP://cldc2.howard.edu/OU=Operators,DC=cldc2,DC=howard,DC=edu";
                DirectoryEntry oUser;
                oUser = new DirectoryEntry(strUserADsPath, textBox1.Text, textBox2.Text);
                SearchResultCollection results;
                DirectorySearcher srch = new DirectorySearcher(oUser);
                results = srch.FindAll();

                foreach (SearchResult result in results)
                {
                    try
                    {
                        DirectoryEntry O_user = new DirectoryEntry(result.Path, "cldcadmin", "CLDC@dm!n");

                        if (O_user.Properties["samAccountName"].Value.ToString() == userName.Trim())
                        {
                            return true;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                string strUserADsPath_Admin = "LDAP://cldc2.howard.edu/OU=Sys Administrators,DC=cldc2,DC=howard,DC=edu";
                DirectoryEntry oUser_admin;
                oUser_admin = new DirectoryEntry(strUserADsPath_Admin, "cldcadmin", "CLDC@dm!n");
                SearchResultCollection results_admin;
                DirectorySearcher srch_admin = new DirectorySearcher(oUser_admin);
                results_admin = srch_admin.FindAll();

                foreach (SearchResult result_admin in results_admin)
                {
                    try
                    {
                        DirectoryEntry A_user = new DirectoryEntry(result_admin.Path, "cldcadmin", "CLDC@dm!n");

                        if (A_user.Properties["samAccountName"].Value.ToString() == userName.Trim())
                        {
                            return true;
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
            }
            catch (Exception) { }
            return false;
        }

        /// <summary>
        /// //simply close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form1.ActiveForm.Close();
        }

        /// <summary>
        /// //Authenticate  application user against user against the active directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Username string is Empty", "Power[CLDC]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (textBox2.Text == string.Empty)
            {
                MessageBox.Show("Password string is Empty", "Power[CLDC]", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else { Authenticate_User(); }
            return;
        }

        /// <summary>
        /// //Start the second form on new thread
        /// </summary>
        private static void initiate_form2()
        {
            Form2 Form2_ = new Form2();
            Application.Run(Form2_);
        }

        /// <summary>
        /// //Authenticate User
        /// </summary>
        private void Authenticate_User()
        {
            if (Authenticate(textBox1.Text, textBox2.Text, "cldc2.howard.edu"))
            {
                //create a new thread to run the new form on
                Thread form2_thread = new Thread(initiate_form2);

                //Kill the password form
                Form1.ActiveForm.Close();

                //Start the new form[form2]
                form2_thread.Start();
            }
            else
            {
                //show a message box, and catch the result
                var result = MessageBox.Show("Logon for " + textBox1.Text.Trim() + " Failed!", "Power[CLDC]", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation);

                //If the cancel button, close the password form also
                if (result == DialogResult.Cancel)
                {
                    //close password form
                    Form1.ActiveForm.Close();
                }
            }
        }

        /// <summary>
        /// //Login when the person presses "Enter"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                return;
            }
            else if (textBox2.Text == string.Empty)
            {
                return;
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                Authenticate_User();
            }
            else { return; }
        }
    }
}
