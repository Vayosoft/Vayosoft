using MediatR;
using Microsoft.Extensions.Logging;
using Vayosoft.Commands;
using Vayosoft.Commons.Exceptions;
using Vayosoft.Identity.Exceptions;
using Vayosoft.Identity.Extensions;
using Vayosoft.Identity.Persistence;

namespace Vayosoft.Identity.Security.Commands
{
    public class SaveRole : SecurityRoleEntity, ICommand
    { }

    public class HandleSaveRole : ICommandHandler<SaveRole>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserContext _userContext;
        private readonly ILogger<HandleSaveRole> _logger;

        public HandleSaveRole(IUserRepository userRepository, IUserContext userContext, ILogger<HandleSaveRole> logger)
        {
            _userRepository = userRepository;
            _userContext = userContext;
            _logger = logger;
        }
        public async Task Handle(SaveRole command, CancellationToken cancellationToken)
        {
            try
            {
                await _userContext.LoadContextAsync();

                if (!_userContext.IsAdministrator)
                    throw new NotEnoughPermissionsException();

                if (!_userContext.IsSupervisor)
                    command.ProviderId = _userContext.User.Identity?.GetProviderId();

                if (!string.IsNullOrEmpty(command.Id))
                {
                    var old = await _userRepository.FindRoleByIdAsync(command.Id, cancellationToken);
                    if (old == null)
                        throw EntityNotFoundException.For<SecurityRoleEntity>(command.Id);

                    if (!old.Name.Equals(command.Name) || !string.Equals(old.Description, command.Description))
                        await _userRepository.UpdateSecurityRoleAsync(command, cancellationToken);
                }
                else
                {
                    await _userRepository.UpdateSecurityRoleAsync(command, cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
            }
        }
    }
}
