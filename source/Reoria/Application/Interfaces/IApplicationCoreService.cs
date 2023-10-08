using Reoria.Application.Enumerations;

namespace Reoria.Application.Interfaces;

public interface IApplicationCoreService
{
    ApplicationServiceState State { get; }

    void Start();
    void Stop();
}