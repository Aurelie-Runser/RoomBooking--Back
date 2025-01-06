using System.ComponentModel.DataAnnotations;
using RoomBookingApi.Models;

namespace RoomBookingApi.Mappers{
    public static class UserExtensions{
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Lastname = user.Lastname,
                Firstname = user.Firstname,
                Email = user.Email,
                Company = user.Company,
                Job = user.Job,
                Role = user.Role
            };
        }
    }
}