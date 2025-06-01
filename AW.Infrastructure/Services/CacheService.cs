using AW.Core.DTOs;
using AW.Infrastructure.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Infrastructure.Services
{
	public class CacheService : ICacheService
	{
		private readonly IMemoryCache _cache;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CacheService(IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
		{
			_cache = cache;
			_httpContextAccessor = httpContextAccessor;
		}

		public virtual T? Get<T>(string key) => _cache.TryGetValue(key, out var value) ? (T)value! : default;

		public virtual MessageGetList<T>? GetAll<T>(string key) => _cache.TryGetValue(key, out var value) ? (MessageGetList<T>)value! : default;

		public string GetAuthorization()
		{
			return _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? ""; 
		}

		public virtual void Set<T>(string key, T value, TimeSpan duration) => _cache.Set(key, value, duration);
		public virtual void Set<T>(string key, MessageGetList<T> value, TimeSpan duration) => _cache.Set(key, value, duration);
	}
}
