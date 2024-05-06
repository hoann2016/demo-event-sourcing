using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EditMessageController : ControllerBase
    {
        private readonly ILogger<EditMessageController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        public EditMessageController(ILogger<EditMessageController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditMessageAsync(Guid id, [FromBody] EditMessageCommand command)
        {

            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);
                return Ok(new BaseResponse { Message = "Message edited successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (AggregateNotFoundException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "could not retrive aggregate, client passed an incorrect");
                return NotFound(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to edit message of a new post";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error"
                });
            }

        }

    }
}