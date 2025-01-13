using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Validations {

    public static class Groupes {
        public static readonly string[] AllowedGroupes = {
            "Petite Réunion", "Moyenne Réunion", "Grande Réunion",
            "Conférence", "Fête", "Gala"
        };
    }
}