using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
    public class PostAggregate : AggregateRoot
    {
        private bool _active;
        private string _author;
        private readonly Dictionary<Guid, Tuple<string, string>> _comments = new Dictionary<Guid, Tuple<string, string>>();
        public bool Active => _active;
        public PostAggregate()
        {

        }
        public PostAggregate(Guid id, string author, string Message)
        {
            RaiseEvent(new PostCreatedEvent
            {
                Id = id,
                Author = author,
                Message = Message,
                DatePosted = DateTime.Now
            });
        }
        public void Apply(PostCreatedEvent @event)
        {
            _id = @event.Id;
            _author = @event.Author;
            _active = true;
        }
        public void EditMessage(string message)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not edit a post that is not active");
            }
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new InvalidOperationException($"the value of {nameof(message)} can not be null or empty");
            }
            RaiseEvent(new MessageUpdatedEvent
            {
                Id = _id,
                Message = message,
            });
        }
        public void Apply(MessageUpdatedEvent @event)
        {
            _id = @event.Id;
        }
        public void LikePost()
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not like a post that is not active");
            }
            RaiseEvent(new PostLikedEvent
            {
                Id = _id
            });
        }
        public void Apply(PostLikedEvent @event)
        {
            _id = @event.Id;
        }
        public void AddComment(string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not add a comment to a post that is not active");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"the value of {nameof(comment)} can not be null or empty");
            }

            RaiseEvent(new CommentAddedEvent
            {
                Id = _id,
                CommentId = Guid.NewGuid(),
                Comment = comment,
                CommentDate = DateTime.Now
            });
        }
        public void Apply(CommentAddedEvent @event)
        {
            _id = @event.Id;
            _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
        }
        public void EditComment(Guid commentId, string comment, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not edit a comment on a post that is not active");
            }
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new InvalidOperationException($"the value of {nameof(comment)} can not be null or empty");
            }
            if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("you are not allowed to edit a comment that was made by other user");
            }

            RaiseEvent(new CommentUpdatedEvent
            {
                Id = _id,
                CommentId = commentId,
                Comment = comment,
                Username = userName,
                EditDate = DateTime.Now
            });
        }
        public void Apply(CommentUpdatedEvent @event)
        {
            _id = @event.Id;
            _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
        }
        public void RemoveComment(Guid commentId, string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("you can not remove a comment on a post that is not active");
            }

            if (!_comments[commentId].Item2.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("you are not allowed to remove a comment that was made by other user");
            }
            RaiseEvent(new CommentRemovedEvent
            {
                Id = _id,
                CommentId = commentId
            });
        }
        public void Apply(CommentRemovedEvent @event)
        {
            _id = @event.Id;
            _comments.Remove(@event.CommentId);
        }
        public void DeletePost(string userName)
        {
            if (!_active)
            {
                throw new InvalidOperationException("the post already deleted");
            }
            if (!_author.Equals(userName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new InvalidOperationException("you are not allowed to delete a post that was made by other user");
            }
            RaiseEvent(new PostRemovedEvent
            {
                Id = _id
            });
        }
        public void Apply(PostRemovedEvent @event)
        {
            _id = @event.Id;
            _active = false;
        }
    }
}