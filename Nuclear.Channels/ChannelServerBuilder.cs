﻿// Copyright © Nikola Milinkovic 
// Licensed under the MIT License (MIT).
// See License.md in the repository root for more information.

using Nuclear.Channels.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclear.Channels
{
    /// <summary>
    /// Class that contains method for building channel host
    /// </summary>
    public class ChannelServerBuilder
    {
        /// <summary>
        /// Method that creates IChannelHost instance
        /// </summary>
        public static IChannelServer CreateServer()
        {
            return ChannelHost.GetHost;
        }
    }
}