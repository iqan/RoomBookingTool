//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestApi1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BookingNew
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
