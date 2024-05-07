
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult> GetAllPostsAsync()
        {
            try
            {
                var posts = await queryDispatcher.SendAsync(new FindAllPostsQuery());
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts.");
            }
        }
        [HttpGet("byId/{postId}")]
        public async Task<ActionResult> GetByPostIDAsync(Guid postId)
        {

            try
            {
                var posts = await queryDispatcher.SendAsync(new FindPostByIdQuery { Id = postId });
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving post by ID.");
            }
        }
        [HttpGet("byAuthor/{author}")]
        public async Task<ActionResult> GetPostsByAuthorAsync(string author)
        {

            try
            {
                var posts = await queryDispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts by author.");
            }
        }
        [HttpGet("withComments")]
        public async Task<ActionResult> GetPostsWithCommentsAsync()
        {

            try
            {
                var posts = await queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts with comments.");
            }

        }
        [HttpGet("withLikes/{numberOfLikes}")]
        public async Task<ActionResult> GetPostsWithLIkesAsync(int numberOfLikes)
        {

            try
            {
                var posts = await queryDispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });
                return NormalResponse(posts);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex, "An error occurred while retrieving posts with number of likes.");
            }

        }
        private ActionResult NormalResponse(List<PostEntity> posts)
        {
            if (posts == null || !posts.Any())
            {
                return NoContent();
            }
            return Ok(new PostLookupResponse
            {
                Posts = posts,
                Message = $"Successfully retrieved {posts.Count} posts."
            });
        }
        private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
        {
            logger.LogError(ex, safeErrorMessage);
            return StatusCode(500, safeErrorMessage);
        }
    }
}