﻿namespace Vayosoft.Mapping
{
    public enum MapDirection : byte
    {
        EntityToDto, DtoToEntity, Both
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ConventionalMapAttribute : Attribute
    {
        public MapDirection Direction { get; set; }

        public Type EntityType { get; }

        public ConventionalMapAttribute(Type entityType, MapDirection direction = MapDirection.Both)
        {
            Direction = direction;
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }
    }
}
