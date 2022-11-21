using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Vayosoft.Persistence.EF.MySQL.Converters
{
    public static class ConverterExtensions
    {
        public static void UseUtcDateTimeConverter(this ModelBuilder modelBuilder)
        {
            foreach (var mutableEntityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var mutableProperty in mutableEntityType.GetProperties())
                {
                    if (mutableProperty.ClrType == typeof(DateTime)/* && mutableProperty.Name.EndsWith("Utc")*/)
                    {
                        mutableProperty.SetValueConverter(new UtcDateTimeConverter());
                    }
                }
            }
        }
    }

    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(
                toDb => toDb,
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc))
        { }
    }
}
