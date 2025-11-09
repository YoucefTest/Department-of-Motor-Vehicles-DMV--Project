using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DVLD_DataAccess
{
    public static class clsDataAccessSettings
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DMVConnection"].ConnectionString;
            }
        }
    }

}


























    //get only → you can read the value, but you cannot assign a new value to it in code.
    //    "Server=.;Database=DVLD; User id = sa ;Password=sa123456";




