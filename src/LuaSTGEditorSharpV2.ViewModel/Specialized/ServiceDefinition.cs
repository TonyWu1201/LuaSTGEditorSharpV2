﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using LuaSTGEditorSharpV2.Core.Model;
using System.Globalization;
using LuaSTGEditorSharpV2.Core;

namespace LuaSTGEditorSharpV2.ViewModel.Specialized
{
    [Serializable]
    internal class ServiceDefinition : ViewModelProviderServiceBase
    {
        [JsonProperty] public string Icon { get; private set; } = "";
        [JsonProperty] public string DeclarationCaputure { get; private set; } = "";
        [JsonProperty] public string ShortNameCaputure { get; private set; } = "";
        [JsonProperty] public string Text { get; private set; } = "";
        [JsonProperty] public Dictionary<string, string> LocalizedText { get; private set; } = new();
        [JsonProperty] public string ErrorText { get; private set; } = "";
        [JsonProperty] public Dictionary<string, string> LocalizedErrorText { get; private set; } = new();

        protected override void UpdateViewModelData(NodeViewModel viewModel, NodeData dataSource, NodeViewModelContext context)
        {
            var shortName = dataSource.GetProperty(ShortNameCaputure);
            var jsonDecl = dataSource.GetProperty(DeclarationCaputure);
            try
            {
                var nodePackageProvider = HostedApplication.GetService<NodePackageProvider>();
                var type = nodePackageProvider.GetServiceTypeOfShortName(shortName);
                var obj = JsonConvert.DeserializeObject(jsonDecl, type);
                string? uid = type.BaseType?.GetProperty("TypeUID")?.GetValue(obj) as string;
                viewModel.Text = string.Format(LocalizedText
                    .GetValueOrDefault(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, Text)
                    , shortName, uid ?? throw new NullReferenceException());
            }
            catch
            {
                viewModel.Text = string.Format(LocalizedErrorText
                    .GetValueOrDefault(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ErrorText)
                    , shortName);
            }
            viewModel.Icon = Icon;
        }
    }
}
