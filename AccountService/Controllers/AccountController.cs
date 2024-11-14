using Microsoft.AspNetCore.Mvc;
using Postie.Dtos;
using Postie.Interfaces;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public ActionResult<Guid> Create([FromBody] NewAccountRequest request)
        {
            var result = _accountService.Create(request.Username, request.Email, request.Password);

            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<bool> Delete(Guid id)
        {
            var result = _accountService.Delete(id);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<AccountResponse> Get(Guid id)
        {
            var result = _accountService.Get(id);

            if (result.Id == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(new AccountResponse(result.Id, result.Username, result.Email));
        }

        [HttpGet]
        public ActionResult<List<AccountResponse>> List()
        {
            var result = _accountService.List();
            return Ok(result.Select(x => new AccountResponse(x.Id, x.Username, x.Email)).ToList());
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Guid> Update(Guid id, [FromBody] AccountRequest request)
        {
            var result = _accountService.Update(id, request.Username, request.Email);

            if (result == Guid.Empty)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPut("{id:guid}/Password")]
        public ActionResult<bool> ChangePassword(Guid id, [FromBody] PasswordChangeRequest request)
        {
            var result = _accountService.ChangePassword(id, request.OldPassword, request.NewPassword);

            if (result)
            {
                return Ok();
            }

            return BadRequest(result);
        }
    }
}
