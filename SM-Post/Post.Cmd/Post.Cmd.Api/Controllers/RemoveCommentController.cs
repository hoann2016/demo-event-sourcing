using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class RemoveCommentController(ILogger<RemoveCommentController> logger, ICommandDispatcher commandDispatcher) : ControllerBase
    {

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveCommentAsync(Guid id, [FromBody] RemoveCommentCommand command)
        {
            try
            {
                command.Id = id;
                await commandDispatcher.SendAsync(command);
                return Ok(new BaseResponse { Message = "Comment removed successfully" });
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
                const string SAFE_ERROR_MESSAGE = "Error while processing request to remove comment on a post";
                logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error"
                });

            }

        }

    }
}