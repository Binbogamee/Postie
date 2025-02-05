using AutoMapper;
using Postie.Dtos;
using Postie.Interfaces;
using Postie.Models;
using Messages = Postie.Messages;

namespace PostService.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private const int MAX_LENGTH = 200;
        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;

        }
        public Result<Guid> Create(Guid requesterId, RequestPostDto request)
        {
            var postText = request.Text;
            var validation = Validate(postText);
            if (!string.IsNullOrEmpty(validation))
            {
                return Result<Guid>.Failure(ErrorType.ValidationError, validation);
            }

            var createdPost = new Post(postText, requesterId);
            _postRepository.Create(createdPost);
            return Result<Guid>.Success(createdPost.Id);
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

        public Result<CreatedPostDto> Get(Guid id)
        {
            var post = _postRepository.Get(id);
            return Result<CreatedPostDto>.Success(_mapper.Map<CreatedPostDto>(post));
        }

        public Result<ICollection<CreatedPostDto>> List()
        {
            var posts = _postRepository.List();
            return Result<ICollection<CreatedPostDto>>.Success(_mapper.Map<List<CreatedPostDto>>(posts));
        }

        public Result<ICollection<CreatedPostDto>> ListByAccountId(Guid accountId)
        {
            var posts = _postRepository.ListByAccountId(accountId);
            return Result<ICollection<CreatedPostDto>>.Success(_mapper.Map<List<CreatedPostDto>>(posts));
        }

        public Result<Guid> Update(Guid id, Guid requesterId, RequestPostDto request)
        {
            var postText = request.Text;

            Post oldPost;

            if (!IsAuthorize(requesterId, id, out oldPost))
            {
                return Result<Guid>.Failure(ErrorType.AccessDenied);
            }

            if (oldPost.Id == Guid.Empty)
            {
                return Result<Guid>.Failure(ErrorType.NotFound);
            }

            var validation = Validate(postText);
            if (validation != string.Empty)
            {
                return Result<Guid>.Failure(ErrorType.ValidationError, validation);
            }


            var post = new Post(postText, oldPost.AccountId, id, oldPost.CreatedBy, DateTime.UtcNow);
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
