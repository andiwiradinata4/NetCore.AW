using AW.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AW.Infrastructure.Interfaces.Services
{
	public interface ICacheService
	{
		T? Get<T>(string key);
		MessageGetList<T>? GetAll<T>(string key);
		void Set<T>(string key, T value, TimeSpan duration);
		void Set<T>(string key, MessageGetList<T> value, TimeSpan duration);
		string GetAuthorization();
	}
}
