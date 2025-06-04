using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AW.Core.Entities.Interface;
using Newtonsoft.Json.Linq;

namespace AW.Infrastructure.Utils
{
    public class Helper
    {
        public static void setPostData(IBaseEntityStandard entity, IPrincipal user)
        {
            entity.CreatedDate = DateTime.Now;
            entity.CreatedDateUTC = DateTime.UtcNow;
            entity.LogDate = DateTime.Now;
            entity.LogDateUTC = DateTime.UtcNow;
            if (user != null)
            {
                //entity.CreatedBy = user.Identity?.Name ?? "";
                //entity.CreatedByUserDisplayName = getValueFromClaims(user, "userDisplayName", user.Identity?.Name ?? "");
                //entity.LogBy = user.Identity?.Name ?? "";
                //entity.LogByUserDisplayName = getValueFromClaims(user, "userDisplayName", user.Identity?.Name ?? "");

                entity.CreatedBy = getValueFromClaims(user, "nameidentifier", user.Identity?.Name ?? "");
                entity.CreatedByUserDisplayName = getValueFromClaims(user, "name", user.Identity?.Name ?? "");
                entity.LogBy = getValueFromClaims(user, "nameidentifier", user.Identity?.Name ?? "");
                entity.LogByUserDisplayName = getValueFromClaims(user, "name", user.Identity?.Name ?? "");
            }
        }

        public static void setPostDataForNonBaseEntity(object entity, IPrincipal user)
        {
            //Type type = entity.GetType();
            //PropertyInfo property = type.GetProperty("CreatedDate");
            //PropertyInfo property3 = type.GetProperty("CreatedDateUTC");
            //PropertyInfo property2 = type.GetProperty("LogDate");
            //PropertyInfo property4 = type.GetProperty("LogDateUTC");
            //PropertyInfo property5 = type.GetProperty("CreatedBy");
            //PropertyInfo property6 = type.GetProperty("LogBy");
            //PropertyInfo property7 = type.GetProperty("CreatedByUserDisplayName");
            //PropertyInfo property8 = type.GetProperty("LogByUserDisplayName");
            //property.SetValue(entity, DateTime.Now);
            //property2.SetValue(entity, DateTime.Now);
            //property3.SetValue(entity, DateTime.UtcNow);
            //property4.SetValue(entity, DateTime.UtcNow);
            //if (user != null)
            //{
            //    property5.SetValue(entity, user.Identity!.Name);
            //    property6.SetValue(entity, user.Identity!.Name);
            //    property7.SetValue(entity, getValueFromClaims(user, "userDisplayName", user.Identity?.Name ?? ""));
            //    property8.SetValue(entity, getValueFromClaims(user, "userDisplayName", user.Identity?.Name ?? ""));
            //}
        }

        public static void setPutData(IBaseEntityStandard entity, IPrincipal user)
        {
            entity.LogDate = DateTime.Now;
            entity.LogDateUTC = DateTime.UtcNow;
            if (user != null)
            {
                entity.LogBy = getValueFromClaims(user, "nameidentifier", user.Identity?.Name ?? "");

                ///NameIdentifier
                ///userDisplayName
                entity.LogByUserDisplayName = getValueFromClaims(user, "name", user.Identity?.Name ?? "");
            }

        }

        public static void setPutDataForNonBaseEntity(object entity, IPrincipal user)
        {
            //Type type = entity.GetType();
            //PropertyInfo property = type.GetProperty("LogDate");
            //PropertyInfo property2 = type.GetProperty("LogDateUTC");
            //PropertyInfo property3 = type.GetProperty("LogBy");
            //PropertyInfo property4 = type.GetProperty("LogByUserDisplayName");
            //property.SetValue(entity, DateTime.Now);
            //property2.SetValue(entity, DateTime.UtcNow);
            //if (user != null)
            //{
            //    property3.SetValue(entity, user.Identity!.Name);
            //    property4.SetValue(entity, getValueFromClaims(user, "userDisplayName", user.Identity!.Name));
            //}
        }

        public static string getValueFromClaims(IPrincipal user, string claimType, string defaultValue = "")
        {
            try
            {
                if (user != null)
                {
                    ClaimsPrincipal? user2 = user as ClaimsPrincipal;
                    return getValueFromClaims(user2, claimType, defaultValue);
                }
            }
            catch
            {
            }

            return defaultValue;
        }

        public static string getValueFromClaims(ClaimsPrincipal? user, string claimType, string defaultValue = "")
        {
            try
            {
                if (user != null && user.Claims != null && user.Claims.Count() > 0)
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    Claim claim = user.Claims.Where((x) => x.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (claim == null) claim = user.Claims.Where((x) => x.Type.Contains(claimType, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (claim != null && !string.IsNullOrEmpty(claim.Value))
                    {
                        return claim.Value;
                    }
                }
            }
            catch
            {
            }
            return defaultValue;
        }

        public static string GetDateTimeFormat_yyyyMMddHHmmssffff() => DateTime.Now.ToString("yyyyMMddHHmmssffff");
        public static string GetDateTimeFormat_yyyyMMddHHmmssffff(DateTime date) => date.ToString("yyyyMMddHHmmssffff");

        public static void EnrichWithReferences(JArray mainData, JArray referencesData, string mainKey, string referencesKey, string targetPropertyName)
        {
            if (referencesData != null)
            {
                //var refDict = referencesData.Children<JObject>().Where(r => r[referencesKey] != null).GroupBy(r => r[referencesKey]!.ToString()).ToDictionary(r => r[referencesKey]?.ToString(), r => r);
                var refDict = referencesData.Children<JObject>().Where(r => r[referencesKey] != null).GroupBy(r => r[referencesKey]!.ToString()).ToDictionary(g => g.Key, g => g.First());
                if (refDict != null)
                {
                    foreach (var item in mainData.Children<JObject>())
                    {
                        var key = item[mainKey]?.ToString();
                        if (key != null && refDict.TryGetValue(key, out var match))
                        {
                            if (match != null) item[targetPropertyName] = JObject.FromObject(match);
                        }
                    }
                }
            }
        }

    }
}
