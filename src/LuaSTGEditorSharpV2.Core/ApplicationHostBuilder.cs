﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using LuaSTGEditorSharpV2.Core.Services;

using Microsoft.Extensions.Hosting;

namespace LuaSTGEditorSharpV2.Core
{
    public abstract class ApplicationHostBuilder(string[] args)
    {
        protected string[] _args = args;

        public IHost BuildHost()
        {
            var assemblyDescs = CreatePackageDependentAssemblyDescriptor(CreatePackedServiceDescriptors());

            HostApplicationBuilder applicationBuilder = CreateApplicationBuilder(_args, CreatePackedServiceDescriptors());

            var host = applicationBuilder.Build();

            host.Services.GetRequiredService<SettingsService>().LoadSettings();

            LoadPackageContent(assemblyDescs, host);

            return host;
        }

        private PackedServiceCollection CreatePackedServiceDescriptors()
        {
            var packedServiceInfos = new PackedServiceCollection();
            LoadAssemblies();
            foreach (var service in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t.GetCustomAttribute<PackedServiceProviderAttribute>() != null))
            {
                packedServiceInfos.Add(service);
            }
            ConfigurePackedServiceInfos(packedServiceInfos);
            return packedServiceInfos;
        }

        private IReadOnlyCollection<PackageAssemblyDescriptor> CreatePackageDependentAssemblyDescriptor(PackedServiceCollection packedServiceInfos)
        {
            ApplicationBootstrapLoader applicationBootstrapLoader = new(packedServiceInfos, ConfigureLogging);
            IReadOnlyCollection<PackageAssemblyDescriptor> assemblyDesc = [];
            using (var bootStrap = applicationBootstrapLoader.Load())
            {
                var bootStrapService = bootStrap.GetRequiredService<BootstrapLoaderNodePackageProvider>();

                assemblyDesc = OverridePackages()
                    ?.Select(bootStrapService.LoadAssembly)
                    ?.OfType<PackageAssemblyDescriptor>()
                    ?.ToArray() ?? bootStrapService.LoadAssemblies();
            }

            return assemblyDesc;
        }

        private HostApplicationBuilder CreateApplicationBuilder(string[] args, PackedServiceCollection packedServiceInfos)
        {
            HostApplicationBuilder applicationBuilder = Host.CreateApplicationBuilder(args);

            var services = applicationBuilder.Services;

            applicationBuilder.Services.AddSingleton<IPackedServiceCollection>(_ => packedServiceInfos);
            applicationBuilder.Services.AddSingleton(_ => services);
            applicationBuilder.Services.AddLogging(ConfigureLogging);
            applicationBuilder.Services.AddSingleton<SettingsService>();

            foreach (var info in packedServiceInfos)
            {
                applicationBuilder.Services.AddSingleton(info.ServiceProviderType);
            }

            foreach (var desc in GetInjectedServiceDescriptors())
            {
                applicationBuilder.Services.Add(desc);
            }

            applicationBuilder.Services.AddSingleton<NodePackageProvider>();

            ConfigureService(applicationBuilder.Services);
            return applicationBuilder;
        }

        private static void LoadPackageContent(IReadOnlyCollection<PackageAssemblyDescriptor> assemblyDescs, IHost host)
        {
            var nodePackageProvider = host.Services.GetRequiredService<NodePackageProvider>();
            foreach (var assemblyDesc in assemblyDescs)
            {
                nodePackageProvider.LoadPackage(assemblyDesc);
            }
        }

        protected abstract void ConfigureLogging(ILoggingBuilder loggingBuilder);
        protected virtual void ConfigurePackedServiceInfos(PackedServiceCollection collection) { }
        protected virtual void ConfigureService(IServiceCollection services) { }
        protected virtual IReadOnlyCollection<string>? OverridePackages() => null;

        private static IEnumerable<ServiceDescriptor> GetInjectedServiceDescriptors()
        {
            LoadAssemblies();
            var arr = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract)
                .Select(t => t.GetCustomAttribute<InjectAttribute>()?.ToDescriptor(t))
                .OfType<ServiceDescriptor>().ToArray();
            return arr;
        }

        private static void LoadAssemblies()
        {
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.GetName().Name?.StartsWith("LuaSTGEditorSharpV2") ?? false)
                {
                    LoadEditorDependencyRecursively(a);
                }
            }
        }

        private static void LoadEditorDependencyRecursively(Assembly assembly)
        {
            var resolver = new AssemblyDependencyResolver(assembly.Location);
            foreach (var an in assembly.GetReferencedAssemblies())
            {
                if (an.Name?.StartsWith("LuaSTGEditorSharpV2") ?? false)
                {
                    Assembly loaded;
                    var path = resolver.ResolveAssemblyToPath(an);
                    if (path != null)
                    {
                        loaded = Assembly.LoadFrom(path);
                    }
                    else
                    {
                        loaded = Assembly.Load(an);
                    }
                    LoadEditorDependencyRecursively(loaded);
                }
            }
        }
    }
}