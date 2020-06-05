using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Data;
using WebApplication.Data.Entities;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase

    {
        private readonly ApplicationRepository repository;

        public UserController(ApplicationRepository repository)
        {
            this.repository = repository;
        }


        /*[HttpPost("login")]
        public async Task<ActionResult> onPostLogin([FromQuery] string email, [FromQuery] string password)
        {
            if(email == null || password == null)
            {
                return BadRequest();
            }

            else
            {
                UserEntity userEntity = await repository.GetUserEntity(email);


                if(userEntity != null)
                {
                    if(userEntity.Password.Equals(password))
                    {
                        return Ok("Login succeded");
                    }
                }

                return NotFound();
            }
        }*/

        //POST

        [HttpPost("CreateAccount")]
        public async Task<ActionResult> OnPostCreateAccount([FromQuery] string UserID, [FromQuery] int ProductID, [FromQuery] string Token)
        {
            if (UserID != null && ProductID > 0 && Token != null)
            {
                UserEntity user = await repository.GetUserEntity(UserID);

                if (user == null)
                {
                    var userEntity = new UserEntity { UserID = UserID, ProductID = ProductID, Token = Token };
                    repository.CreateAccount(userEntity);
                    return CreatedAtAction("CreateAccount", Models.User.getUserFromEntity(userEntity));
                }
                else
                {
                    return BadRequest("There is already a user with such data");
                }
            }
            return BadRequest("userId or productId or token not applicable");
        }

        //DELETE
        [HttpDelete("DeleteAccount")]
        public async Task<ActionResult> OnDelete([FromQuery] string UserID)
        {
            if (UserID != null)
            {
                UserEntity user = await repository.GetUserEntity(UserID);
                if (user != null)
                {
                    repository.DeleteUser(user);
                    return CreatedAtAction("DeleteAccount", Models.User.getUserFromEntity(user));
                }
                else
                {
                    return BadRequest("There is no such a user with the given UserID");
                }
            }
            return BadRequest("userId not applicable");


        }
    }
}