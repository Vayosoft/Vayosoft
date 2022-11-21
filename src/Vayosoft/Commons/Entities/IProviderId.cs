﻿namespace Vayosoft.Commons.Entities
{
    public interface IProviderId
    {
        object ProviderId { get; }
    }

    public interface IProviderId<out TKey> : IProviderId
    {
        new TKey ProviderId { get; }
    }
}
