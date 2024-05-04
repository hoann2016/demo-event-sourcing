using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories
{
    public interface ICommentRepository
    {
        Task CreateAsync(CommentEntity comment);
        Task UpdateAsync(CommentEntity comment);
        Task<CommentEntity> GetByIdAsync(Guid commentId);
        Task DeleteAsync(Guid commentId);
    }
}