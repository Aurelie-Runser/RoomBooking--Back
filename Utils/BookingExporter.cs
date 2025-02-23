using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using CsvHelper;
using System.Globalization;
using RoomBookingApi.Models;

namespace RoomBookingApi.Utils
{
    public static class BookingExporter 
    {
        public static string ExportToCsv(IEnumerable<BookingDto> bookings)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            
            csv.WriteRecords(bookings.Select(b => new {
                b.Name,
                b.Description,
                RoomName = b.RoomName ?? "N/A",
                Organizer = $"{b.OrganizerFirstname} {b.OrganizerLastname}",
                b.DateFrom,
                b.DateTo,
                b.Statut,
                Guests = string.Join(";", b.GuestsName ?? Array.Empty<string>())
            }));
            
            writer.Flush();
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        public static string ExportToICalendar(IEnumerable<BookingDto> bookings)
        {
            var calendar = new Ical.Net.Calendar();

            foreach (var booking in bookings)
            {
                var calEvent = new CalendarEvent
                {
                    Summary = booking.Name,
                    Description = booking.Description ?? string.Empty,
                    Location = booking.RoomName ?? "N/A",
                    Start = new CalDateTime(booking.DateFrom),
                    End = new CalDateTime(booking.DateTo),
                    Organizer = new Organizer($"{booking.OrganizerFirstname} {booking.OrganizerLastname}")
                };

                if (booking.GuestsName?.Any() == true)
                {
                    calEvent.Attendees = booking.GuestsName.Select(g => new Attendee { Value = new Uri($"mailto:{g}") }).ToList();
                }

                calendar.Events.Add(calEvent);
            }

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }

        public static string ExportToCalendar(IEnumerable<BookingDto> bookings)
        {
            var calBuilder = new StringBuilder();
            calBuilder.AppendLine("BEGIN:VCALENDAR");
            calBuilder.AppendLine("VERSION:2.0");
            calBuilder.AppendLine("PRODID:-//RoomBookingApi//Room Booking//FR");

            foreach (var booking in bookings)
            {
                calBuilder.AppendLine("BEGIN:VEVENT");
                calBuilder.AppendLine($"SUMMARY:{booking.Name}");
                calBuilder.AppendLine($"LOCATION:{booking.RoomName}");
                calBuilder.AppendLine($"DESCRIPTION:{booking.Description ?? ""}");
                calBuilder.AppendLine($"DTSTART:{booking.DateFrom:yyyyMMddTHHmmss}");
                calBuilder.AppendLine($"DTEND:{booking.DateTo:yyyyMMddTHHmmss}");
                calBuilder.AppendLine($"ORGANIZER;CN={booking.OrganizerFirstname} {booking.OrganizerLastname}");
                
                if (booking.GuestsName?.Any() == true)
                {
                    foreach (var guest in booking.GuestsName)
                    {
                        calBuilder.AppendLine($"ATTENDEE;CN={guest}");
                    }
                }
                
                calBuilder.AppendLine("END:VEVENT");
            }

            calBuilder.AppendLine("END:VCALENDAR");
            return calBuilder.ToString();
        }
    }
} 