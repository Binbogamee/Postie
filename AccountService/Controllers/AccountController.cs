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
            var result = _accountService.Create(request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpDelete("{id:guid}")]
        public ActionResult<bool> Delete(Guid id)
        {
            var requesterId = Guid.Empty;
            Guid.TryParse((string)HttpContext.Items[RequesterIdMiddleware.UserIdName], out requesterId);
            var result = _accountService.Delete(requesterId, id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpGet("{id:guid}")]
        public ActionResult<AccountDto> Get(Guid id)
        {
            var result = _accountService.Get(id);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpGet]
        public ActionResult<List<AccountDto>> List()
        {
            var result = _accountService.List();
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPut("{id:guid}")]
        public ActionResult<Guid> Update(Guid id, [FromBody] AccountDto request)
        {
            request = new AccountDto(id, request.Username, request.Email);
            var requesterId = Guid.Empty;
            Guid.TryParse((string)HttpContext.Items[RequesterIdMiddleware.UserIdName], out requesterId);
            var result = _accountService.Update(requesterId, request);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }

        [HttpPut("{id:guid}/Password")]
        public ActionResult<bool> ChangePassword(Guid id, [FromBody] PasswordChangeRequest request)
        {
            var requesterId = Guid.Empty;
            Guid.TryParse((string)HttpContext.Items[RequesterIdMiddleware.UserIdName], out requesterId);
            var result = _accountService.ChangePassword(requesterId, id, request.OldPassword, request.NewPassword);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return ControllerResultMapper.ResultMapper(result.Error, result.ErrorMessage);
        }
    }
}
