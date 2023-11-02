using System.Runtime.Serialization;

namespace Vayosoft.Commons.Enums
{
    [Flags]
    public enum DayOfWeekFlag
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Sunday = 1,
        [EnumMember]
        Monday = 2,
        [EnumMember]
        Tuesday = 4,
        [EnumMember]
        Wednesday = 8,
        [EnumMember]
        Thursday = 16,
        [EnumMember]
        Friday = 32,
        [EnumMember]
        Saturday = 64
    }
}
