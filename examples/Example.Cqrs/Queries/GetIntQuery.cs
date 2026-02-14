using Lumini.Core.Cqrs.Queries;

namespace Example.Cqrs.Queries;

internal class GetIntQuery : IQuery<int>
{
}

internal class GetIntQueryHandler : IQueryHandler<GetIntQuery, int>
{
    public async Task<int> Handle(GetIntQuery request, CancellationToken cancellationToken)
    {
        return 42;
    }
}
