namespace Vayosoft.Persistence.MongoDB
{
    public interface IDocumentUoW : IDisposable
    {
        public Task<bool> CommitAsync(CancellationToken cancellationToken = default);
    }
}
