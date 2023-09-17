﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaSTGEditorSharpV2.Core.Model;

namespace LuaSTGEditorSharpV2.Core.Building
{
    /// <summary>
    /// Provide functionality of gathering resources from <see cref="NodeData"/>.
    /// </summary>
    [ServiceShortName("resg"), ServiceName("ResourceGathering")]
    public class ResourceGatheringServiceBase 
        : NodeService<ResourceGatheringServiceBase, ResourceGatheringContext, ResourceGatheringServiceSettings>
    {
        private static readonly ResourceGatheringServiceBase _default = new();
        private static readonly Dictionary<string, string> _empty = new Dictionary<string, string>();

        static ResourceGatheringServiceBase()
        {
            _defaultServiceGetter = () => _default;
        }

        public static IEnumerable<GroupedResource> GetResourcesToPack(NodeData nodeData, LocalServiceParam settings)
        {
            var ctx = GetContextOfNode(nodeData, settings);
            var service = GetServiceOfNode(nodeData);
            return service.GetResourcesToPackWithContext(nodeData, ctx);
        }

        public static IEnumerable<GroupedResource> ProceedChildren(NodeData node
            , ResourceGatheringContext context)
        {
            context.Push(node);
            foreach (NodeData child in node.GetLogicalChildren())
            {
                foreach (GroupedResource s in GetServiceOfNode(child).GetResourcesToPackWithContext(child, context))
                {
                    yield return s;
                }
            }
            context.Pop();
        }

        public override sealed ResourceGatheringContext GetEmptyContext(LocalServiceParam localSettings)
        {
            return new ResourceGatheringContext(localSettings, ServiceSettings);
        }

        public virtual IEnumerable<GroupedResource> GetResourcesToPackWithContext(NodeData node
            , ResourceGatheringContext context)
        {
            return ProceedChildren(node, context);
        }
    }
}