using Microsoft.AspNetCore.Mvc;
using Vayosoft.Commands;
using Vayosoft.Identity.Extensions;
using Vayosoft.Identity.Persistence;
using Vayosoft.Identity.Security;
using Vayosoft.Identity.Security.Commands;
using Vayosoft.Identity.Security.Models;
using Vayosoft.Web.Controllers;
using Vayosoft.Web.Identity.Authorization;

namespace Vayosoft.Web.Identity.Controllers
{
    [Route("api/security")]
    public class SecurityController : ApiControllerBase
    {
        private readonly ICommandBus _commandBus;

        private readonly IUserRepository _userRepository;

        public SecurityController(
            IUserRepository userRepository,
            ICommandBus commandBus)
        {
            _userRepository = userRepository;
            _commandBus = commandBus;
        }

        [PermissionAuthorization]
        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles(CancellationToken token = default)
        {
            List<RoleDTO> items = null;
            if (_userRepository is IUserRoleStore store)
            {
                var userId = HttpContext.User.Identity!.GetUserId();
                items = await store.GetUserRolesAsync(userId, token);
            }

            return List(items);
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("user-roles/{id:long}")]
        public async Task<IActionResult> GetUserRolesById(long id, CancellationToken token = default)
        {
            List<RoleDTO> items = null;
            if (_userRepository is IUserRoleStore store)
            {
                items = await store.GetUserRolesAsync(id, token);
            }

            return List(items);
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(CancellationToken token)
        {
            List<SecurityRoleEntity> items = null;
            if (_userRepository is IUserRoleStore store)
            {
                var providerId = HttpContext.User.Identity?.GetProviderId() ?? 0;
                items = await store.GetRolesAsync(new object[] { providerId }, token);
            }

            return List(items);
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("objects")]
        public async Task<IActionResult> GetObjects(CancellationToken token)
        {
            List<SecurityObjectEntity> items = null;
            if (_userRepository is IUserRoleStore store)
            {
                items = await store.GetObjectsAsync(token);
            }

            return List(items);
        }

        [PermissionAuthorization("USER", SecurityPermissions.View)]
        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(string roleId, CancellationToken token)
        {
            RolePermissions result = null;
            if (_userRepository is IUserRoleStore store)
            {
                var role = await store.FindRoleByIdAsync(roleId, token);
                if (role == null)
                    return null;

                var permissions = await store.GetRolePermissionsAsync(roleId, token);
                var objects = await store.GetObjectsAsync(token);
                foreach (var obj in objects)
                {
                    if (permissions.All(p => p.ObjectId != obj.Id))
                        permissions.Add(new RolePermissionsDTO
                        {
                            Id = null,
                            RoleId = roleId,
                            ObjectId = obj.Id,
                            ObjectName = obj.Name,
                            Permissions = SecurityPermissions.None
                        });
                }
                result = new RolePermissions(role, permissions);
            }

            if (result == null)
            {
                return NotFound(roleId);
            }

            return Ok(result);
        }

        [PermissionAuthorization("USER", SecurityPermissions.Add)]
        [HttpPost("roles/save")]
        public async Task<IActionResult> SaveRole([FromBody] SaveRole command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok();
        }

        [PermissionAuthorization("USER", SecurityPermissions.Grant)]
        [HttpPost("permissions/save")]
        public async Task<IActionResult> SavePermissions([FromBody] SavePermissions command, CancellationToken token)
        {
            await _commandBus.Send(command, token);
            return Ok();
        }
    }
}
