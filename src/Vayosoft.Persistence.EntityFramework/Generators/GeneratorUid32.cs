using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vayosoft.Utilities;

namespace Vayosoft.Persistence.EntityFramework.Generators
{
    public class GeneratorUid32: ValueGenerator<string>
    {
        public override string Next(EntityEntry entry)
        {
            return GuidUtils.CreateUid();
        }

        public override bool GeneratesTemporaryValues => false;
    }
}
