﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using NLog;
using NLog.Extensions.Logging;

using LuaSTGEditorSharpV2.Core;
using LuaSTGEditorSharpV2.Core.CodeGenerator;
using LuaSTGEditorSharpV2.PropertyView;
using LuaSTGEditorSharpV2.ViewModel;
using LuaSTGEditorSharpV2.Services;
using LuaSTGEditorSharpV2.Core.Services;
using LuaSTGEditorSharpV2.ServiceBridge.Services;

using static LuaSTGEditorSharpV2.Core.HostedApplicationHelper;

namespace LuaSTGEditorSharpV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var args = e.Args;

            AddNodeServiceProvider<CodeGeneratorServiceProvider>();
            AddNodeServiceProvider<ViewModelProviderServiceProvider>();
            AddNodeServiceProvider<PropertyViewServiceProvider>();
            AddApplicationSingletonService<ActiveDocumentService>();
            AddApplicationSingletonService<LocalizationService>();
            AddApplicationSingletonService<SettingsService>();
            AddApplicationSingletonService<SettingsDisplayService>();
            SetUpHost(() =>
            {
                HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder(args);

                applicationBuilder.Services.AddLogging(builder => builder.AddNLog());
                applicationBuilder.Services.AddHostedService<MainWorker>();
                return applicationBuilder;
            }, args);
        }
    }
}
