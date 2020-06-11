﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreventativeMaintenanceReminders
{
    public class EmailData
    {
        public long ID { get; set; }
        public string Subject { get; set; }
        public string Issue { get; set; }
        public string Email { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public EmailData()
        {
        }
    }
}
