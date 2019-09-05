using System;
using System.Threading.Tasks;

namespace Magis.School.ApiClient.DataObjects.Base
{
    public interface IDataObject : IDisposable
    {
        bool Loading { get; }

        bool Loaded { get; }

        Task EnsureLoadedAsync();

        Task ReloadAsync();

        Task ResetAsync();
    }
}
