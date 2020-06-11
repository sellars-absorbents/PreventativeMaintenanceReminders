using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventativeMaintenanceReminders
{
    public class ControlValues
    {
        public bool Error { get; set; } = false;
        public string ErrorMessage { get; set; }
        public string Text { get; set; }

        public ControlValues()
        {
        }

        public string Read(SqlConnection conn, SqlTransaction tran, string Key)
        {
            string command = "SELECT @Value = ISNULL(Value, '') from ControlValues where [Key] = @Key";
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = tran;
                cmd.Parameters.Add(new SqlParameter("@Key", Key));

                SqlParameter parmValue = new SqlParameter("@Value", SqlDbType.NVarChar, -1);
                parmValue.Direction = ParameterDirection.Output;
                parmValue.Value = null;
                cmd.Parameters.Add(parmValue);

                //Perform the update to the database
                try
                {
                    cmd.ExecuteNonQuery();
                    Text = parmValue.Value.ToString();
                }
                catch (Exception ex)
                {
                    Error = true;
                    ErrorMessage = ex.Message;
                    Text = "";
                }
            }

            return Text;
        }

        public string Read(SqlConnection conn, string Key)
        {
            string rtnValue = "";

            string command = "SELECT @Value = ISNULL(Value, '') from ControlValues where [Key] = @Key";
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@Key", Key));

                SqlParameter parmValue = new SqlParameter("@Value", SqlDbType.NVarChar, -1);
                parmValue.Direction = ParameterDirection.Output;
                parmValue.Value = null;
                cmd.Parameters.Add(parmValue);

                //Perform the update to the database
                try
                {
                    cmd.ExecuteNonQuery();
                    rtnValue = parmValue.Value.ToString();
                }
                catch (Exception ex)
                {
                    Error = true;
                    ErrorMessage = ex.Message;
                    rtnValue = "";
                }
            }

            return rtnValue;
        }

        public string ReadByte(SqlConnection conn, string Key)
        {
            string rtnValue = null;

            string command = "SELECT @Value = isnull(FileData, '') from ControlValues where [Key] = @Key";
            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@Key", Key));

                SqlParameter parmValue = new SqlParameter("@Value", SqlDbType.NVarChar, -1);
                parmValue.Direction = ParameterDirection.Output;
                parmValue.Value = null;
                cmd.Parameters.Add(parmValue);

                //Perform the update to the database
                try
                {
                    cmd.ExecuteNonQuery();
                    rtnValue = parmValue.Value.ToString();
                }
                catch (Exception ex)
                {
                    Error = true;
                    ErrorMessage = ex.Message;
                    rtnValue = "";
                }
            }

            return rtnValue;
        }

        public string WriteValue(SqlConnection conn, string Key, string Value)
        {
            string rtnValue = "";

            string command = "update ControlValues Set Value = @Value where [Key] = @Key";

            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@Key", Key));
                cmd.Parameters.Add(new SqlParameter("@Value", Value));

                //Perform the update to the database
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Error = true;
                    ErrorMessage = ex.Message;
                    rtnValue = "";
                }
            }

            return rtnValue;
        }

        public string Write(SqlConnection conn, string Key, string Value)
        {
            string rtnValue = "";

            string command = "update ControlValues Set FileData = @Value where [Key] = @Key";

            using (SqlCommand cmd = new SqlCommand(command, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@Key", Key));
                cmd.Parameters.Add(new SqlParameter("@Value", Value));

                //Perform the update to the database
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Error = true;
                    ErrorMessage = ex.Message;
                    rtnValue = "";
                }
            }

            return rtnValue;
        }

    }
}
