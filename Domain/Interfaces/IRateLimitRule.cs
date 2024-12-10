using System;
using System.Collections.Generic;

namespace Domain.Interfaces;

public interface IRateLimitRule
{
    bool IsRequestAllowed(string token, DateTime requestTime);
}