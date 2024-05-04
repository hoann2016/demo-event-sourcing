using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain;

namespace CQRS.Core.Handler
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregateRoot);
        Task<T> GetByIdAsync(Guid id);
    }
}