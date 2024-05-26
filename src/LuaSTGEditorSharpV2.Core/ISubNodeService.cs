﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaSTGEditorSharpV2.Core.Model;

namespace LuaSTGEditorSharpV2.Core
{
    public interface ISubNodeService<out TOutput, in TContext>
        where TContext : NodeContext
    {
        public TOutput CreateOutput(NodeData nodeData, TContext context);
    }
}