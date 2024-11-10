using Microsoft.AspNetCore.Mvc;
using Postie.Configurations;
using Postie.Dtos;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Postie.Controllers
{
    [ApiController]
    [Route("controllerPost")]
    public class PostController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public PostController()
        {
            _httpClient = RequestClient.Instance.DefaultHttpClient;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostDto>>> Posts()
        {
            var url = ServicesRouts.Routs[ServicesRouts.Services.Post];

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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var url = String.Format("{0}/{1}", ServicesRouts.Routs[ServicesRouts.Services.Post], id);

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
                var url = ServicesRouts.Routs[ServicesRouts.Services.Post];
                var content = new StringContent($"\"{text}\"", Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<bool>> UpdatePost(Guid id, [FromBody] string text)
        {
            try
            {
                var url = String.Format("{0}/{1}", ServicesRouts.Routs[ServicesRouts.Services.Post], id);
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
                var url = String.Format("{0}/{1}", ServicesRouts.Routs[ServicesRouts.Services.Post], id);
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
