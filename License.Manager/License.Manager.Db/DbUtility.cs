using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace License.Manager.Db
{
    public class DbUtility
    {
        
        private readonly string _connectionString;
        public DbUtility(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool SaveLicense(LicenseModel license)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sqlCmd = new SqlCommand(@"[dsfastaddydbuser].[SaveLicense]", connection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("licId", SqlDbType.NVarChar).Value = license.LicId;
                    sqlCmd.Parameters.Add("publicKey", SqlDbType.NVarChar).Value = license.PublicKey;
                    sqlCmd.Parameters.Add("licFile", SqlDbType.VarBinary).Value = license.LicFile;
                    connection.Open();
                    sqlCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public LicenseModel GetLicense(string licKey)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var sqlCmd = new SqlCommand(@"[dsfastaddydbuser].[GetLicense]", connection);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("licId", SqlDbType.NVarChar).Value = licKey;
                    connection.Open();
                    using (var reader = sqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var license = new LicenseModel
                            {
                                LicId = reader["LicId"] == null ? string.Empty : reader["LicId"].ToString(),
                                PublicKey = reader["PublicKey"] == null ? string.Empty : reader["PublicKey"].ToString(),
                                LicFile = reader["LicFile"] == null ? new byte[1] : (byte[]) reader["LicFile"],
                            };
                            return license;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
