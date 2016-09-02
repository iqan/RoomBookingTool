using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using TestApi1.Models;

namespace TestApi1.Controllers
{
    public class RoomBookingController : ApiController
    {
        private MeetingRoomManagerEntities db = new MeetingRoomManagerEntities();

        // GET: api/RoomBooking
        public IQueryable<BookingNew> GetBookingNews()
        {
            return db.BookingNews;
        }

        // GET: api/RoomBooking/5
        [ResponseType(typeof(BookingNew))]
        public IHttpActionResult GetBookingNew(int id)
        {
            BookingNew bookingNew = db.BookingNews.Find(id);
            if (bookingNew == null)
            {
                return NotFound();
            }

            return Ok(bookingNew);
        }

        // POST: api/RoomBooking
        [ResponseType(typeof(BookingNew))]
        public IHttpActionResult PostBookingNew(BookingNew bookingNew)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TimeZoneInfo indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaZone);

            bookingNew.BookingTime = indianTime;

            var temp = db.BookingNews.Where(x => x.StartDate == bookingNew.StartDate && x.RoomNumber == bookingNew.RoomNumber).ToList();
            TimeRange range = new TimeRange(bookingNew.StartTime, bookingNew.EndTime);
            
            string ErrorInvalidDateTime = "Select valid date and/or times";
            string ErrorRestriction = "You are not allowed to book room for more than 2 hours.";

            foreach (var item in temp)
            {
                string ErrorClash = "Booking timings are clashing with a meeting. Booking Id: " + item.BookingId + " | Start Time:" + item.StartTime + " | End Time:" + item.EndTime + " | Booked By:" + item.EmpId;
                TimeRange rangeItem = new TimeRange(item.StartTime, item.EndTime);
                if (rangeItem.Clashes(range, true))
                    return BadRequest(ErrorClash);
            }
            if ((bookingNew.StartTime < indianTime.TimeOfDay || bookingNew.EndTime <= indianTime.TimeOfDay) && bookingNew.StartDate <= indianTime)
                return BadRequest(ErrorInvalidDateTime);
            if (bookingNew.EndTime > bookingNew.StartTime.Add(new TimeSpan(2, 0, 0)))
                return BadRequest(ErrorRestriction);

            db.BookingNews.Add(bookingNew);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bookingNew.BookingId }, bookingNew);
        }

        // DELETE: api/RoomBooking/5
        [ResponseType(typeof(BookingNew))]
        public IHttpActionResult DeleteBookingNew(int id)
        {
            BookingNew bookingNew = db.BookingNews.Find(id);
            if (bookingNew == null)
            {
                return NotFound();
            }

            db.BookingNews.Remove(bookingNew);
            db.SaveChanges();
            
            return Ok(bookingNew);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookingNewExists(int id)
        {
            return db.BookingNews.Count(e => e.BookingId == id) > 0;
        }

        public interface IRange<T>
        {
            T Start { get; }
            T End { get; }
            bool Includes(T value);
            bool Includes(IRange<T> range);
            bool Clashes(TimeRange other, bool inclusive);
        }
        public class TimeRange : IRange<TimeSpan>
        {
            public TimeRange(TimeSpan start, TimeSpan end)
            {
                Start = start;
                End = end;
            }

        #region TimeCalculations

            public TimeSpan Start { get; private set; }
            public TimeSpan End { get; private set; }

            public bool Includes(TimeSpan value)
            {
                return (Start <= value) && (value <= End);
            }

            public bool Includes(IRange<TimeSpan> range)
            {
                if (Start <= range.Start)
                {

                }
                return (Start <= range.Start) && (range.End <= End);
            }

            public bool Clashes(TimeRange other, bool inclusive)
            {
                if (inclusive)
                {
                    return (other.Start <= Start && other.End >= End) ||
                        (other.Start < Start && other.End >= Start) ||
                        (other.End > End && other.Start <= End) ||
                        (other.Start >= Start && other.End <= End);
                }
                else
                {
                    return (other.Start < Start && other.End > End) ||
                        (other.Start < Start && other.End > Start) ||
                        (other.End > End && other.Start < End) ||
                        (other.Start >= Start && other.End <= End);
                }
            }
            //usage
            //DateRange range = new DateRange(startDate, endDate);
            //range.Includes(date);
        #endregion
        }
    }
}