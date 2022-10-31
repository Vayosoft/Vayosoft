using System.Linq;
using AutoMapper.QueryableExtensions;
using Vayosoft.Commons;
using Vayosoft.Utilities;
using AM = AutoMapper;
using IMapper = Vayosoft.Commons.IMapper;

namespace Vayosoft.AutoMapper
{
    public class AutoMapperWrapper : Commons.IMapper, IProjector
    {
        public AM.IConfigurationProvider Configuration { get; private set; }
        public AM.IMapper Instance { get; private set; }

        public AutoMapperWrapper(AM.IConfigurationProvider configuration, bool skipValidnessAssert = false)
        {
            Configuration = Guard.NotNull(configuration, nameof(AM.IConfigurationProvider));

            if (!skipValidnessAssert)
            {
                Configuration.AssertConfigurationIsValid();
            }

            Instance = Configuration.CreateMapper();
        }

        public TReturn Map<TReturn>(object src) => Instance.Map<TReturn>(src);

        public TReturn Map<TReturn>(object src, TReturn dest) => Instance.Map(src, dest);

        public IQueryable<TReturn> Project<TSource, TReturn>(IQueryable<TSource> queryable)
            => queryable.ProjectTo<TReturn>(Configuration);
    }
}
