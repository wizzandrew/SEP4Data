using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public int LicenseCode { get; set; }

        public int RoomID { get; set; }

        public static User getUserFromEntity(UserEntity userEntity)
        {
            User user;

            if(userEntity != null)
            {
                user = new User
                {
                    UserID = userEntity.UserID,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Email = userEntity.Email,
                    Password = userEntity.Password,
                    RoomID = userEntity.R_ID
                };
                return user;
            }
            return null;
        }
    }
}
