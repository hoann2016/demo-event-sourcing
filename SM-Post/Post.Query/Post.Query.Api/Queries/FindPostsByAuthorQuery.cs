using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Domain.Queries;

namespace Post.Query.Api.Queries
{
    public class FindPostsByAuthorQuery : BaseQuery
    {
        public string Author { get; set; }
    }
}