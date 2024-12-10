using System;
using System.Collections.Generic;
using Domain.Interfaces;

namespace RateLimiter.Domain;

public class FixedWindowRateLimitRule : IRateLimitRule
{
    private readonly int _maxRequests;
    private readonly TimeSpan _timeSpan;
    private readonly Dictionary<string, List<DateTime>> _requestHistory;

    public FixedWindowRateLimitRule(int maxRequests, TimeSpan timeSpan)
    {
        _maxRequests = maxRequests;
        _timeSpan = timeSpan;
        _requestHistory = new Dictionary<string, List<DateTime>>();
    }

    public bool IsRequestAllowed(string token, DateTime requestTime)
    {
        if (!_requestHistory.ContainsKey(token))
            _requestHistory[token] = new List<DateTime>();

        var requests = _requestHistory[token];
        requests.RemoveAll(t => t > requestTime - _timeSpan);

        if (requests.Count >= _maxRequests)
            return false;

        requests.Add(requestTime);
        return true;
    }
}
