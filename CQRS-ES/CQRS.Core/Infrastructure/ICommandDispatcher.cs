using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Commands;

namespace CQRS.Core.Infrastructure
{
    public interface ICommandDispatcher
    {
        // RegisterHandler method is a void function  and its param is  function that has BaseComand as param =>task
        void RegisterHandler<T>(Func<T,Task> Handler) where T: BaseCommand;
        Task SendAsync(BaseCommand command);
    }
}