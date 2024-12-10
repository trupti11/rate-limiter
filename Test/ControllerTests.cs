using Domain.Interfaces;
using RateLimiter.Application;
using RateLimiter.Domain;
using Xunit;

namespace RateLimiterApp.Test.UnitTests
{
    public class RateLimiterTests
    {
        [Fact]
        public void TestRequestFixedWindowRateLimitRuleAllowed()
        {
            var rule = new FixedWindowRateLimitRule(10, TimeSpan.FromSeconds(10));
            var service = new RateLimiterService(new List<IRateLimitRule> { rule });

            var accessToken = "test-token";
            var now = DateTime.UtcNow;

            Assert.True(service.IsRequestAllowed(accessToken, now));
            Assert.True(service.IsRequestAllowed(accessToken, now.AddSeconds(5)));
            Assert.True(service.IsRequestAllowed(accessToken, now.AddSeconds(15)));
        }

        [Fact]
        public void TestRequestFixedWindowRateLimitRuleNotAllowed()
        {
            var rule = new FixedWindowRateLimitRule(1, TimeSpan.FromSeconds(1));
            var service = new RateLimiterService(new List<IRateLimitRule> { rule });

            var accessToken = "test-token";
            var now = DateTime.UtcNow;

            Assert.True(service.IsRequestAllowed(accessToken, now));
            Assert.False(service.IsRequestAllowed(accessToken, now.AddSeconds(5)));
            Assert.False(service.IsRequestAllowed(accessToken, now.AddSeconds(15)));
        }
    }
}