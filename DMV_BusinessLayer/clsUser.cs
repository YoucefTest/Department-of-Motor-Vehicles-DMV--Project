using System;
using System.Data;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DVLD_DataAccess;

namespace DVLD_Buisness
{
    public  class clsUser
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int UserID { set; get; }
        public int PersonID { set; get; }
        public clsPerson PersonInfo;
        public string UserName { set; get; }
        public string Password { set; get; }
        public byte []PasswordHash { set; get; }
        public bool IsActive { set; get; }
        public byte[] Salt { set; get; }    
     
        public clsUser()

        {
            //this.UserID = -1;
            //this.UserName = "";
            //this.Password = "";
            //this.IsActive = true;
            Mode = enMode.AddNew;
        }
        private clsUser(int UserID, int PersonID, string Username, byte []Password,
      bool IsActive , byte[] Salte)

        {
            this.UserID = UserID;
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.UserName = Username;
            this.PasswordHash = Password;
            this.IsActive = IsActive;
            this.Salt = Salte;

            Mode = enMode.Update;
        }
        private clsUser(int UserID, int PersonID, string Username,string Password,
            bool IsActive)

        {
            this.UserID = UserID; 
            this.PersonID = PersonID;
            this.PersonInfo = clsPerson.Find(PersonID);
            this.UserName = Username;
            this.Password = Password;
            this.IsActive = IsActive;

            Mode = enMode.Update;
        }
        //in here User management stays in clsUsers (business layer).
        // amybe i need anohter one for for retrieve 
        //and another one for verifying
        public static void RegisterUser(string username, string password)
        {
            //string salt = clsCryptoService.GenerateSalt();
            //string hash = clsCryptoService.HashPasswordAndSalt(password, salt);

            //clsUser.AddUser(username, hash, salt);
        }
        private bool _AddNewUser()
        {
            // inside this method i called AddNewUser and pass these variables as an argument   to it using this this keyword and return newly UserID and return true if !-1 otherwise flase

            this.UserID = clsUserData.AddNewUser(this.PersonID, this.UserName,
                this.PasswordHash, this.IsActive , this.Salt);

            return (this.UserID != -1);
        }
        private bool _UpdateUser()
        {
            //call DataAccess Layer 

            return clsUserData.UpdateUser(this.UserID, this.PersonID, this.UserName,
                this.PasswordHash, this.IsActive, this.Salt);
            return true;
        }
        public static clsUser FindByUserID(int UserID)
        {
            int PersonID = -1;
            string UserName = ""; byte[] Password = null ;
            bool IsActive = false; byte[]Salt = null;

            bool IsFound = clsUserData.GetUserInfoByUserID
                                ( UserID,ref PersonID, ref UserName,ref Password,ref IsActive, ref Salt);

            if (IsFound)
                //we return new object of that User with the right data
                return new clsUser(UserID,PersonID,UserName,Password,IsActive ,Salt);
            else
                return null;
        }
        public static clsUser FindByPersonID(int PersonID)
        {
            int UserID = -1;
            string UserName = "", Password = "";
            bool IsActive = false;

            bool IsFound = clsUserData.GetUserInfoByPersonID
                                (PersonID, ref UserID, ref UserName, ref Password, ref IsActive);

            if (IsFound)
                //we return new object of that User with the right data
                return new clsUser(UserID, UserID, UserName, Password, IsActive);
            else
                return null;
        }
        public static   clsUser FindUserrByUsername(string UserName)
        {
            int UserID = -1;
            int PersonID=-1;
            byte[] Password =new byte[64] ;
            bool IsActive = false;
            byte[] Salt = new byte[32] ;

            bool IsFound =  clsUserData.FindByUsername
                                (UserName , ref  Password,ref UserID,ref PersonID, ref IsActive , ref Salt );

            if (IsFound)
                //we return new object of that User with the right data
                return new clsUser(UserID, PersonID, UserName, Password, IsActive, Salt); 
            else
                return null;
        }

        public bool Save()
        { //    
                byte[] salt = clsCryptoService.GenerateSalt();

                        // 2. Hash the password with salt
                        byte[] hashedPassword = clsCryptoService.HashPasswordAndSalt(this.Password, salt);

                        // 3. Store salt and hash in _User object
                        this.PasswordHash = hashedPassword;
                        this.Salt = salt;
            switch (Mode)
            {
                case enMode.AddNew:
                    
                    if (_AddNewUser())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateUser();

            }

            return false;
        }

        public static DataTable GetAllUsers()
        {
            return clsUserData.GetAllUsers();
        }

        public static bool DeleteUser(int UserID)
        {
            return clsUserData.DeleteUser(UserID); 
        }

        public static bool isUserExist(int UserID)
        {
           return clsUserData.IsUserExist(UserID);
        }

        public static bool isUserExist(string UserName)
        {
            return clsUserData.IsUserExist(UserName);
        }

        public static bool isUserExistForPersonID(int PersonID)
        {
            return clsUserData.IsUserExistForPersonID(PersonID);
        }


    }
}
