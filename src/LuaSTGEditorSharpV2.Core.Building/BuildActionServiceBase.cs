﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using LuaSTGEditorSharpV2.Core.Model;

namespace LuaSTGEditorSharpV2.Core.Building
{
    /// <summary>
    /// Provide functionality of creating build commands from <see cref="NodeData"/>.
    /// </summary>
    [ServiceShortName("build"), ServiceName("BuildAction")]
    public class BuildActionServiceBase 
        : NodeService<BuildActionServiceBase, BuildActionContext, BuildActionServiceSettings>
    {
        private static readonly BuildActionServiceBase _default = new();

        static BuildActionServiceBase()
        {
            _defaultServiceGetter = () => _default;
        }

        public static async Task BuildAsync(NodeData nodeData, LocalParams settings)
        {
            var ctx = GetContextOfNode(nodeData, settings);
            var service = GetServiceOfNode(nodeData);
            await service.BuildWithContextAsync(nodeData, ctx);
        }

        public static void Build(NodeData nodeData, LocalParams settings) 
            => BuildAsync(nodeData, settings).RunSynchronously();

        public static async Task ProceedChildrenAsync(NodeData node
            , BuildActionContext context)
        {
            context.Push(node);
            foreach (NodeData child in node.GetLogicalChildren())
            {
                await GetServiceOfNode(child).BuildWithContextAsync(child, context);
            }
            context.Pop();
        }

        public static void ProceedChildren(NodeData node, BuildActionContext context) 
            => ProceedChildrenAsync(node, context).RunSynchronously();

        public override sealed BuildActionContext GetEmptyContext(LocalParams localSettings)
        {
            return new BuildActionContext(localSettings, ServiceSettings);
        }

        public virtual async Task BuildWithContextAsync(NodeData node, BuildActionContext context)
        {
            await ProceedChildrenAsync(node, context);
        }

        public void BuildWithContext(NodeData node, BuildActionContext context)
            => BuildWithContextAsync(node, context).RunSynchronously();
    }
}
