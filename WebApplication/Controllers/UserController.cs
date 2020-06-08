using System;
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
        public readonly IApplicationRepository repository;

        public UserController(IApplicationRepository repository)
        {
            this.repository = repository;
        }


        
        /// <summary>
        /// Create user account speciying user Id, product Id and token
        /// </summary>
        /// <param name="UserID">Id of the user</param>
        /// <param name="ProductID">Id of the product</param>
        /// <param name="Token">Token given to user during registration in google firebase</param>
        /// <returns>created user object or status code</returns>
        [ProducesResponseType(typeof(UserEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("CreateAccount")]
        public async Task<ActionResult> OnPostCreateAccount([FromQuery] string UserID, [FromQuery] int ProductID, [FromQuery] string Token)
        {
            UserEntity user;

            if (UserID != null && ProductID > 0 && Token != null)
            {
                try
                {
                    user = await repository.GetUserEntity(UserID);

                    if (user == null)
                    {

                        try
                        {
                            var res = await repository.CreateAccount(new UserEntity { UserID = UserID, ProductID = ProductID, Token = Token });

                            if (res != null)
                            {
                                return Ok("User Created");
                            }
                            return BadRequest("Internal server error");
                        }

                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            return StatusCode(500, "Internal server error");
                        }

                    }

                    return BadRequest("There is already a user with such data");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return StatusCode(500, "Internal server error");
                }

            }

            return BadRequest("UserId or ProductId or Token not applicable");
        }

        

        /// <summary>
        /// Deletes user account specifying user Id
        /// </summary>
        /// <param name="UserID">Id of the user</param>
        /// <returns>status code</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("DeleteAccount")]
        public async Task<ActionResult> OnDeleteAccount([FromQuery] string UserID)
        {
            UserEntity user;

            if (UserID != null)
            {
                try
                {
                    user = await repository.GetUserEntity(UserID);
                    if (user != null)
                    {
                        try
                        {
                            var res = await repository.DeleteAccount(user);

                            if (res == null)
                            {
                                return NoContent();
                            }
                            return BadRequest("Internal server error");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                            return StatusCode(500, "Internal server error");
                        }
                    }
                    else
                    {
                        return BadRequest("There is no such a user with the given UserID");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return StatusCode(500, "Internal server error");
                }
             
            }

            return BadRequest("UserId not applicable");

        }
    }
}