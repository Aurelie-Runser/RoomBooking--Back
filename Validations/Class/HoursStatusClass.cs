namespace RoomBookingApi.Validations
{
    public static class Hours
    {
        public static readonly string[] AllowedHoursFrom = GenerateAllowedHours(7, 22, 15);
        public static readonly string[] AllowedHoursTo = GenerateAllowedHours(7, 23, 15);

        private static string[] GenerateAllowedHours(int startHour, int endHour, int intervalMinutes)
        {
            var hours = new List<string>();

            for (int hour = startHour; hour <= endHour; hour++)
            {
                for (int minute = 0; minute < 60; minute += intervalMinutes)
                {
                    hours.Add($"{hour:D2}:{minute:D2}");
                }
            }

            return hours.ToArray();
        }
    }
}