﻿// Copyright © Nikola Milinkovic 
// Licensed under the MIT License (MIT).
// See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclear.Channels.Generators
{
    public interface IImportedServicesResolver
    {
        object GetImportedService(Type type);
    }
}
