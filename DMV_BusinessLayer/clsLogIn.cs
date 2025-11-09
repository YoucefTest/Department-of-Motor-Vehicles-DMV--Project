using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using CredentialManagement;
namespace DVLD_Buisness
{
    public  class clsLogIn
    {     
        public bool IsLoggedIn { get; set; }
        public string Message { get; set; }
        public clsLogIn(string Message , bool isLoggedIn)
        {
            this.Message = Message;
            this.IsLoggedIn = isLoggedIn;
        }
        public static clsLogIn Log_In(string txtUserName , string txtPassword , bool chkRememberMe) 
        {
            //send log messages to a file
                var fileLogger = new clsLogger(clsLogMethods.LogToFile);
 try
            { 
                clsUser user = clsUser.FindUserrByUsername(txtUserName);

                if (user == null) // If no user is not found, 
                {
                    
                    // record the -LOG- error message iNTO  file for tracking failed login attempts.”
                    fileLogger.log("Failed login: username not found") ;
                    return new clsLogIn("Wrong Email Or Password", false); // Returns a friendly message to the UI
                }
                // If the user is found but their account is not active , I also stop the process.
                if (!user.IsActive)
                {// record the error message in a log file for tracking inactive account login attempts for security.”.”
                    fileLogger.log($"Inactive account login attempt for '{txtUserName}'");
                    return new clsLogIn("Your account is not Active, Contact Admin. , InActive Account", false);
                }

                byte[] NewHashedPassword = clsCryptoService.HashPasswordAndSalt(txtPassword, user.Salt);

                bool found = clsCryptoService.VerifyPassword(NewHashedPassword, user.PasswordHash);
                if (!found)
                {
                   // we can log , and in here we returned freindly error message to the  file 
                    return new clsLogIn($"Failed login: wrong password for '{txtUserName}'", false);
                }
                else

                    if (chkRememberMe)
                {
                    //symmetric encryption
                    string encryptedPassword = clsCryptoService.Encrypt(txtPassword);

                    //      I encrypt the password and store it securely in Windows Credential Manager.. in web/mob app use token
                    var cred = new CredentialManagement.Credential
                    {
                        Target = "MyDMV_RememberMe",
                        Username = txtUserName,
                        Password = encryptedPassword,
                        PersistanceType = CredentialManagement.PersistanceType.LocalComputer
                    };
                    cred.Save();

                }
                else
                {
                    // If unchecked, I remove any previously stored credentials. from Windows Credential Manager,
                    var oldCred = new CredentialManagement.Credential { Target = "MyDMV_RememberMe" };
                    oldCred.Delete();
                }


            }
            catch (Exception ex)
            {

                string errorMessage = $"Unexpected error during login for '{txtUserName}': {ex.Message}";

                fileLogger.log(errorMessage);
                return new clsLogIn(errorMessage, false);
            }
            return new clsLogIn($"Successful login for '{txtUserName}'", true); ;
        }
       
    }
   
}
