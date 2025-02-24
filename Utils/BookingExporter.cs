using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using CsvHelper;
using System.Globalization;
using RoomBookingApi.Models;
using OfficeOpenXml;
using System.Drawing;

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
                b.Day,
                b.TimeFrom,
                b.TimeTo,
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
                TimeOnly timeFrom = TimeOnly.ParseExact(booking.TimeFrom, "HH:mm", CultureInfo.InvariantCulture);
                TimeOnly timeTom = TimeOnly.ParseExact(booking.TimeTo, "HH:mm", CultureInfo.InvariantCulture);
                
                DateTime startDate = booking.Day.ToDateTime(timeFrom);
                DateTime endDate = booking.Day.ToDateTime(timeTom);

                var calEvent = new CalendarEvent
                {
                    Summary = booking.Name,
                    Description = booking.Description ?? string.Empty,
                    Location = booking.RoomName ?? "N/A",
                    Start =  new CalDateTime(startDate),
                    End = new CalDateTime(endDate),
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
                TimeOnly timeFrom = TimeOnly.ParseExact(booking.TimeFrom, "HH:mm", CultureInfo.InvariantCulture);
                TimeOnly timeTom = TimeOnly.ParseExact(booking.TimeTo, "HH:mm", CultureInfo.InvariantCulture);
                
                DateTime startDate = booking.Day.ToDateTime(timeFrom);
                DateTime endDate = booking.Day.ToDateTime(timeTom);

                calBuilder.AppendLine("BEGIN:VEVENT");
                calBuilder.AppendLine($"SUMMARY:{booking.Name}");
                calBuilder.AppendLine($"LOCATION:{booking.RoomName}");
                calBuilder.AppendLine($"DESCRIPTION:{booking.Description ?? ""}");
                calBuilder.AppendLine($"DTSTART:{startDate:yyyyMMddTHHmmss}");
                calBuilder.AppendLine($"DTEND:{endDate:yyyyMMddTHHmmss}");
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

        public static byte[] ExportToExcel(IEnumerable<BookingDto> bookings)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Réservations");

            var headers = new[] { "Nom", "Description", "Salle", "Organisateur", "Date de début", "Date de fin", "Statut", "Invités" };
            worksheet.Cells["A1:H1"].LoadFromArrays(new[] { headers });

            var row = 2;
            foreach (var booking in bookings)
            {
                TimeOnly timeFrom = TimeOnly.ParseExact(booking.TimeFrom, "HH:mm", CultureInfo.InvariantCulture);
                TimeOnly timeTom = TimeOnly.ParseExact(booking.TimeTo, "HH:mm", CultureInfo.InvariantCulture);
                
                DateTime startDate = booking.Day.ToDateTime(timeFrom);
                DateTime endDate = booking.Day.ToDateTime(timeTom);

                var rowData = new object[]
                {
                    booking.Name,
                    booking.Description ?? "",
                    booking.RoomName ?? "N/A",
                    $"{booking.OrganizerFirstname} {booking.OrganizerLastname}",
                    startDate.ToString("dd/MM/yyyy HH:mm"),
                    endDate.ToString("dd/MM/yyyy HH:mm"),
                    booking.Statut,
                    string.Join(", ", booking.GuestsName ?? Array.Empty<string>())
                };
                
                worksheet.Cells[row, 1, row, 8].LoadFromArrays(new[] { rowData });
                row++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.Cells["A1:H1"].Style.Font.Bold = true;
            worksheet.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);

            return package.GetAsByteArray();
        }
    }
} 