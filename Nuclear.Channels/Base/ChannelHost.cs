﻿using Nuclear.Channels.Hosting;
using Nuclear.ExportLocator.Services;
using System;
using System.Threading.Tasks;

namespace Nuclear.Channels.Base
{
    /// <summary>
    /// Implementation of ChannelHosting
    /// </summary>
    public class ChannelHost : IChannelHost
    {
        private AppDomain _domain;
        private IServiceLocator Services;
        private static ChannelHost _host;

        /// <summary>
        /// Get the Singleton Instance
        /// </summary>
        public static ChannelHost GetHost
        {
            get
            {
                if (_host == null)
                {
                    _host = new ChannelHost();
                }
                return _host;
            }
        }

        /// <summary>
        /// Singleton Design pattern private constructor
        /// </summary>
        private ChannelHost()
        {
            Services = ServiceLocator.GetInstance;
        }

        /// <summary>
        /// Method to load all assemblies containing channels
        /// </summary>
        /// <param name="domain">Current AppComain</param>
        /// <param name="assemblies">Assembly names of your projects</param>
        public void LoadAssemblies(AppDomain domain, string[] assemblies)
        {
            _domain = domain;
            if (assemblies != null && assemblies.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    _domain.Load(assembly);
                }
            }
        }

        /// <summary>
        /// Starts hosting
        /// </summary>
        public void StartHosting(string baseURL)
        {
            Services.Get<IChannelActivator>().Execute(_domain, Services, baseURL);
            Task.WaitAll();
        }
    }
}