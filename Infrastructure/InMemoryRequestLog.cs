using System;
using System.Collections.Generic;

namespace RateLimiter.Infrastructure;

public class InMemoryRequestLog
{
    private readonly Dictionary<string, List<DateTime>> _log = new();

    public void LogRequest(string token, DateTime timestamp)
    {
        if (!_log.ContainsKey(token))
            _log[token] = new List<DateTime>();

        _log[token].Add(timestamp);
    }

    public List<DateTime> GetRequests(string token)
    {
        return _log.ContainsKey(token) ? _log[token] : new List<DateTime>();
    }
}
