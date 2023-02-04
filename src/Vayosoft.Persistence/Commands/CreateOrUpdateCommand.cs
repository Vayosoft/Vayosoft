using MediatR;
using Vayosoft.Commands;
using Vayosoft.Commons;
using Vayosoft.Commons.Entities;
using Vayosoft.Utilities;

namespace Vayosoft.Persistence.Commands;

public record CreateOrUpdateCommand<TDto>(TDto Entity) : ICommand where TDto : IEntity;

public class CreateOrUpdateHandler<TKey, TEntity, TDto> : ICommandHandler<CreateOrUpdateCommand<TDto>>
    where TEntity : class, IEntity<TKey>
    where TDto : class, IEntity<TKey>
{
    private readonly IUoW _unitOfWork;
    private readonly IMapper _mapper;

    public CreateOrUpdateHandler(IUoW unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(CreateOrUpdateCommand<TDto> command, CancellationToken cancellationToken)
    {
        Guard.NotNull(command.Entity, nameof(command.Entity));

        var id = command.Entity.Id;
        if (id != null && !default(TKey)!.Equals(id))
        {
            var entity = _mapper.Map(command.Entity, _unitOfWork.Find<TEntity>(id));
            _unitOfWork.Update(entity);
        }
        else
        {
            var entity = _mapper.Map<TEntity>(command.Entity);
            _unitOfWork.Add(entity);
        }

        await _unitOfWork.CommitAsync();
        return Unit.Value;
    }
}
