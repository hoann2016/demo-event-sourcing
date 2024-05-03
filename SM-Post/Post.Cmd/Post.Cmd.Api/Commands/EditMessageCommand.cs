using CQRS.Commands;

namespace Post.Cmd.Api.Commands
{
    public class EditMessage : BaseCommand
    {
        public string Message { get; set; }
    }
}