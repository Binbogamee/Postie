using Postie.Interfaces;
using Postie.Models;
using Messages = Postie.Messages;

namespace PostService.InternalServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private const int MAX_LENGTH = 200;
        public PostService(IPostRepository postRepository) 
        {
            _postRepository = postRepository;

        }
        public Result<Guid> Create(Guid accountId, string text)
        {
            var validation = Validate(text);
            if (!string.IsNullOrEmpty(validation))
            {
                return Result<Guid>.Failure(ErrorType.ValidationError, validation);
            }

            var post = new Post(text, accountId);
            _postRepository.Create(post);
            return Result<Guid>.Success(post.Id);
        }

        public Result<bool> Delete(Guid id, Guid requesterId)
        {
            if (!IsAuthorize(requesterId, id, out _))
            {
                return Result<bool>.Failure(ErrorType.AccessDenied);
            }

            if (_postRepository.Delete(id))
            {
                return Result<bool>.Success(true);
            }

            return Result<bool>.Failure(ErrorType.NotFound);
        }

        public Result<Post> Get(Guid id)
        {
            return Result<Post>.Success(_postRepository.Get(id));
        }

        public Result<ICollection<Post>> List()
        {
            return Result<ICollection<Post>>.Success(_postRepository.List());
        }

        public Result<ICollection<Post>> ListByAccountId(Guid accountId)
        {
            return Result<ICollection<Post>>.Success(_postRepository.ListByAccountId(accountId));
        }

        public Result<Guid> Update(Guid id, string text, Guid requesterId)
        {
            Post oldPost;

            if (!IsAuthorize(requesterId, id, out oldPost))
            {
                return Result<Guid>.Failure(ErrorType.AccessDenied);
            }

            if (oldPost.Id == Guid.Empty)
            {
                return Result<Guid>.Failure(ErrorType.NotFound);
            }

            var validation = Validate(text);
            if (validation != string.Empty)
            {
                return Result<Guid>.Failure(ErrorType.ValidationError, validation);
            }


            var post = new Post(text, oldPost.AccountId, id, oldPost.CreatedBy, DateTime.UtcNow);
            _postRepository.Update(post);
            return Result<Guid>.Success(post.Id);
        }

        private string Validate(string text)
        {
            if (text.Length == 0)
            {
                return Messages.EmptyPost;
            }

            if (text.Length > MAX_LENGTH)
            {
                return string.Format(Messages.PostIsTooLong, MAX_LENGTH);
            }

            return string.Empty;
        }

        private bool IsAuthorize(Guid requesterId, Guid postId, out Post post)
        {
            post = _postRepository.Get(postId);
            if (post.Id == Guid.Empty)
            {
                return false;
            }

            return post.AccountId == requesterId;
        }
    }
}
