using Postie.Dtos;
using Postie.Models;

namespace AccountService.Test
{
    [TestClass]
    public class AccountTest : BaseTest.BaseTest
    {
        public AccountTest() : base()
        {
        }

        [TestMethod]
        public void CreateAccountTest()
        {
            var newAccount = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password));
            Assert.IsTrue(newAccount.IsSuccess);
        }

        [TestMethod]
        public void CreateInvalidAccountTest()
        {
            var emptyUsernameAccount = AccountServiceInstance.Create(new NewAccountRequest("", "", ""));
            Assert.IsTrue(emptyUsernameAccount.Error == ErrorType.ValidationError);

            var tooLongUsername = AccountServiceInstance.Create(new NewAccountRequest(new string('a', 100), "email", ""));
            Assert.IsTrue(tooLongUsername.Error == ErrorType.ValidationError);

            var tooLongEmail = AccountServiceInstance.Create(new NewAccountRequest("name", new string('a', 256), ""));
            Assert.IsTrue(tooLongEmail.Error == ErrorType.ValidationError);

            var invalidEmail = AccountServiceInstance.Create(new NewAccountRequest("name", "email", ""));
            Assert.IsTrue(invalidEmail.Error == ErrorType.ValidationError);

            var shortPassword = AccountServiceInstance.Create(new NewAccountRequest("name", "email@email.com", "1234"));
            Assert.IsTrue(shortPassword.Error == ErrorType.ValidationError);

            var invalidPassword = AccountServiceInstance.Create(new NewAccountRequest("name", "email@email.com", "123456789"));
            Assert.IsTrue(invalidPassword.Error == ErrorType.ValidationError);
        }

        [TestMethod]
        public void GetAccountByIdTest()
        {
            var createResult = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password));
            var createdAccount = AccountServiceInstance.Get(createResult.Value);

            Assert.IsTrue(createdAccount.IsSuccess);
            Assert.IsTrue(createdAccount.Value.Username == User_Name);
            Assert.IsTrue(createdAccount.Value.Email == User_Email);
        }

        [TestMethod]
        public void GetAccountsListTest()
        {
            AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password));
            AccountServiceInstance.Create(new NewAccountRequest("test", "test@test.com", "testTest1"));

            var list = AccountServiceInstance.List();
            Assert.IsTrue(list.Value.Count == 2);
        }

        [TestMethod]
        public void UpdateAccountTest()
        {
            var createResult = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password));
            var accountId = createResult.Value;
            var createdAccount = AccountServiceInstance.Get(accountId);
            Assert.IsTrue(createdAccount.Value.Email == User_Email);

            var newEmail = "newemail@email.com";
            var updateResult = AccountServiceInstance.Update(accountId, new AccountDto(accountId, User_Name, newEmail));
            Assert.IsTrue(updateResult.IsSuccess);

            var updatedAccount = AccountServiceInstance.Get(accountId);
            Assert.IsTrue(updatedAccount.Value.Email == newEmail);
        }

        [TestMethod]
        public void DeleteAccountTest()
        {
            var createResult = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password));
            var accountId = createResult.Value;
            var createdAccount = AccountServiceInstance.Get(accountId);
            Assert.IsTrue(createdAccount.IsSuccess);

            var deleteResult = AccountServiceInstance.Delete(accountId, accountId);
            Assert.IsTrue(deleteResult.Value);
            var deletedAccount = AccountServiceInstance.Get(accountId);
            Assert.IsTrue(deletedAccount.Value.Id == Guid.Empty);
        }

        [TestMethod]
        public void ChangePasswordTest()
        {
            var createResult = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password));
            var accountId = createResult.Value;

            var failedChanging = AccountServiceInstance.ChangePassword(accountId, accountId, "123456789", "newPassword1");
            Assert.IsFalse(failedChanging.IsSuccess);
            Assert.IsTrue(failedChanging.Error == ErrorType.ValidationError);

            var successChanging = AccountServiceInstance.ChangePassword(accountId, accountId, User_Password, "newPassword1");
            Assert.IsTrue(successChanging.IsSuccess);
            Assert.IsTrue(String.IsNullOrEmpty(successChanging.Value));

            var failedChanging2 = AccountServiceInstance.ChangePassword(accountId, accountId, User_Password, "newPassword2");
            Assert.IsFalse(failedChanging2.IsSuccess);
            Assert.IsTrue(failedChanging2.Error == ErrorType.ValidationError);

            var successChanging2 = AccountServiceInstance.ChangePassword(accountId, accountId, "newPassword1", "newPassword2");
            Assert.IsTrue(successChanging2.IsSuccess);
            Assert.IsTrue(String.IsNullOrEmpty(successChanging.Value));
        }

        [TestMethod]
        public void AuthorizeTest()
        {
            var firstAccountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var secondAccountId = AccountServiceInstance.Create(new NewAccountRequest("test", "test@test.com", "testTest1")).Value;

            var update = AccountServiceInstance.Update(secondAccountId, new AccountDto(firstAccountId, "newusername", "newemail@email.com"));
            Assert.IsFalse(update.IsSuccess);
            Assert.IsTrue(update.Error == ErrorType.AccessDenied);

            var changePassword = AccountServiceInstance.ChangePassword(secondAccountId, firstAccountId, User_Password, "newPassword1");
            Assert.IsFalse(changePassword.IsSuccess);
            Assert.IsTrue(changePassword.Error == ErrorType.AccessDenied);

            var delete = AccountServiceInstance.Delete(secondAccountId, firstAccountId);
            Assert.IsFalse(delete.IsSuccess);
            Assert.IsTrue(delete.Error == ErrorType.AccessDenied);
        }
    }
}
