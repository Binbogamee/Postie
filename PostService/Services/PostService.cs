using Postie.Interfaces;
using Postie.Models;
using Shared.KafkaLogging;
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
        public (Guid id, string error) Create(string text)
        {
            var validation = Validate(text);

            if (!string.IsNullOrEmpty(validation))
            {
                return (Guid.Empty, validation);
            }

            var post = new Post(text);
            _postRepository.Create(post);
            return (post.Id, string.Empty);
        }

        public bool Delete(Guid id)
        {
            return _postRepository.Delete(id);
        }

        public Post Get(Guid id)
        {
            return _postRepository.Get(id);
        }

        public ICollection<Post> List()
        {
            return _postRepository.List();
        }

        public (Guid id, string error) Update(Guid id, string text)
        {
            var validation = Validate(text);

            if (validation != string.Empty)
            {
                return (Guid.Empty, validation);
            }

            var oldPost = Get(id);
            if (oldPost.Id == Guid.Empty)
            {
                return (Guid.Empty, Messages.PostNotFound);
            }

            var post = new Post(text, id, oldPost.CreatedBy, DateTime.UtcNow);
            _postRepository.Update(post);
            return (post.Id, string.Empty);
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
    }
}
