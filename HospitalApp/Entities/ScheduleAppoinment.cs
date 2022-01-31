using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalApp.Entities
{
    class ScheduleAppoinment
    {

        public int ScheduleId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public AppoinmentType AppoinmentType { get; set; }
        public DateTime Date { get; set; }
    }
}
