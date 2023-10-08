namespace Reoria.Application.Enumerations;

public enum ApplicationState
{
    Constructed = 1 << 0,
    Starting = 1 << 1,
    Running = 1 << 2,
    Pausing = 1 << 3,
    Paused = 1 << 4,
    Resuming = 1 << 5,
    Stopping = 1 << 6,
    Stopped =  1 << 7
}
