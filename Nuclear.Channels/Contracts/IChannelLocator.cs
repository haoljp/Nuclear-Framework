﻿using System;
using System.Collections.Generic;

namespace Nuclear.Channels.Contracts
{
    /// <summary>
    /// Service that contains all Channels
    /// </summary>
    internal interface IChannelLocator
    {

        /// <summary>
        /// Method that get all Channels
        /// </summary>
        /// <param name="domain">Domain with all assemblies</param>
        /// <returns>List of classes that are decorated with ChannelAttribute</returns>
        List<Type> RegisteredChannels(AppDomain domain);
    }
}
