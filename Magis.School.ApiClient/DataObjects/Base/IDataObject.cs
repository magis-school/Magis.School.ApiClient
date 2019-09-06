using System;
using System.Threading;
using System.Threading.Tasks;

namespace Magis.School.ApiClient.DataObjects.Base
{
    public interface IDataObject : IDisposable
    {
        bool Loading { get; }

        bool Loaded { get; }

        Task EnsureLoadedAsync(CancellationToken cancellationToken = default);

        Task ReloadAsync(CancellationToken cancellationToken = default);

        Task ResetAsync(CancellationToken cancellationToken = default);
    }
}
