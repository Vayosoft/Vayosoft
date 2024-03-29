﻿using Grpc.Core;

namespace Vayosoft.gRPC.Reactive;

public class GrpcStreamObservable<T> : IObservable<T>
{
    private readonly IAsyncStreamReader<T> _reader;
    private readonly CancellationToken _token;
    private int _used;

    public GrpcStreamObservable(IAsyncStreamReader<T> reader, CancellationToken token = default)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _token = token;
        _used = 0;
    }

    public IDisposable Subscribe(IObserver<T> observer) =>
        Interlocked.Exchange(ref _used, 1) == 0
            ? new GrpcStreamSubscription<T>(_reader, observer, _token)
            : throw new InvalidOperationException("Subscribe can only be called once.");

}