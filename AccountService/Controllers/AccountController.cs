using Microsoft.AspNetCore.Mvc;
using Postie.Dtos;
using Postie.Infrastructure;
using Postie.Interfaces;

namespace AccountService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<bool> Delete(Guid id, [FromBody] RequesterDto request)
        {
            var result = _accountService.Delete(request.RequesterId, id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<AccountResponse> Get(Guid id)
        {
            var result = _accountService.Get(id);

            if (result.IsSuccess)
            {
                var account = result.Value;
                return Ok(new AccountResponse(account.Id, account.Username, account.Email));
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpGet]
        public ActionResult<List<AccountResponse>> List()
        {
            var result = _accountService.List();
            if (result.IsSuccess)
            {
                return Ok(result.Value.Select(x => new AccountResponse(x.Id, x.Username, x.Email)).ToList());
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Guid> Update(Guid id, [FromBody] AccountRequest request)
        {
            var result = _accountService.Update(request.RequesterId, id, request.Username, request.Email);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPut("{id:guid}/Password")]
        public ActionResult<bool> ChangePassword(Guid id, [FromBody] PasswordChangeRequest request)
        {
            var result = _accountService.ChangePassword(request.RequesterId, id, request.OldPassword, request.NewPassword);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }
    }
}
