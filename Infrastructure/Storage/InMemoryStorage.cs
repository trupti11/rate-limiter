using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateLimiterApp.Infrastructure.Storage;

public static class InMemoryStorage
{
    public static Dictionary<string, DateTime> RequestLog = new Dictionary<string, DateTime>();
}
