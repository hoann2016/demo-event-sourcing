using CQRS.Commands;

namespace Post.Cmd.Api.Commands
{
    public class DeleteCommentCommand : BaseCommand
    {
        public string UserName { get; set; }
    }
}