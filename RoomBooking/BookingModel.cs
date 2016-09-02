using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomBooking
{
    class BookingModel
    {
        public int BookingId { get; set; }
        public string RoomNumber { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.TimeSpan StartTime { get; set; }
        public System.TimeSpan EndTime { get; set; }
        public string EmpName { get; set; }
        public string EmpId { get; set; }
        public System.DateTime BookingTime { get; set; }
        public string Subject { get; set; }
    }
}
