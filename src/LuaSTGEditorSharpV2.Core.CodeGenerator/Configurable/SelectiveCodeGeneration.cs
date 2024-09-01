﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using LuaSTGEditorSharpV2.Core.Model;
using LuaSTGEditorSharpV2.Core.Configurable;

namespace LuaSTGEditorSharpV2.Core.CodeGenerator.Configurable
{
    [Serializable]
    public class SelectiveCodeGeneration(CodeGeneratorServiceProvider nodeServiceProvider, IServiceProvider serviceProvider) 
        : CodeGeneratorServiceBase(nodeServiceProvider, serviceProvider)
    {
        [JsonProperty] public CaptureWithMacroOption[] Captures { get; private set; } = [];
        [JsonProperty] public ContextCapture[] ContextCaptures { get; private set; } = [];
        [JsonProperty] public Selection[]? Head { get; private set; } = [];
        [JsonProperty] public Selection[]? Tail { get; private set; } = [];
        [JsonProperty] public bool IgnoreChildren { get; private set; } = false;
        [JsonProperty] public int IndentionIncrement { get; private set; } = 1;

        private string[]? _captureResult;

        internal protected override IEnumerable<CodeData> GenerateCodeWithContext(NodeData node, CodeGenerationContext context)
        {
            var token = new NodePropertyAccessToken(ServiceProvider, node, context);
            _captureResult ??= new string[GetCaptureCacheLength()];
            int n;
            for (n = 0; n < Captures.Length; n++)
            {
                _captureResult[n] = Captures[n].ApplyMacro(token, context);
            }
            for (int i = 0; i < ContextCaptures.Length; i++)
            {
                for (int j = 0; j < ContextCaptures[i].Property.Length; j++)
                {
                    var contextNode = context.PeekType(ContextCaptures[i].TypeUID);
                    if (contextNode != null)
                    {
                        _captureResult[n] = ContextCaptures[i].Property[j].ApplyMacro(token, context);
                    }
                    else
                    {
                        _captureResult[n] = string.Empty;
                    }
                    n++;
                }
            }
            if (Head != null)
            {
                StringBuilder sb = new();
                for (int i = 0; i < Head.Length; i++)
                {
                    if (Head[i].ShouldAppend(_captureResult))
                    {
                        sb.Append(context.ApplyIndentedFormat(context.GetIndented(), Head[i].Text, _captureResult));
                    }
                }
                yield return new CodeData(sb.ToString(), node);
            }
            if (!IgnoreChildren)
            {
                foreach (var cd in GetNodeServiceProvider().GenerateForChildren(node, context, IndentionIncrement))
                {
                    yield return cd;
                }
            }
            if (Tail != null)
            {
                StringBuilder sb = new();
                for (int i = 0; i < Tail.Length; i++)
                {
                    if (Tail[i].ShouldAppend(_captureResult))
                    {
                        sb.Append(context.ApplyIndentedFormat(context.GetIndented(), Tail[i].Text, _captureResult));
                    }
                }
                yield return new CodeData(sb.ToString(), node);
            }
        }

        private int GetCaptureCacheLength()
        {
            int l = Captures.Length;
            for (int i = 0; i < ContextCaptures.Length; i++)
            {
                l += ContextCaptures[i].Property.Length;
            }
            return l;
        }
    }
}
