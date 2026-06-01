using System.Collections.Concurrent;
using CBBSimulator.Core.Simulation;

namespace CBBSimulator.Web.Services;

/// <summary>
/// Holds the live state of one interactive season per connection. The gate
/// serializes step calls (so a manual "sim day" can't interleave with an
/// in-flight "sim to end" run), and the cancellation source lets a running
/// "sim to end" loop be stopped.
/// </summary>
public sealed class SeasonSession
{
    public required string SeasonId { get; init; }
    public required string ConnectionId { get; init; }
    public required SeasonSimulation Simulation { get; init; }

    public SemaphoreSlim Gate { get; } = new(1, 1);
    public CancellationTokenSource? RunCts { get; set; }

    public void CancelRun()
    {
        RunCts?.Cancel();
        RunCts?.Dispose();
        RunCts = null;
    }
}

/// <summary>
/// Registry of active season sessions, keyed by season id. Registered as a
/// singleton so session state survives across individual hub method calls.
/// </summary>
public sealed class SeasonSessionManager
{
    private readonly ConcurrentDictionary<string, SeasonSession> _sessions = new();

    public SeasonSession Add(SeasonSession session)
    {
        _sessions[session.SeasonId] = session;
        return session;
    }

    public bool TryGet(string seasonId, out SeasonSession session)
        => _sessions.TryGetValue(seasonId, out session!);

    public void Remove(string seasonId)
    {
        if (_sessions.TryRemove(seasonId, out var session))
        {
            session.CancelRun();
            session.Gate.Dispose();
        }
    }

    public void RemoveByConnection(string connectionId)
    {
        foreach (var entry in _sessions)
        {
            if (entry.Value.ConnectionId == connectionId)
            {
                Remove(entry.Key);
            }
        }
    }
}
