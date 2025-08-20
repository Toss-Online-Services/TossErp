using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Shared.Infrastructure.Idempotency;

/// <summary>
/// Simple in-memory idempotency cache middleware. Not production grade (no eviction persistence),
/// but provides a deterministic response replay for identical requests carrying an Idempotency-Key header.
/// </summary>
public class IdempotencyMiddleware
{
    private const string HeaderName = "Idempotency-Key";
    private readonly RequestDelegate _next;
    private static readonly ConcurrentDictionary<string, CachedResponse> Cache = new();
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var keyValues))
        {
            await _next(context);
            return;
        }
        var key = keyValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(key))
        {
            await _next(context);
            return;
        }

        var cacheKey = BuildCompositeKey(context, key);
        if (Cache.TryGetValue(cacheKey, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
        {
            context.Response.StatusCode = cached.StatusCode;
            foreach (var header in cached.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }
            await context.Response.Body.WriteAsync(cached.Body, 0, cached.Body.Length);
            return;
        }

        // Capture response
        var originalBody = context.Response.Body;
        await using var mem = new MemoryStream();
        context.Response.Body = mem;
        await _next(context);
        await context.Response.Body.FlushAsync();
        var bodyBytes = mem.ToArray();
        var headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
        var entry = new CachedResponse
        {
            StatusCode = context.Response.StatusCode,
            Body = bodyBytes,
            Headers = headers,
            ExpiresAt = DateTimeOffset.UtcNow.Add(DefaultTtl)
        };
        Cache[cacheKey] = entry;
        mem.Position = 0;
        context.Response.Body = originalBody;
        await context.Response.Body.WriteAsync(bodyBytes, 0, bodyBytes.Length);
    }

    private static string BuildCompositeKey(HttpContext ctx, string idempotencyKey)
    {
        // Include method + path + hash of body bytes for safety
        string bodyHash = string.Empty;
        if (ctx.Request.ContentLength > 0 && ctx.Request.Body.CanSeek)
        {
            long pos = ctx.Request.Body.Position;
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(ctx.Request.Body);
            bodyHash = Convert.ToHexString(hash);
            ctx.Request.Body.Position = pos;
        }
        return $"{ctx.Request.Method}:{ctx.Request.Path}:{idempotencyKey}:{bodyHash}";
    }

    private class CachedResponse
    {
        public int StatusCode { get; set; }
        public byte[] Body { get; set; } = Array.Empty<byte>();
        public Dictionary<string,string> Headers { get; set; } = new();
        public DateTimeOffset ExpiresAt { get; set; }
    }
}

public static class IdempotencyExtensions
{
    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app) => app.UseMiddleware<IdempotencyMiddleware>();
}
