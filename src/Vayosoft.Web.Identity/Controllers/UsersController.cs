using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vayosoft.Commands;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Identity;
using Vayosoft.Identity.EntityFramework;
using Vayosoft.Identity.Extensions;
using Vayosoft.Identity.Security;
using Vayosoft.Identity.Security.Commands;
using Vayosoft.Identity.Security.Models;
using Vayosoft.Identity.Specifications;
using Vayosoft.Persistence.Commands;
using Vayosoft.Persistence.Queries;
using Vayosoft.Queries;
using Vayosoft.Utilities;
using Vayosoft.Web.Controllers;
using Vayosoft.Web.Identity.Authorization;
using Vayosoft.Web.Model;

namespace Vayosoft.Web.Identity.Controllers
{
    [Produces("application/json")]
    [ProducesResponseType(typeof(HttpErrorWrapper), StatusCodes.Status401Unauthorized)]
    [ProducesErrorResponseType(typeof(void))]
    [ApiVersion("1.0")]
    [Route("api/users")]
    public class UsersController : ApiControllerBase
    {
        private readonly ICommandBus commandBus;
        private readonly IQueryBus queryBus;
        private readonly IUserContext userContext;
        private readonly IdentityContext _context;

        public UsersController(ICommandBus commandBus, IQueryBus queryBus, IUserContext userContext, IdentityContext context)
        {
            this.commandBus = commandBus;
            this.queryBus = queryBus;
            this.userContext = userContext;
            _context = context;
        }

        //[HttpGet]
        //[ProducesResponseType(typeof(PagedResponse<UserEntityDto>), StatusCodes.Status200OK)]
        //[PermissionAuthorization("USER", SecurityPermissions.View)]
        //public async Task<IActionResult> Get(int page, int size, string searchTerm = null, CancellationToken token = default)
        //{
        //    await userContext.LoadContextAsync();
        //    var providerId = !userContext.IsSupervisor
        //        ? Guard.NotNull(userContext.User.Identity?.GetProviderId())
        //        : null;
        //    var spec = new GetUsersSpec(page, size, providerId, searchTerm);
        //    var query = new SpecificationQuery<GetUsersSpec, IPagedEnumerable<UserEntityDto>>(spec);

        //    return Page(await queryBus.Send(query, token), size);
        //}

        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserEntityDto>), StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> Get(int page, int pageSize, string searchText = null, CancellationToken token = default)
        {
            await userContext.LoadContextAsync();
            var providerId = !userContext.IsSupervisor
                ? Guard.NotNull(userContext.User.Identity?.GetProviderId())
                : null;

            var queryable = _context.Set<UserEntity>()
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchText))
            {
                queryable = queryable
                    .Where(u => u.Username.Contains(searchText));
            }

            if (providerId != null)
            {
                queryable = queryable
                    .Where(u => u.ProviderId == providerId);
            }

            var query = queryable
                .OrderBy(u => u.Username)
                .Select(e => new UserEntityDto
            {
                Id = e.Id,
                Email = e.Email,
                Username = e.Username,
                CultureId = e.CultureId,
                Deregistered = e.Deregistered,
                LogLevel = e.LogLevel,
                Phone = e.Phone,
                ProviderId = e.ProviderId,
                Registered = e.Registered,
                Type = e.Type
            });

            var items = await query.Paginate(page, pageSize).ToArrayAsync(cancellationToken: token);
            var totalCount = await query.CountAsync(cancellationToken: token);

            return Page(new PagedList<UserEntityDto>(items, totalCount), pageSize);
        }

        //[HttpGet("{id}")]
        //[ProducesResponseType(typeof(UserEntityDto), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[PermissionAuthorization("USER", SecurityPermissions.View)]
        //public async Task<IActionResult> GetById(ulong id, CancellationToken token)
        //{
        //    var query = new SingleQuery<UserEntityDto>(id);
        //    UserEntityDto result;
        //    if ((result = await queryBus.Send(query, token)) is null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(result);
        //}

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserEntityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [PermissionAuthorization("USER", SecurityPermissions.View)]
        public async Task<IActionResult> GetById(ulong id, CancellationToken token)
        {
            var e = await _context.Set<UserEntity>()
                .AsNoTracking()
                .Where(e => e.Id.Equals(id))
                .SingleOrDefaultAsync(cancellationToken: token);

            if (e == null)
            {
                return NotFound();
            }

            return Ok(new UserEntityDto
            {
                Id = e.Id,
                Email = e.Email,
                Username = e.Username,
                CultureId = e.CultureId,
                Deregistered = e.Deregistered,
                LogLevel = e.LogLevel,
                Phone = e.Phone,
                ProviderId = e.ProviderId,
                Registered = e.Registered,
                Type = e.Type
            });
        }

        [HttpPost("set")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.Add | SecurityPermissions.Edit)]
        public async Task<IActionResult> Post([FromBody] SaveUser command, CancellationToken token)
        {
            await commandBus.Send(command, token);
            return Ok();
        }

        [HttpPost("delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [PermissionAuthorization("USER", SecurityPermissions.Delete)]
        public async Task<IActionResult> PostDelete([FromBody] UserEntityDto entity, CancellationToken token)
        {
            var command = new DeleteCommand<UserEntity>(new UserEntity(entity.Username) { Id = entity.Id });
            await commandBus.Send(command, token);
            return Ok();
        }

        
    }
}