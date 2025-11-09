using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsPersonData
    {   
        public static async Task < DataTable> GetAllPeople()
        {
            DataTable dt = new DataTable();

            string query = @"SELECT People.PersonID, People.NationalNo,
                     People.FirstName, People.SecondName, People.ThirdName, People.LastName,
                     People.DateOfBirth, People.Gendor,  
                     CASE WHEN People.Gendor = 0 THEN 'Male' ELSE 'Female' END as GendorCaption,
                     People.Address, People.Phone, People.Email, 
                     People.NationalityCountryID, Countries.CountryName, People.ImagePath
                     FROM People 
                     INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID
                     ORDER BY People.FirstName";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                  
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                            }
                        }
                    } //Release unmanaged resources.
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }

            return dt;
        }
        public static bool GetPersonInfoByID(int PersonID, ref string FirstName, ref string SecondName,
    ref string ThirdName, ref string LastName, ref string NationalNo, ref DateTime DateOfBirth,
    ref short Gendor, ref string Address, ref string Phone, ref string Email,
    ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "SELECT * FROM People WHERE PersonID = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // The record was found
                                isFound = true;

                                FirstName = (string)reader["FirstName"];
                                SecondName = (string)reader["SecondName"];

                                //ThirdName: allows null in database so we should handle null
                                if (reader["ThirdName"] != DBNull.Value)
                                {
                                    ThirdName = (string)reader["ThirdName"];
                                }
                                else
                                {
                                    ThirdName = "";
                                }

                                LastName = (string)reader["LastName"];
                                NationalNo = (string)reader["NationalNo"];
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Gendor = (byte)reader["Gendor"];
                                Address = (string)reader["Address"];
                                Phone = (string)reader["Phone"];

                                //Email: allows null in database so we should handle null
                                if (reader["Email"] != DBNull.Value)
                                {
                                    Email = (string)reader["Email"];
                                }
                                else
                                {
                                    Email = "";
                                }

                                NationalityCountryID = (int)reader["NationalityCountryID"];

                                //ImagePath: allows null in database so we should handle null
                                if (reader["ImagePath"] != DBNull.Value)
                                {
                                    ImagePath = (string)reader["ImagePath"];
                                }
                                else
                                {
                                    ImagePath = "";
                                }
                            }
                            else
                            {
                                // The record was not found
                                isFound = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }

            return isFound;
        }

        public static bool GetPersonInfoByNationalNo(string NationalNo, ref int PersonID, ref string FirstName, ref string SecondName,
        ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
         ref short Gendor, ref string Address, ref string Phone, ref string Email,
         ref int NationalityCountryID, ref string ImagePath)
        {
            bool isFound = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {

                    string query = "SELECT * FROM People WHERE NationalNo = @NationalNo";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {

                        command.Parameters.AddWithValue("@NationalNo", NationalNo);


                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                // The record was found
                                isFound = true;

                                PersonID = (int)reader["PersonID"];
                                FirstName = (string)reader["FirstName"];
                                SecondName = (string)reader["SecondName"];
                                //ThirdName: allows null in database so we should handle null
                                if (reader["ThirdName"] != DBNull.Value)
                                {
                                    ThirdName = (string)reader["ThirdName"];
                                }
                                else
                                {
                                    ThirdName = "";
                                }
                                LastName = (string)reader["LastName"];
                                DateOfBirth = (DateTime)reader["DateOfBirth"];
                                Gendor = (byte)reader["Gendor"];
                                Address = (string)reader["Address"];
                                Phone = (string)reader["Phone"];
                                //Email: allows null in database so we should handle null
                                if (reader["Email"] != DBNull.Value)
                                {
                                    Email = (string)reader["Email"];
                                }
                                else
                                {
                                    Email = "";
                                }
                                NationalityCountryID = (int)reader["NationalityCountryID"];
                                //ImagePath: allows null in database so we should handle null
                                if (reader["ImagePath"] != DBNull.Value)
                                {
                                    ImagePath = (string)reader["ImagePath"];
                                }
                                else
                                {
                                    ImagePath = "";
                                }

                            }
                            else
                            {
                                // The record was not found
                                isFound = false;
                            }


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            return isFound;
        }

        public static async Task <int> AddNewPerson(string FirstName, string SecondName, string ThirdName, string LastName,
    string NationalNo, DateTime DateOfBirth, short Gendor, string Address, string Phone, string Email,
    int NationalityCountryID, string ImagePath)
        {
            // This function will return the new person ID if succeeded, and -1 if not.
            int PersonID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = @"INSERT INTO People 
                (FirstName, SecondName, ThirdName, LastName, NationalNo, DateOfBirth, Gendor, Address, Phone, Email, NationalityCountryID, ImagePath)
                VALUES 
                (@FirstName, @SecondName, @ThirdName, @LastName, @NationalNo, @DateOfBirth, @Gendor, @Address, @Phone, @Email, @NationalityCountryID, @ImagePath);
                SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@SecondName", SecondName);
                        command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        command.Parameters.AddWithValue("@Gendor", Gendor);
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@Phone", Phone);
                        command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email);
                        command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                        command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);

                        await connection.OpenAsync();
                      
                        object result = await command.ExecuteScalarAsync();

                        if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        {
                            PersonID = insertedID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
            }

            return PersonID;
        }

        public static async Task<bool> UpdatePerson(int PersonID, string FirstName, string SecondName,
     string ThirdName, string LastName, string NationalNo, DateTime DateOfBirth,
     short Gendor, string Address, string Phone, string Email,
     int NationalityCountryID, string ImagePath)
        { 
            int rowsAffected = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                             string query = @"UPDATE People  
                                 SET FirstName = @FirstName,
                                 SecondName    = @SecondName,
                                 ThirdName     = @ThirdName,
                                 LastName      = @LastName, 
                                 NationalNo    = @NationalNo,
                                 DateOfBirth   = @DateOfBirth,
                                 Gendor        = @Gendor,
                                 Address       = @Address,  
                                 Phone         = @Phone,
                                 Email         = @Email, 
                                 NationalityCountryID = @NationalityCountryID,
                                 ImagePath     = @ImagePath
                             WHERE PersonID    = @PersonID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PersonID", PersonID);
                        command.Parameters.AddWithValue("@FirstName", FirstName);
                        command.Parameters.AddWithValue("@SecondName", SecondName);
                        command.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName);
                        command.Parameters.AddWithValue("@LastName", LastName);
                        command.Parameters.AddWithValue("@NationalNo", NationalNo);
                        command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        command.Parameters.AddWithValue("@Gendor", Gendor);
                        command.Parameters.AddWithValue("@Address", Address);
                        command.Parameters.AddWithValue("@Phone", Phone);
                        command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email);
                        command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
                        command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);

                        await connection.OpenAsync();
                      
                        rowsAffected  =  await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            return (rowsAffected > 0);
        }

     
        public static async Task< bool> DeletePerson(int PersonID)
        {
            int rowsAffected = 0;

            string query = @"DELETE FROM People WHERE PersonID = @PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString)) 
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                  await connection.OpenAsync();
   
                    rowsAffected = await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }

            return (rowsAffected > 0);
        }
        public static async Task <bool> IsPersonExist(int PersonID)
        {
            bool isFound = false;

            string query = "SELECT 1 FROM People WHERE PersonID = @PersonID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", PersonID);
                    await connection.OpenAsync() ;
                   
                    using (SqlDataReader reader =await command.ExecuteReaderAsync())
                    {
                        isFound = reader.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }

            return isFound;
        }
        public static async Task <bool> IsPersonExist(string NationalNo)
        {
            bool isFound = false;

            string query = "SELECT 1 FROM People WHERE NationalNo = @NationalNo";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalNo", NationalNo);
                  await  connection.OpenAsync();
                   
                    using (SqlDataReader reader =await command.ExecuteReaderAsync())
                    {
                        isFound = reader.HasRows;
                    }
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }

            return isFound;
        }

    }
}