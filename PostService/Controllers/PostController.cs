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
                var filteredResult = filteredPosts.Value.Select(x => new CreatedPostDto(x.Id, x.AccountId, x.Text, x.CreatedBy.ToLocalTime(), x.ModifiedBy?.ToLocalTime()));
                return Ok(filteredResult);
            }
            
            var posts = _postService.List();
            var result = posts.Value.Select(x => new CreatedPostDto(x.Id, x.AccountId, x.Text, x.CreatedBy.ToLocalTime(), x.ModifiedBy?.ToLocalTime()));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<CreatedPostDto> GetPost(Guid id)
        {
            var result = _postService.Get(id);

            if (result.IsSuccess)
            {
                var post = result.Value;
                var response = new CreatedPostDto(post.Id, post.AccountId, post.Text, post.CreatedBy.ToLocalTime(), post.ModifiedBy?.ToLocalTime());
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPost]
        public ActionResult<Guid> CreatePost([FromBody] RequestPostDto post)
        {
            var result = _postService.Create(post.RequesterId, post.Text);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Guid> UpdatePost(Guid id, [FromBody] RequestPostDto post)
        {
            var result = _postService.Update(id, post.Text, post.RequesterId);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<bool> DeletePost(Guid id, [FromBody] RequesterDto request)
        {
            var result = _postService.Delete(id, request.RequesterId);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }        
    }
}
