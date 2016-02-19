using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Gym_Membership.Helpers
{

    public enum CacheExpiration
    {
        Midnight = 0,
        Fix = 1,
        Sliding = 2,
        In_2_Hours = 3

    }


    public class CacheHelper
    {

        /// <summary>
        /// Adds a cache object for a user
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        public static void AddObjectToCache(string key, object objectToInsert, DateTime? expirationTime, string userId, CacheExpiration expiration)
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));

            try
            {
                string cacheKey = String.Format("username={0};key={1};", userId, key);
                AddObjectToCache(cacheKey, objectToInsert, expirationTime, false, expiration);
            }
            catch (Exception e)
            {
                log.Error("[AddObjectToCache] - Exception Caught" + e.ToString());

                throw;
            }
        }


        /// <summary>
        /// This method add an object to cache with a default expiration date of 1day.
        /// </summary>
        /// <param name="key">The Cache key</param>
        /// <param name="objectToInsert">The object to save in cache</param>
        /// <param name="expirationTime">The time the cache should expire. the default is 1hour</param>
        public static void AddObjectToCache(string key, object objectToInsert, DateTime? expirationTime, bool ObjectIsPerPerson, CacheExpiration expiration)
        {

            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));
            try
            {
                MemoryCache cache = MemoryCache.Default;



                if (!expirationTime.HasValue)
                {
                    expirationTime = DateTime.Now.AddHours(1);
                }

                string keyForCache = key;

                if (ObjectIsPerPerson)
                {
                    keyForCache = GetKeyForUser(key);
                }

                CacheItemPolicy policy = new CacheItemPolicy();

                switch (expiration)
                {
                    case CacheExpiration.Midnight:
                        policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Today.AddDays(1.0));
                        break;
                    case CacheExpiration.Fix:
                        policy.AbsoluteExpiration = new DateTimeOffset(expirationTime.Value);
                        break;
                    case CacheExpiration.Sliding:
                        TimeSpan time = expirationTime.Value - DateTime.Now;
                        policy.SlidingExpiration = time;
                        break;
                    case CacheExpiration.In_2_Hours:
                        policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddHours(2));
                        break;
                    default:
                        break;
                }

                // configuration setting to determine if cache is used
                if (true)//(leads.Properties.Settings.Default.UseCache)
                {
                    log.Debug(String.Concat("AddObjectToCache : Cache key -> ", keyForCache));
                    cache.Set(keyForCache, objectToInsert, policy);
                }
            }
            catch (Exception e)
            {
                log.Error("[AddObjectToCache] - Exception Caught" + e.ToString());

                throw;
            }
        }

        /// <summary>
        /// Gets the cache object for a specific user
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        public static T GetDataFromCache<T>(string key, string userId) where T : class
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));
            try
            {
                string cacheKey = String.Format("username={0};key={1};", userId, key);
                return GetDataFromCache<T>(cacheKey, false);
            }
            catch (Exception e)
            {
                log.Error("[GetDataFromCache<T>] - Exception Caught" + e.ToString());

                throw;
            }
        }

        /// <summary>
        /// Gets data from cache, if data is not present, null is returned.
        /// It is casted as the type needed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="ObjectIsPerPerson"></param>
        /// <returns></returns>
        public static T GetDataFromCache<T>(string key, bool ObjectIsPerPerson) where T : class
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));

            try
            {
                MemoryCache cache = MemoryCache.Default;

                string keyForCache = key;

                if (ObjectIsPerPerson)
                {
                    keyForCache = GetKeyForUser(key);
                }

                if (cache[keyForCache] != null)
                {
                    log.Debug(String.Concat("GetDataFromCache : Cache key -> ", keyForCache));

                    return (T)cache[keyForCache];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("[GetDataFromCache<T>] - Exception Caught" + e.ToString());

                throw;
            }
        }

        public static void AddObjectToCache(string key, object objectToInsert, DateTime? expirationTime, string userId)
        {
            string cacheKey = String.Format("username={0};key={1};", userId, key);
            AddObjectToCache(cacheKey, objectToInsert, expirationTime, false);
        }


        /// <summary>
        /// This method add an object to cache with a default expiration date of 1day.
        /// </summary>
        /// <param name="key">The Cache key</param>
        /// <param name="objectToInsert">The object to save in cache</param>
        /// <param name="expirationTime">The time the cache should expire. the default is 1hour</param>
        public static void AddObjectToCache(string key, object objectToInsert, DateTime? expirationTime, bool ObjectIsPerPerson)
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));

            try
            {
                MemoryCache cache = MemoryCache.Default;

                if (!expirationTime.HasValue)
                {
                    expirationTime = DateTime.Now.AddHours(1);
                }

                string keyForCache = key;

                if (ObjectIsPerPerson)
                {
                    keyForCache = GetKeyForUser(key);
                }

                // configuration setting to determine if cache is used
                if (true)//(leads.Properties.Settings.Default.UseCache)
                {
                    log.Debug(String.Concat("AddObjectToCache : Cache key -> ", keyForCache));
                    cache.Set(keyForCache, objectToInsert, new DateTimeOffset(expirationTime.Value));
                }
            }
            catch (Exception e)
            {
                log.Error("[AddObjectToCache] - Exception Caught" + e.ToString());

                throw e;
            }
        }


        /// <summary>
        /// Removes the cache entry for a specific user
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        public static void RemoveObjectFromCache(string key, string userId)
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));

            try
            {
                string cacheKey = String.Format("username={0};key={1};", userId, key);
                RemoveObjectFromCache(cacheKey, false);
            }
            catch (Exception e)
            {
                log.Error("[GetDataFromCache<T>] - Exception Caught" + e.ToString());

                throw;
            }
        }


        public static bool RemoveObjectFromCache(string cacheKey)
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));

            try
            {
                //string cacheKey = String.Format("username={0};key={1};", userId, key);
                MemoryCache cache = MemoryCache.Default;
                if (cache[cacheKey] != null)
                {
                    log.Debug(String.Concat("RemoveObjectFromCache : Cache key -> ", cacheKey));
                    cache.Remove(cacheKey);
                    return true;
                }

                //object was not found so nothing removed
                return false;

            }
            catch (Exception e)
            {
                log.Error("[GetDataFromCache<T>] - Exception Caught" + e.ToString());

                throw;
            }
        }



        /// <summary>
        /// This method clears cache depending on key
        /// </summary>
        /// <param name="key">The key to be destroyed</param>
        /// <param name="ObjectIsPerPerson">If true, the key will be modified to matched the CURRENT logged person - meaning his personnal cache</param>
        /// <param name="refreshCorrespondingCacheForAllUsers">This will clear all cache having same key</param>
        public static void RemoveObjectFromCache(string key, bool ObjectIsPerPerson, bool refreshCorrespondingCacheForAllUsers = false)
        {
            ILog log = log4net.LogManager.GetLogger(typeof(CacheHelper));

            try
            {
                MemoryCache cache = MemoryCache.Default;
                List<string> lstCacheKeys = new List<string>();
                string cacheKey = key;
                if (ObjectIsPerPerson)
                {
                    cacheKey = GetKeyForUser(key);
                }

                lstCacheKeys.Add(cacheKey);

                if (refreshCorrespondingCacheForAllUsers)
                {
                    foreach (var cacheItem in cache)
                    {
                        if (cacheItem.Key.Contains(String.Format("key={1};", key)))
                        {
                            lstCacheKeys.Add(cacheItem.Key);
                        }
                    }
                }

                foreach (var cacheKeys in lstCacheKeys)
                {
                    if (cache[cacheKeys] != null)
                    {
                        log.Debug(String.Concat("RemoveObjectFromCache : Cache key -> ", key));
                        cache.Remove(cacheKeys);
                    }
                }

            }
            catch (Exception e)
            {
                log.Error("[RemoveObjectFromCache] - Exception Caught" + e.ToString());

                throw;
            }
        }

        /// <summary>
        /// This method flushes the cache.
        /// </summary>
        public static void FlushCache()
        {
            MemoryCache cache = MemoryCache.Default;
            cache.Dispose();
        }

        /// <summary>
        /// Build key for user
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetKeyForUser(string key)
        {
            string userName = UserSession.Current.Username;

            return String.Format("username={0};key={1};", userName, key);
        }
    }

    /// <summary>
    /// This Class contains all the keys for use in the caching system.
    /// </summary>
    public static class CacheHelperKeys
    {
        /// <summary>
        /// Used for all employees
        /// </summary>
        public static string CK_GET_CONTACTS_BY_USER(string userCacheKey)
        {
            return string.Concat("CK_GET_CONTACTS_BY_USER_", userCacheKey.ToUpper());
        }


        public static string CK_GET_MEMBER_BY_ID(int userCacheKey)
        {
            return string.Concat("CK_GET_MEMBER_BY_ID_", userCacheKey);
        }

        public static string CK_GET_MEMBERS_BY_DATE(DateTime userCacheKey)
        {
            return string.Concat("CK_GET_MEMBERS_BY_DATE_", userCacheKey.ToString("yyyyMMdd"));
        }


        /// <summary>
        /// Used for all employees
        /// </summary>
        //public readonly static string CK_GET_ALL_INSTIUTIONS = "CK_GET_ALL_INSTIUTIONS";



        public readonly static string CK_GET_USER_ORGANISATIONS = "CK_GET_USER_ORGANISATIONS";
        public readonly static string CK_GET_MEMBERS = "CK_GET_MEMBERS";
        public readonly static string CK_GET_MEMBERSHIP_TYPES = "CK_GET_MEMBERSHIP_TYPES";
        public readonly static string CK_GET_ADMINS = "CK_GET_ADMINS";

        }
}