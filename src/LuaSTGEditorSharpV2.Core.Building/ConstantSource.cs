﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuaSTGEditorSharpV2.Core.Building
{
    public class ConstantSource : IInputSourceVariable
    {
        public string[] Path { get; private set; }

        public ConstantSource(params string[] path)
        {
            Path = path;
        }

        public IReadOnlyList<string> GetVariable(BuildingContext context)
        {
            return Path;
        }
    }
}