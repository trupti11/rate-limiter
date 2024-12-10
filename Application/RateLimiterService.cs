using Domain.Interfaces;
using System.Collections.Generic;
using System;
using System.Linq;

namespace RateLimiter.Application;

public class RateLimiterService
{
    private readonly List<IRateLimitRule> _rules;

    public RateLimiterService(List<IRateLimitRule> rules)
    {
        _rules = rules;
    }

    public bool IsRequestAllowed(string accessToken, DateTime requestTime)
    {
        return _rules.All(rule => rule.IsRequestAllowed(accessToken, requestTime));
    }
}
