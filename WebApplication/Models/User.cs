using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Data.Entities;

namespace WebApplication.Models
{
    public class User
    {
        public string UserID { get; set; }

        public string Token { get; set; }

        public int ProductID { get; set; }


        public static User getUserFromEntity(UserEntity userEntity)
        {
            User user;

            if (userEntity != null)
            {
                user = new User
                {
                    UserID = userEntity.UserID,
                    Token = userEntity.Token,
                    ProductID = userEntity.ProductID,

                };
                return user;
            }
            return null;
        }
    }
}
