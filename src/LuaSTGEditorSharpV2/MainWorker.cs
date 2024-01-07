﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using LuaSTGEditorSharpV2.Core;
using LuaSTGEditorSharpV2.Core.CodeGenerator;
using LuaSTGEditorSharpV2.PropertyView;
using LuaSTGEditorSharpV2.ViewModel;
using LuaSTGEditorSharpV2.Core.Services;

namespace LuaSTGEditorSharpV2
{
    public class MainWorker : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                HostedApplicationHelper.InitNodeService();
                var nodePackageProvider = HostedApplicationHelper.GetService<NodePackageProvider>();
                var resc = nodePackageProvider.LoadPackage("Core");
                var lua = nodePackageProvider.LoadPackage("Lua");
                var resln = nodePackageProvider.LoadPackage("LegacyNode");
                ResourceManager.MergeResources();

                HostedApplicationHelper.GetService<LocalizationService>().OnCultureChanged += (o, e) =>
                    WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = e.CultureInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
