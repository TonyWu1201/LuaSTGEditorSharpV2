using System;

using LuaSTGEditorSharpV2.Core;
using LuaSTGEditorSharpV2.PropertyView;

namespace LuaSTGEditorSharpV2.Package.Lua.PropertyView
{
    public class PackageEntry : IPackageEntry
    {
        public void InitializePackage()
        {
            PropertyViewServiceBase.AddResourceDictUri(
                "pack://application:,,,/LuaSTGEditorSharpV2.Package.Lua.PropertyView;component/PropertyView.xaml");
        }
    }
}
