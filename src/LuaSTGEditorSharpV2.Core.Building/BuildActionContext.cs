﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaSTGEditorSharpV2.Core.Model;

namespace LuaSTGEditorSharpV2.Core.Building
{
    public class BuildActionContext : NodeContext<BuildActionServiceSettings>
    {
        public new LocalParams LocalSettings => base.LocalSettings;

        public BuildActionContext(LocalParams localSettings, BuildActionServiceSettings serviceSettings) 
            : base(localSettings, serviceSettings)
        {
        }
    }
}
