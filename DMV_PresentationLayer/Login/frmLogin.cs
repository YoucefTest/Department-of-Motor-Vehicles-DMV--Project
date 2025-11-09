using DVLD.Classes;
using DVLD_Buisness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CredentialManagement;


namespace DVLD.Login
{
    public partial class frmLogin : Form
    {  
        public frmLogin()
        {
            InitializeComponent();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //---------------------------------------------------------------------------------------------
        private void btnLogin_Click(object sender, EventArgs e)
        {

          clsLogIn LogIn = clsLogIn.Log_In(txtUserName.Text.Trim(),txtPassword.Text.Trim() ,chkRememberMe.Checked);

            try {
               if( LogIn.IsLoggedIn==false)
                {
                       
                    MessageBox.Show(LogIn.Message , "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);   txtUserName.Focus();
                    return;
                }
                // If the user is found but their account is not active , I also stop the process.
           
                this.Hide();
                frmMain frm = new frmMain(this);
                frm.ShowDialog();
            } 
            catch (Exception ex) 
            {
                
        MessageBox.Show("An unexpected error occurred. Please try again.");
    }
            }
              

        

        private void frmLogin_Load(object sender, EventArgs e)
        {
            //create an object from the Windows Credential Manager library.
 //Target = "MyDMV_RememberMe" is the “key name” where your app looks for saved credentials.

             var cred = new CredentialManagement.Credential { Target = "MyDMV_RememberMe" };
            if (cred.Load()) //checks if Windows Credential Manager has stored values for that Target.
            {
                // Pre-fill the UI element
                txtUserName.Text  =  cred.Username;
                txtPassword.Text  =  clsCryptoService.Decrypt(cred.Password);
                chkRememberMe.Checked = true;

            
            }
        }
    }
}
