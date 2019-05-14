using System;
using System.Collections.Generic;

namespace FutureConcepts.Media.Server
{
    /// <summary>
    /// This class is used to manage access to resources.
    /// </summary>
    /// <author>kdixon 02/11/2009</author>
    public static class ResourceManager
    {
        private static readonly object _lock = new object();

        private static Dictionary<string, Dictionary<Type, IResourceAccessRule>> accessControl = new Dictionary<string, Dictionary<Type, IResourceAccessRule>>();

        /// <summary>
        /// Attempts to acquire a particular type of resource, using the specified access rule.
        /// If another rule already exists for the requested resource, returns false.
        /// If another rule already exists, but has no clients, the rule is changed to the one specified.
        /// If a rule does not exist, then the specified rule is used
        /// </summary>
        /// <typeparam name="T">type of resource access rule to acquire</typeparam>
        /// <param name="sourceName">source name associated with the resource</param>
        /// <param name="type">type of resource</param>
        /// <param name="ownerName">friendly name of entity that wants to acquire the resource</param>
        /// <param name="rule">type/instance of the rule to use</param>
        /// <returns>true if the resource was acquired using the specified rule, false otherwise</returns>
        public static bool Acquire<T>(string sourceName, Type type, string ownerName) where T : IResourceAccessRule
        {
            lock (_lock)
            {
                if (!accessControl.ContainsKey(sourceName))
                {
                    accessControl.Add(sourceName, new Dictionary<Type, IResourceAccessRule>());
                }

                if (!accessControl[sourceName].ContainsKey(type))
                {
                    accessControl[sourceName].Add(type, Activator.CreateInstance<T>());
                }
                else if (!(accessControl[sourceName][type] is T) && (accessControl[sourceName][type].HasNoClients))
                {
                    accessControl[sourceName][type] = Activator.CreateInstance<T>();
                }

                if (accessControl[sourceName][type] is T)
                {
                    return accessControl[sourceName][type].Acquire(ownerName);
                }

                return false;
            }
        }

        /// <summary>
        /// Attempts to acquire a resource. If no rule exists for the resource, a MutexRule will be created.
        /// </summary>
        /// <param name="sourceName">source name associated with the resource</param>
        /// <param name="type">type of resource</param>
        /// <param name="ownerName">friendly name of entity that wants to acquire the resource</param>
        /// <returns>true if acquired, false if not</returns>
        public static bool Acquire(string sourceName, Type type, string ownerName)
        {
            lock (_lock)
            {
                if (!accessControl.ContainsKey(sourceName))
                {
                    accessControl.Add(sourceName, new Dictionary<Type, IResourceAccessRule>());
                }

                if (!accessControl[sourceName].ContainsKey(type))
                {
                    accessControl[sourceName].Add(type, new MutexRule());
                }

                return accessControl[sourceName][type].Acquire(ownerName);
            }
        }

        /// <summary>
        /// Releases a resource
        /// </summary>
        /// <param name="sourceName">source name associated with resource</param>
        /// <param name="type">type of resource</param>
        /// <param name="ownerName">owner name who is releasing the resource</param>
        /// <exception cref="System.InvalidOperationException">Thrown if ownerName does not own the resource</exception>
        public static void Release(string sourceName, Type type, string ownerName)
        {
            lock (_lock)
            {
                if (accessControl.ContainsKey(sourceName))
                {
                    if (accessControl[sourceName].ContainsKey(type))
                    {
                        accessControl[sourceName][type].Release(ownerName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the owner of the resource
        /// </summary>
        /// <param name="sourceName">source name associated with resource</param>
        /// <param name="type">type of resource</param>
        /// <returns>null if no owners</returns>
        public static string GetOwner(string sourceName, Type type)
        {
            lock (_lock)
            {
                if (accessControl.ContainsKey(sourceName))
                {
                    if (accessControl[sourceName].ContainsKey(type))
                    {
                        return accessControl[sourceName][type].Owner;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Removes a rule on a given resource if there are no clients
        /// </summary>
        /// <param name="sourceName">name of the source</param>
        /// <param name="type">type governed by a rule</param>
        /// <returns>true if the rule was removed, false if no rule existed or if it has clients</returns>
        public static bool RemoveRule(string sourceName, Type type)
        {
            lock (_lock)
            {
                try
                {
                    if (accessControl[sourceName][type].HasNoClients)
                    {
                        accessControl[sourceName].Remove(type);
                        return true;
                    }
                }
                catch
                {
                    //doh
                }
                return false;
            }
        }

        #region Resource Access Rules

        public interface IResourceAccessRule
        {
            /// <summary>
            /// Attempts to acquire the resource
            /// </summary>
            /// <param name="ownerName">name of entity owning the resource</param>
            /// <returns>returns true if acquired, returns false if not available</returns>
            bool Acquire(string ownerName);
            /// <summary>
            /// Releases the resource
            /// </summary>
            /// <param name="ownerName">name of the entity currently owning the resource</param>
            /// <exception cref="System.InvalidOperationException">Thrown if the specified ownerName does not own the resource</exception>
            void Release(string ownerName);
            /// <summary>
            /// Returns true if it is possible to acquire the resource
            /// </summary>
            bool CanAcquire { get; }
            /// <summary>
            /// Returns true if there are currently no acquisitions on this rule
            /// </summary>
            bool HasNoClients { get; }
            /// <summary>
            /// Fetches a string identifying the owner(s) of this resource.
            /// Returns null if there is/are no owner(s)
            /// </summary>
            string Owner { get; }
        }

        /// <summary>
        /// Implements a mutually exclusive access rule
        /// </summary>
        public class MutexRule : IResourceAccessRule
        {
            private readonly object _lock = new object();

            string owner = null;

            public bool Acquire(string ownerName)
            {
                lock (_lock)
                {
                    if (owner != null)
                    {
                        return false;
                    }
                    else
                    {
                        owner = ownerName;
                        return true;
                    }
                }
            }

            public void Release(string ownerName)
            {
                lock (_lock)
                {
                    if (owner.Equals(ownerName))
                    {
                        owner = null;
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot release a resource you do not own!");
                    }
                }
            }

            public bool CanAcquire
            {
                get
                {
                    return (owner == null);
                }
            }

            public bool HasNoClients
            {
                get
                {
                    return (owner == null);
                }
            }

            public string Owner
            {
                get
                {
                    return owner;
                }
            }
        }

        /// <summary>
        /// Implements a semaphore access rule
        /// </summary>
        public class SemaphoreRule : IResourceAccessRule
        {
            private readonly object _lock = new object();

            private int max;
            private List<string> users = new List<string>();

            public SemaphoreRule(int maximum)
            {
                this.max = maximum;
            }

            public bool Acquire(string ownerName)
            {
                lock (_lock)
                {
                    if (CanAcquire)
                    {
                        users.Add(ownerName);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            public void Release(string ownerName)
            {
                lock (_lock)
                {
                    if (!users.Remove(ownerName))
                    {
                        throw new InvalidOperationException("Cannot release a resource you do not own!");
                    }
                }
            }

            public bool CanAcquire
            {
                get { return (users.Count < max); }
            }

            public bool HasNoClients
            {
                get
                {
                    return (users.Count == 0);
                }
            }

            public string Owner
            {
                get
                {
                    if(users.Count == 0)
                    {
                        return null;
                    }

                    string output = users[0];
                    for (int i = 1; i < users.Count; i++)
                    {
                        output += Environment.NewLine + users[i];
                    }
                    return output;
                }
            }
        }

        #endregion
    }
}
