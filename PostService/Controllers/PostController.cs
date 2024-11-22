using Microsoft.AspNetCore.Mvc;
using Postie.Interfaces;
using Postie.Dtos;

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
        public ActionResult<List<PostDto>> Posts()
        {
            var posts = _postService.List();
            var result = posts.Select(x => new PostDto(x.Id, x.Text, x.CreatedBy.ToLocalTime(), x.ModifiedBy?.ToLocalTime()));

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<PostDto> GetPost(Guid id)
        {
            var post = _postService.Get(id);
            if (post.Id == Guid.Empty)
            {
                return NotFound();
            }

            var response = new PostDto(post.Id, post.Text, post.CreatedBy.ToLocalTime(), post.ModifiedBy?.ToLocalTime());
            return Ok(response);
        }

        [HttpPost]
        public ActionResult<Guid> CreatePost([FromBody] string text)
        {
            var result = _postService.Create(text);
            if (result.id == Guid.Empty)
            {
                return UnprocessableEntity(result.error);
            }

            return Ok(result.id);
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Guid> UpdatePost(Guid id, [FromBody] string text)
        {
            var result = _postService.Update(id, text);
            if (result.id == Guid.Empty)
            {
                return UnprocessableEntity(result.error);
            }

            return Ok(result.id);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<bool> DeletePost(Guid id)
        {
            var result = _postService.Delete(id);
            return result ? Ok() : NotFound();
        }
    }
}
