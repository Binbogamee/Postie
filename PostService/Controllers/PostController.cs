using Microsoft.AspNetCore.Mvc;
using Postie.Interfaces;
using Postie.Dtos;
using Postie.Infrastructure;

namespace PostService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public ActionResult<List<CreatedPostDto>> Posts([FromQuery] Guid? accountId)
        {
            if (accountId.HasValue)
            {
                var filteredPosts = _postService.ListByAccountId(accountId.Value);
                return Ok(filteredPosts);
            }
            
            var posts = _postService.List();
            return Ok(posts);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<CreatedPostDto> GetPost(Guid id)
        {
            var result = _postService.Get(id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPost]
        public ActionResult<Guid> CreatePost([FromBody] RequestPostDto post)
        {
            var headerid = HttpContext.Items[RequesterIdMiddleware.UserIdName];
            var requesterId = Guid.Empty;
            if (headerid is Guid guid)
            {
                requesterId = guid;
            }

            var result = _postService.Create(requesterId, post);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Guid> UpdatePost(Guid id, [FromBody] RequestPostDto post)
        {
            var headerid = HttpContext.Items[RequesterIdMiddleware.UserIdName];
            var requesterId = Guid.Empty;
            if (headerid is Guid guid)
            {
                requesterId = guid;
            }

            var result = _postService.Update(id, requesterId, post);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<bool> DeletePost(Guid id)
        {
            var headerid = HttpContext.Items[RequesterIdMiddleware.UserIdName];
            var requesterId = Guid.Empty;
            if (headerid is Guid guid)
            {
                requesterId = guid;
            }

            var result = _postService.Delete(id, requesterId);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }        
    }
}
