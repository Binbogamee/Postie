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
            var newAccount = AccountServiceInstance.Create(User_Name, User_Email, User_Password);
            Assert.IsTrue(newAccount.IsSuccess);
        }

        [TestMethod]
        public void CreateInvalidAccountTest()
        {
            var emptyUsernameAccount = AccountServiceInstance.Create("", "", "");
            Assert.IsTrue(emptyUsernameAccount.Error == ErrorType.ValidationError);

            var tooLongUsername = AccountServiceInstance.Create(new string('a', 100), "email", "");
            Assert.IsTrue(tooLongUsername.Error == ErrorType.ValidationError);

            var tooLongEmail = AccountServiceInstance.Create("name", new string('a', 256), "");
            Assert.IsTrue(tooLongEmail.Error == ErrorType.ValidationError);

            var invalidEmail = AccountServiceInstance.Create("name", "email", "");
            Assert.IsTrue(invalidEmail.Error == ErrorType.ValidationError);

            var shortPassword = AccountServiceInstance.Create("name", "email@email.com", "1234");
            Assert.IsTrue(shortPassword.Error == ErrorType.ValidationError);

            var invalidPassword = AccountServiceInstance.Create("name", "email@email.com", "123456789");
            Assert.IsTrue(invalidPassword.Error == ErrorType.ValidationError);
        }

        [TestMethod]
        public void GetAccountByIdTest()
        {
            var createResult = AccountServiceInstance.Create(User_Name, User_Email, User_Password);
            var createdAccount = AccountServiceInstance.Get(createResult.Value);

            Assert.IsTrue(createdAccount.IsSuccess);
            Assert.IsTrue(createdAccount.Value.Username == User_Name);
            Assert.IsTrue(createdAccount.Value.Email == User_Email);
        }

        [TestMethod]
        public void GetAccountsListTest()
        {
            AccountServiceInstance.Create(User_Name, User_Email, User_Password);
            AccountServiceInstance.Create("test", "test@test.com", "testTest1");

            var list = AccountServiceInstance.List();
            Assert.IsTrue(list.Value.Count == 2);
        }

        [TestMethod]
        public void UpdateAccountTest()
        {
            var createResult = AccountServiceInstance.Create(User_Name, User_Email, User_Password);
            var accountId = createResult.Value;
            var createdAccount = AccountServiceInstance.Get(accountId);
            Assert.IsTrue(createdAccount.Value.Email == User_Email);

            var newEmail = "newemail@email.com";
            var updateResult = AccountServiceInstance.Update(accountId, accountId, User_Name, newEmail);
            Assert.IsTrue(updateResult.IsSuccess);

            var updatedAccount = AccountServiceInstance.Get(accountId);
            Assert.IsTrue(updatedAccount.Value.Email == newEmail);
        }

        [TestMethod]
        public void DeleteAccountTest()
        {
            var createResult = AccountServiceInstance.Create(User_Name, User_Email, User_Password);
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
            var createResult = AccountServiceInstance.Create(User_Name, User_Email, User_Password);
            var accountId = createResult.Value;
            var createdAccount = AccountServiceInstance.Get(accountId);
            var oldPasswordHash = createdAccount.Value.PasswordHash;

            var failedChanging = AccountServiceInstance.ChangePassword(accountId, accountId, "123456789", "newPassword1");
            Assert.IsFalse(failedChanging.IsSuccess);
            Assert.IsTrue(failedChanging.Error == ErrorType.ValidationError);

            var successChanging = AccountServiceInstance.ChangePassword(accountId, accountId, User_Password, "newPassword1");
            Assert.IsTrue(successChanging.IsSuccess);
            Assert.IsTrue(String.IsNullOrEmpty(successChanging.Value));

            var newAccount = AccountServiceInstance.Get(accountId);
            Assert.IsFalse(oldPasswordHash ==  newAccount.Value.PasswordHash);
        }

        [TestMethod]
        public void AuthorizeTest()
        {
            var firstAccountId = AccountServiceInstance.Create(User_Name, User_Email, User_Password).Value;
            var secondAccountId = AccountServiceInstance.Create("test", "test@test.com", "testTest1").Value;

            var update = AccountServiceInstance.Update(secondAccountId, firstAccountId, "newusername", "newemail@email.com");
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
