using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace PreventativeMaintenanceReminders
{
    [Serializable]
    public class Reminder : DDITWeb.Windows.Services.Interfaces.IServiceTask
    {
        NameValueCollection settings;

        private ControlValues cv;

        private string ConnectionString { get; set; }
        public string emailTemplate { get; set; }
        public string emailFromAddress { get; set; }
        public DateTime pm_currentDate { get; set; }

        public Reminder()
        {
        }

        public object Run(NameValueCollection parms)
        {
            // set the parameters from what was passed into the application
            settings = parms;

            // Set up a return object
            object test = null;


            // retrieve the connection string from the task config
            ConnectionString = ConfigurationManager.ConnectionStrings["Manufacturing"].ConnectionString;
            SqlConnection conn;

            // Open up the database connection
            using (conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // Instantiate the control values object
                cv = new ControlValues();

                // Get the notification template name and sales order service URL from the control values table
                emailFromAddress = cv.Read(conn, "PMEmailFrom");
                emailTemplate = cv.Read(conn, "PMEmailTemplate");
            }

            // Set the default current date
            pm_currentDate = DateTime.Now;

            // Get all the emails that we need to send
            List<EmailData> emails = GetPMReminderData();

            // Send the emails and update the database in a parallel loop for better performance
            Parallel.ForEach(emails, ed =>
            {
                // Send the email
                if (Send(ed))
                {
                    // if the email succeeded then add the history
                    AddHistory(ed);
                }
            });

            // Return an object
            return test;
        }

        public int AddHistory(EmailData ed)
        {
            decimal parameterErrorCode = 0;

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                string strSQL = "INSERT INTO Mfg_EmailPMReminderHistory(TicketID, Subject, Body, ScheduleStart, ScheduleEnd, SentDate, AssignTo) values (@TicketID, @Subject, @Body, @ScheduleStart, @ScheduleEnd, @SentDate, @AssignTo)";

                using (SqlCommand cmd = new SqlCommand(strSQL, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new SqlParameter("@TicketID", ed.ID));
                    cmd.Parameters.Add(new SqlParameter("@Subject", ed.Subject));
                    cmd.Parameters.Add(new SqlParameter("@Body", ed.Issue));
                    cmd.Parameters.Add(new SqlParameter("@ScheduleStart", ed.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@ScheduleEnd", ed.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@SentDate", pm_currentDate));
                    cmd.Parameters.Add(new SqlParameter("@AssignTo", ed.Email));

                    try
                    {
                        parameterErrorCode = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        parameterErrorCode = -1;
                    }
                }
            }
            return Convert.ToInt32(parameterErrorCode);
        }

        public bool Send(EmailData ed)
        {
            bool FunctionValue = false;

            List<String> to = new List<string>();
            to.Add(ed.Email);

            Hashtable templateVars = new Hashtable();
            templateVars.Add("Today", pm_currentDate.ToString("MM/dd/yyyy HH:mm:ss"));
            templateVars.Add("ID", ed.ID.ToString());
            templateVars.Add("Issue", ed.Issue);
            templateVars.Add("Scheduled", ed.StartDate.ToString("MM/dd/yyyy hh:mm") + " to " + ed.EndDate.ToString("MM/dd/yyyy hh:mm"));

            SellarsEmail.TemplateEmail te = new SellarsEmail.TemplateEmail();
            te.From = emailFromAddress;
            te.To = to;
            te.Subject = ed.Subject;
            te.TemplateVariables = templateVars;
            te.TemplateName = emailTemplate;

            te.Send();

            FunctionValue = true;

            return FunctionValue;
        }

        public List<EmailData> GetPMReminderData()
        {
            List<EmailData> emails = new List<EmailData>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand myCommand = new SqlCommand("Mfg_PMReminder", conn))
                {
                    myCommand.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader dr = myCommand.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while(dr.Read())
                        {
                            EmailData ed = new EmailData();
                            ed.ID = Convert.ToInt64(dr["RequestedID"]);
                            ed.Email = dr["Email"].ToString();
                            ed.Issue = dr["Body"].ToString();
                            ed.Subject = dr["Subject"].ToString();
                            ed.StartDate = Convert.ToDateTime(dr["StartDate"].ToString());
                            ed.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                            emails.Add(ed);
                        }
                    }
                }
            }

            return emails;
        }
    }
}
