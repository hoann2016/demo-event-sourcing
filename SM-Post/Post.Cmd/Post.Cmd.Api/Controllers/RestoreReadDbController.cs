using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> RestoreReadDbAsync()
        {
            var id = Guid.NewGuid();
            try
            {
                await commandDispatcher.SendAsync(new RestoreReadCommand());
                return StatusCode(StatusCodes.Status201Created, new BaseResponse
                {
                    Message = "Restore read db request completed successfully.",
                });
            }
            catch (InvalidOperationException ex)
            {
                logger.Log(LogLevel.Warning, ex, "bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to restore read db";
                logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error"
                });
            }
        }
    }
}