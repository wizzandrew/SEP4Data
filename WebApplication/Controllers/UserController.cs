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


        [HttpPost("login")]
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
        }
    }
}