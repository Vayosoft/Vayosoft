using Vayosoft.Commands;
using Vayosoft.Commons.Entities;
using Vayosoft.Utilities;

namespace Vayosoft.Persistence.Commands;

public record DeleteCommand<TEntity>(TEntity Entity) : ICommand where TEntity : IEntity;

public class DeleteCommandHandler<TEntity> : ICommandHandler<DeleteCommand<TEntity>>
    where TEntity : class, IEntity
{
    private readonly IDAO _dao;

    public DeleteCommandHandler(IDAO dao)
    {
        _dao = dao;
    }

    public async Task Handle(DeleteCommand<TEntity> command, CancellationToken cancellationToken)
    {
        var entity = Guard.NotNull(command.Entity, nameof(command.Entity));
        await _dao.DeleteAsync(entity, cancellationToken);
    }
}
