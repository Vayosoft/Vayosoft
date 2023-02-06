using MediatR;
using Vayosoft.Commands;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Exceptions;
using Vayosoft.Utilities;

namespace Vayosoft.Persistence.Commands;

public record DeleteCommand<TEntity>(TEntity Entity) : ICommand where TEntity : IEntity;

public class DeleteCommandHandler<TEntity> : ICommandHandler<DeleteCommand<TEntity>>
    where TEntity : class, IEntity
{
    private readonly IUoW _unitOfWork;

    public DeleteCommandHandler(IUoW unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteCommand<TEntity> command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command.Entity, nameof(command.Entity));

        var id = command.Entity.Id;

        var entity = await _unitOfWork.FindAsync<TEntity>(id, cancellationToken);
        if (entity == null)
        {
            throw EntityNotFoundException.For<TEntity>(id);
        }

        _unitOfWork.Delete(entity);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
