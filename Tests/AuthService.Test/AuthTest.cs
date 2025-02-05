using Microsoft.AspNetCore.Http;
using Postie.Dtos;
using System.IdentityModel.Tokens.Jwt;

namespace AuthService.Test
{
    [TestClass]
    public class AuthTest : BaseTest.BaseTest
    {
        public AuthTest() : base()
        {
            
        }

        [TestMethod]
        public void LoginTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var token = AuthServiceInstance.Login(User_Email, User_Password);
            Assert.IsFalse(String.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var id = tokenS.Claims.First(claim => claim.Type == "requesterId").Value;
            Assert.IsTrue(accountId == Guid.Parse(id));
        }
    }
}