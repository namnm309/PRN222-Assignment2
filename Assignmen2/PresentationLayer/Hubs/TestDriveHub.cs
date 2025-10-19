using System;
using System.Collections.Concurrent;
using BusinessLayer.DTOs.Requests;
using BusinessLayer.DTOs.Responses;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PresentationLayer.Hubs
{
    [AllowAnonymous]
    public class TestDriveHub : Hub
    {
        private readonly ITestDriveService _service;
        private static readonly ConcurrentDictionary<string, (string ConnectionId, DateTime ExpiresAtUtc)> _holds = new();
        private static readonly TimeSpan HoldTtl = TimeSpan.FromMinutes(3);
        public TestDriveHub(ITestDriveService service)
        {
            _service = service;
        }

        private static string GroupKey(Guid dealerId, Guid productId) => $"{dealerId}:{productId}";
        private static string SlotKey(Guid dealerId, Guid productId, DateTime scheduledUtc) => $"{dealerId}:{productId}:{scheduledUtc:O}";
        private static DateTime NormalizeUtc(DateTime dt) => dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dt, DateTimeKind.Local).ToUniversalTime()
        };
        private static void CleanupExpiredHolds()
        {
            var now = DateTime.UtcNow;
            foreach (var kv in _holds)
            {
                if (kv.Value.ExpiresAtUtc <= now)
                {
                    _holds.TryRemove(kv.Key, out _);
                }
            }
        }

        public async Task JoinGroup(Guid dealerId, Guid productId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupKey(dealerId, productId));
        }

        public async Task LeaveGroup(Guid dealerId, Guid productId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupKey(dealerId, productId));
        }

        public async Task<BookResult> BookSlot(BookTestDriveRequest req)
        {
            var (success, error, td) = await _service.CreatePublicAsync(req.CustomerName, req.CustomerPhone, req.CustomerEmail, req.Notes, req.ProductId, req.DealerId, req.ScheduledDate);
            if (success && td != null)
            {
                await Clients.Group(GroupKey(req.DealerId, req.ProductId)).SendAsync("SlotBooked", td.ScheduledDate);
                // clear hold (if any)
                var key = SlotKey(req.DealerId, req.ProductId, NormalizeUtc(td.ScheduledDate));
                _holds.TryRemove(key, out _);
                return new BookResult { Success = true, TestDriveId = td.Id, ScheduledDate = td.ScheduledDate };
            }
            else
            {
                // release hold to avoid deadlock
                var key = SlotKey(req.DealerId, req.ProductId, NormalizeUtc(req.ScheduledDate));
                if (_holds.TryGetValue(key, out var hold) && hold.ConnectionId == Context.ConnectionId)
                {
                    _holds.TryRemove(key, out _);
                    await Clients.Group(GroupKey(req.DealerId, req.ProductId)).SendAsync("SlotReleased", req.ScheduledDate);
                }
            }
            return new BookResult { Success = false, Error = error };
        }

        public async Task<List<DateTime>> GetDisabledSlots(Guid dealerId, Guid productId, DateTime from, DateTime to)
        {
            CleanupExpiredHolds();
            var (success, _, data) = await _service.GetScheduledInRangeAsync(dealerId, productId, from, to);
            var result = success ? new List<DateTime>(data) : new List<DateTime>();
            // add held slots in range
            var gkeyPrefix = $"{dealerId}:{productId}:";
            foreach (var kv in _holds)
            {
                if (!kv.Key.StartsWith(gkeyPrefix)) continue;
                var parts = kv.Key.Split(':');
                // last part is ISO datetime
                if (DateTime.TryParse(parts[^1], null, System.Globalization.DateTimeStyles.RoundtripKind, out var heldUtc))
                {
                    if (heldUtc >= NormalizeUtc(from) && heldUtc <= NormalizeUtc(to))
                    {
                        result.Add(heldUtc);
                    }
                }
            }
            return result;
        }

        public async Task<bool> HoldSlot(Guid dealerId, Guid productId, DateTime scheduledDate)
        {
            CleanupExpiredHolds();
            var scheduledUtc = NormalizeUtc(scheduledDate);
            var key = SlotKey(dealerId, productId, scheduledUtc);
            var now = DateTime.UtcNow;
            var expires = now.Add(HoldTtl);

            // If already held by other connection and not expired -> deny
            if (_holds.TryGetValue(key, out var existing))
            {
                if (existing.ExpiresAtUtc > now && existing.ConnectionId != Context.ConnectionId)
                {
                    return false;
                }
            }

            _holds[key] = (Context.ConnectionId, expires);
            await Clients.Group(GroupKey(dealerId, productId)).SendAsync("SlotHeld", scheduledUtc);
            return true;
        }

        public async Task ReleaseSlot(Guid dealerId, Guid productId, DateTime scheduledDate)
        {
            var scheduledUtc = NormalizeUtc(scheduledDate);
            var key = SlotKey(dealerId, productId, scheduledUtc);
            if (_holds.TryGetValue(key, out var existing) && existing.ConnectionId == Context.ConnectionId)
            {
                _holds.TryRemove(key, out _);
                await Clients.Group(GroupKey(dealerId, productId)).SendAsync("SlotReleased", scheduledUtc);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // release all holds by this connection
            var toRelease = new List<(Guid DealerId, Guid ProductId, DateTime ScheduledUtc)>();
            foreach (var kv in _holds)
            {
                if (kv.Value.ConnectionId == Context.ConnectionId)
                {
                    var parts = kv.Key.Split(':');
                    if (parts.Length >= 3 && Guid.TryParse(parts[0], out var d) && Guid.TryParse(parts[1], out var p)
                        && DateTime.TryParse(parts[2], null, System.Globalization.DateTimeStyles.RoundtripKind, out var s))
                    {
                        toRelease.Add((d, p, s));
                    }
                }
            }

            foreach (var item in toRelease)
            {
                _holds.TryRemove(SlotKey(item.DealerId, item.ProductId, item.ScheduledUtc), out _);
                await Clients.Group(GroupKey(item.DealerId, item.ProductId)).SendAsync("SlotReleased", item.ScheduledUtc);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
