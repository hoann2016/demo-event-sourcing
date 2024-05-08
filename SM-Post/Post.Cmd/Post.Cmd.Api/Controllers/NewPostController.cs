using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NewPostController : ControllerBase
    {
        private readonly ILogger<NewPostController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;

        public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> NewPostAsync([FromBody] NewPostCommand command)
        {
            var id = Guid.NewGuid();
            try
            {
                command.Id = id;
                await _commandDispatcher.SendAsync(command);
                return StatusCode(StatusCodes.Status201Created, new NewPostResponse
                {
                               
                    Message = $"New post creation request with message {command.Message} completed successfully.",
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log(LogLevel.Warning, ex, "bad request");
                return BadRequest(new BaseResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                const string SAFE_ERROR_MESSAGE = "Error while processing request to create a new post";
                _logger.Log(LogLevel.Error, ex, SAFE_ERROR_MESSAGE);
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Message = "Internal server error"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPostsAsync()
        {
            return Ok("ok now");
        }
    }
}