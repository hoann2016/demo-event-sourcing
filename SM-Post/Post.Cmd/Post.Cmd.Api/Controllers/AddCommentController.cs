using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class AddCommentController(ILogger<AddCommentController> logger, ICommandDispatcher dispatcher)
        : ControllerBase
    {
        [HttpPut("{id}")]
        public async Task<ActionResult> AddCommentAsync(Guid id, [FromBody] AddCommentCommand command)
        {
            try
            {
                command.Id = id;
                await dispatcher.SendAsync(command);
                return Ok(new BaseResponse { Message = "Add comment successfully" });
            }
            catch (InvalidOperationException ex)
            {
                logger.Log(LogLevel.Warning, ex, "bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (AggregateNotFoundException ex)
            {
                logger.Log(LogLevel.Warning, ex, "could not retrive aggregate, client passed an incorrect");
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to add comment";
                logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error"
                });
            }
        }
    }
}