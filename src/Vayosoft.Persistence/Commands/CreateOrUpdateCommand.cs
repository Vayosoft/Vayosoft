using Vayosoft.Commands;
using Vayosoft.Commons;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Utilities;

namespace Vayosoft.Persistence.Commands;

public record CreateOrUpdateCommand<TDto>(TDto Entity) : ICommand where TDto : IEntity;

public class CreateOrUpdateHandler<TKey, TEntity, TDto> : ICommandHandler<CreateOrUpdateCommand<TDto>>
    where TEntity : class, IEntity<TKey>
    where TDto : class, IEntity<TKey>
{
    private readonly IDAO _dao;
    private readonly IMapper _mapper;

    public CreateOrUpdateHandler(IDAO dao, IMapper mapper)
    {
        _dao = dao;
        _mapper = mapper;
    }

    public async Task Handle(CreateOrUpdateCommand<TDto> command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command.Entity, nameof(command.Entity));

        var id = command.Entity.Id;
        if (id != null && !default(TKey)!.Equals(id))
        {
            var entity = _mapper.Map(command.Entity, await _dao.FindAsync(new Criteria<TEntity>(e => e.Id.Equals(id)), cancellationToken));
            await _dao.UpdateAsync(entity, cancellationToken);
        }
        else
        {
            var entity = _mapper.Map<TEntity>(command.Entity);
            await _dao.CreateAsync(entity, cancellationToken);
        }
    }
}
