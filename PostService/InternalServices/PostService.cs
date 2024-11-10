using Postie.Interfaces;
using Postie.Models;
using LogLevel = NLog.LogLevel;

namespace PostService.InternalServices
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ILoggingProducerService _logProduceService;
        private const int MAX_LENGTH = 50;
        public PostService(IPostRepository postRepository, ILoggingProducerService logProduceService) 
        {
            _postRepository = postRepository;
            _logProduceService = logProduceService;

        }
        public Guid Create(string text)
        {
            var validation = Validate(text);

            if (validation != string.Empty)
            {
                _logProduceService.SendLogMessage(LogLevel.Error, validation);
                return Guid.Empty;
            }

            var post = new Post(text);
            if (_postRepository.Create(post))
            {
                _logProduceService.SendLogMessage(LogLevel.Info, $"The post {post.Id} created");
                return post.Id;
            }

            _logProduceService.SendLogMessage(LogLevel.Error, "The post didn't be added to database");
            return Guid.Empty;
        }

        public bool Delete(Guid id)
        {
            if (_postRepository.Delete(id))
            {
                _logProduceService.SendLogMessage(LogLevel.Info, $"The post {id} deleted");
                return true;
            }

            _logProduceService.SendLogMessage(LogLevel.Error, $"Error deleting the post {id}");
            return false;
        }

        public Post Get(Guid id)
        {
            var post = _postRepository.Get(id);
            if (post.Id == Guid.Empty)
            {
                _logProduceService.SendLogMessage(LogLevel.Error, $"The post {id} not found");
            }

            _logProduceService.SendLogMessage(LogLevel.Info, $"Get the post {id}");
            
            return post;
        }

        public ICollection<Post> List()
        {
            _logProduceService.SendLogMessage(LogLevel.Info, "Get all posts");
            return _postRepository.List();
        }

        public Guid Update(Guid id, string text)
        {
            var validation = Validate(text);

            if (validation != string.Empty)
            {
                _logProduceService.SendLogMessage(LogLevel.Error, validation);
                return Guid.Empty;
            }

            var oldPost = Get(id);
            var post = new Post(text, id, oldPost.CreatedBy, DateTime.UtcNow);

            if (_postRepository.Update(post))
            {
                _logProduceService.SendLogMessage(LogLevel.Info, $"The post {post.Id} updated");
                return post.Id;
            }

            _logProduceService.SendLogMessage(LogLevel.Error, "The post didn't be updated");
            return Guid.Empty;
        }

        private string Validate(string text)
        {
            if (text.Length == 0)
            {
                return "The post cannot be empty";
            }

            if (text.Length > MAX_LENGTH)
            {
                return "The post must be less than 200 characters.";
            }

            return string.Empty;
        }
    }
}
