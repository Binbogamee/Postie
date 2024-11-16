using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Postie.Infrastructure;
using Postie.Dtos;
using System.Text;

namespace Postie.Controllers
{
    [ApiController]
    [Route("controllerPost")]
    public class PostController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _postServiceRoute;
        public PostController(IOptions<ServicesRoutes> routes)
        {
            _httpClient = HttpRequestHelper.Instance.DefaultHttpClient;
            _postServiceRoute = routes.Value.PostServiceUrl;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostDto>>> Posts()
        {
            try
            {
                var response = await _httpClient.GetAsync(_postServiceRoute);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
                return Ok(response.Content.ReadAsStringAsync().Result);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var url = String.Format("{0}/{1}", _postServiceRoute, id);

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
                return Ok(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreatePost([FromBody] string text)
        {
            try
            {
                var content = new StringContent($"\"{text}\"", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_postServiceRoute, content);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
                return Created(_postServiceRoute, response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<bool>> UpdatePost(Guid id, [FromBody] string text)
        {
            try
            {
                var url = String.Format("{0}/{1}", _postServiceRoute, id);
                var content = new StringContent($"\"{text}\"", Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
                return Created(url, response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeletePost(Guid id)
        {
            try
            {
                var url = String.Format("{0}/{1}", _postServiceRoute, id);
                var response = await _httpClient.DeleteAsync(url);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return BadRequest(response.Content.ReadAsStringAsync().Result);
                }
                return Created(url, response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
