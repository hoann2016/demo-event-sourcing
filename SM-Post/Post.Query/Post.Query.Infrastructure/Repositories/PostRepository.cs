using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly DatabaseContextFactory _databaseContextFactory;

        public PostRepository(DatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }

        public async Task CreateAsync(PostEntity post)
        {
            using (DatabaseContext context = _databaseContextFactory.CreateDbContext())
            {
                context.Posts.Add(post);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid postId)
        {
            using var context = _databaseContextFactory.CreateDbContext();

            var post = await GetByIdAsync(postId);
            if (post == null) return;
            context.Posts.Remove(post);
            await context.SaveChangesAsync();
        }

        public async Task<PostEntity> GetByIdAsync(Guid postId)
        {
            using var context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(x => x.PostId == postId);
        }

        public async Task<List<PostEntity>> ListAllAsync()
        {
            using var context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(x => x.Comments).AsNoTracking().ToListAsync();
        }

        public async Task<List<PostEntity>> ListByAuthorAsync(string author)
        {
            using var context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(x => x.Comments).AsNoTracking()
                .Where(x => x.Author.Contains(author)).ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithCommentAsync()
        {
            using var context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(x => x.Comments).AsNoTracking()
                .Where(x => x.Comments != null && x.Comments.Any()).ToListAsync();
        }

        public async Task<List<PostEntity>> ListWithLikeAsync(int numberOfLikes)
        {
            using var context = _databaseContextFactory.CreateDbContext();
            return await context.Posts.AsNoTracking().Include(x => x.Comments).AsNoTracking()
                .Where(x => x.Likes >= numberOfLikes).ToListAsync();
        }

        public async Task UpdateAsync(PostEntity post)
        {
            using var context = _databaseContextFactory.CreateDbContext();
            context.Posts.Update(post);
            await context.SaveChangesAsync();
        }
    }
}