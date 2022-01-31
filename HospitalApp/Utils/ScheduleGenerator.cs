using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalApp.Utils
{
    class ScheduleGenerator
    {
        private DateTime _startDate;
        private DateTime _endDate;
        private List<Entities.DoctorSchedule> _doctorSchedule;

        public ScheduleGenerator(DateTime startDate, DateTime endDate,
            List<Entities.DoctorSchedule> schedule)
        {
            _startDate = startDate;
            _endDate = endDate;
            _doctorSchedule = schedule.Where(p => p.Date >= _startDate.Date && p.Date <= _endDate.Date).ToList();

        }

       public List<Entities.ScheduleHeader> GenerateHeader()
        {
            var result = new List<Entities.ScheduleHeader>();
            var startDate = _startDate;
            while(startDate.Date < _endDate.Date)
            {
                result.Add(new Entities.ScheduleHeader
                {
                    Date = startDate.Date
                });
                startDate = startDate.AddDays(1);
            }


            return result;
        }              

        public List<List<Entities.ScheduleAppoinment>> GenerateAppoinments(List<Entities.ScheduleHeader> headers)
        {
            var result = new List<List<Entities.ScheduleAppoinment>>();
            if(_doctorSchedule.Count() > 0)
            {
                var minStartTime = _doctorSchedule.Min(p => p.StartTime);
                var maxEndTime = _doctorSchedule.Max(p => p.EndTime);

                var startTime = minStartTime;
                while (startTime <= maxEndTime)
                {
                    var appointmentsPerInterval = new List<Entities.ScheduleAppoinment>();

                    foreach(var header in headers)
                    {
                        var currentSchedule = _doctorSchedule.FirstOrDefault(p => p.Date == header.Date);

                        var scheduleAppointment = new Entities.ScheduleAppoinment   
                        {
                            ScheduleId = currentSchedule?.Id ?? -1,
                            StartTime = startTime,
                            EndTime = startTime.Add(TimeSpan.FromMinutes(30))
                        };
                        if(currentSchedule != null)
                        {
                            var busyAppointment = currentSchedule.Appointments.FirstOrDefault(p => p.StartTime == startTime);
                            if(busyAppointment !=null)
                            {
                                scheduleAppointment.AppoinmentType = Entities.AppoinmentType.Busy;
                            }
                            else
                            {
                                if(startTime < currentSchedule.StartTime || startTime >= currentSchedule.EndTime)
                                {
                                    scheduleAppointment.AppoinmentType = Entities.AppoinmentType.DayOff;
                                }
                                else
                                {
                                    scheduleAppointment.AppoinmentType = Entities.AppoinmentType.Free;
                                }
                                
                            }
                        }
                        else
                        {
                            scheduleAppointment.AppoinmentType = Entities.AppoinmentType.DayOff;
                        }


                        appointmentsPerInterval.Add(scheduleAppointment);
                    }
                    result.Add(appointmentsPerInterval);
                    startTime = startTime.Add(TimeSpan.FromMinutes(30));
                }
            }
            return result;
        }
    }
}
