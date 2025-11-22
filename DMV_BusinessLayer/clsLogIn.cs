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
                    
                    
                    fileLogger.log("Failed login: username not found") ;
                    return new clsLogIn("Wrong Email Or Password", false); // Returns a friendly message to the UI
                }
         .
                if (!user.IsActive)
                {// record the error message in a log file for tracking inactive account login attempts for security.”.”
                    fileLogger.log($"Inactive account login attempt for '{txtUserName}'");
                    return new clsLogIn("Your account is not Active, Contact Admin. , InActive Account", false);
                }

                byte[] NewHashedPassword = clsCryptoService.HashPasswordAndSalt(txtPassword, user.Salt);

                bool found = clsCryptoService.VerifyPassword(NewHashedPassword, user.PasswordHash);
                if (!found)
                {
      
                    return new clsLogIn($"Failed login: wrong password for '{txtUserName}'", false);
                }
                else

                    if (chkRememberMe)
                {
                    //symmetric encryption
                    string encryptedPassword = clsCryptoService.Encrypt(txtPassword);

                    //  in web/mob app use token
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
