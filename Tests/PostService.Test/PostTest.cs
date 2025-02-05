using Postie.Dtos;
using Postie.Models;

namespace PostService.Test
{
    [TestClass]
    public class PostTest : BaseTest.BaseTest
    {
        private const string Default_Post = "Post text";
        public PostTest() : base()
        {
            
        }

        [TestMethod]
        public void CreatePostTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var createResult = PostServiceInstance.Create(accountId, new RequestPostDto(Default_Post));
            Assert.IsTrue(createResult.IsSuccess);
        }

        [TestMethod]
        public void CreateInvalidPostTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var emptyResult = PostServiceInstance.Create(accountId, new RequestPostDto(""));
            Assert.IsFalse(emptyResult.IsSuccess);
            Assert.IsTrue(emptyResult.Error == ErrorType.ValidationError);
        }

        [TestMethod]
        public void GetPostByIdTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;

            var postId = PostServiceInstance.Create(accountId, new RequestPostDto(Default_Post)).Value;
            var post = PostServiceInstance.Get(postId);
            Assert.IsTrue(post.IsSuccess);
            Assert.IsTrue(post.Value.Text == Default_Post);
            Assert.IsTrue(post.Value.AccountId == accountId);
            Assert.IsTrue(post.Value.CreatedBy.Date == DateTime.Now.Date);
        }

        [TestMethod]
        public void GetPostsListTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var secondPost = "second post";
            PostServiceInstance.Create(accountId, new RequestPostDto(Default_Post));
            PostServiceInstance.Create(accountId, new RequestPostDto(secondPost));

            var posts = PostServiceInstance.List();
            Assert.IsTrue(posts.IsSuccess);
            Assert.IsTrue(posts.Value.Count == 2);
            Assert.IsTrue(posts.Value.Where(x => x.Text == Default_Post).Count() == 1);
            Assert.IsTrue(posts.Value.Where(x => x.Text == secondPost).Count() == 1);
        }

        [TestMethod]
        public void GetPostsByAccountIdTest()
        {
            var firstAccountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var secondAccountId = AccountServiceInstance.Create(new NewAccountRequest("test", "test@test.com", "testTest1")).Value;

            var secondPost = "second post";
            PostServiceInstance.Create(firstAccountId, new RequestPostDto(Default_Post));
            PostServiceInstance.Create(secondAccountId, new RequestPostDto(secondPost));

            var allPosts = PostServiceInstance.List().Value;
            Assert.IsTrue(allPosts.Count == 2);

            var accountPosts = PostServiceInstance.ListByAccountId(firstAccountId);
            Assert.IsTrue(accountPosts.IsSuccess);
            Assert.IsTrue(accountPosts.Value.Count == 1);
            Assert.IsTrue(accountPosts.Value.Where(x => x.Text == Default_Post).Count() == 1);
        }

        [TestMethod]
        public void UpdatePostTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;

            var postId = PostServiceInstance.Create(accountId, new RequestPostDto(Default_Post)).Value;
            var post = PostServiceInstance.Get(postId);
            Assert.IsTrue(post.Value.Text == Default_Post);

            var text = "new text";
            var update = PostServiceInstance.Update(postId, accountId, new RequestPostDto(text));
            Assert.IsTrue(update.IsSuccess);

            var updatedPost = PostServiceInstance.Get(postId).Value;
            Assert.IsTrue(updatedPost.Text == text);
            Assert.IsTrue(updatedPost.ModifiedBy.Value.Date == DateTime.Now.Date);
        }

        [TestMethod]
        public void DeletePostTest()
        {
            var accountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;

            var postId = PostServiceInstance.Create(accountId, new RequestPostDto(Default_Post)).Value;
            var post = PostServiceInstance.Get(postId);
            Assert.IsTrue(post.Value.Id == postId);

            var delete = PostServiceInstance.Delete(postId, accountId);
            Assert.IsTrue(delete.IsSuccess);
            var deletedPost = PostServiceInstance.Get(postId);
            Assert.IsTrue(deletedPost.Value.Id == Guid.Empty);
        }

        [TestMethod]
        public void AuthorizeTest()
        {
            var firstAccountId = AccountServiceInstance.Create(new NewAccountRequest(User_Name, User_Email, User_Password)).Value;
            var secondAccountId = AccountServiceInstance.Create(new NewAccountRequest("test", "test@test.com", "testTest1")).Value;

            var postId = PostServiceInstance.Create(firstAccountId, new RequestPostDto(Default_Post)).Value;
            var update = PostServiceInstance.Update(secondAccountId, postId, new RequestPostDto("second post"));
            Assert.IsFalse(update.IsSuccess);
            Assert.IsTrue(update.Error == ErrorType.AccessDenied);

            var delete = PostServiceInstance.Delete(postId, secondAccountId);
            Assert.IsFalse(delete.IsSuccess);
            Assert.IsTrue(delete.Error == ErrorType.AccessDenied);
        }
    }
}