﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LuaSTGEditorSharpV2.Core;
using LuaSTGEditorSharpV2.Core.Command;
using LuaSTGEditorSharpV2.Core.Model;
using LuaSTGEditorSharpV2.Core.ViewModel;

namespace LuaSTGEditorSharpV2.PropertyView
{
    // TODO: re-design "subtype". maybe it should be supported individually for each property.
    /// <summary>
    /// Provide functionality of presenting and manipulating <see cref="NodeData"/> properties.
    /// </summary>
    [ServiceName("PropertyView"), ServiceShortName("prop")]
    public class PropertyViewServiceBase 
        : NodeService<PropertyViewServiceBase, PropertyViewContext, PropertyViewServiceSettings>
    {
        private static readonly PropertyViewServiceBase _defaultService = new();

        static PropertyViewServiceBase()
        {
            _defaultServiceGetter = () => _defaultService;
        }

        /// <summary>
        /// Obtain a list of <see cref="PropertyViewModel"/> according to data source for providing properties to edit. 
        /// </summary>
        /// <param name="nodeData"> The data source. </param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public static IReadOnlyList<PropertyViewModel> GetPropertyViewModelOfNode(NodeData nodeData, int subtype = 0)
        {
            return GetServiceOfNode(nodeData).ResolvePropertyViewModelOfNode(nodeData, subtype);
        }

        /// <summary>
        /// Obtain a command which manipulate target <see cref="NodeData"/> by infomation from UI.
        /// </summary>
        /// <param name="nodeData"> The data source. </param>
        /// <param name="propertyList"> 
        /// The <see cref="PropertyViewModel"/> generated by <see cref="GetPropertyViewModelOfNode"/>. 
        /// </param>
        /// <param name="index"> Index of item in <see cref="PropertyViewModel"/>s. </param>
        /// <param name="edited"> The <see cref="string"/> as edit result. </param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public static CommandBase GetCommandOfEditingNode(NodeData nodeData
            , IReadOnlyList<PropertyViewModel> propertyList, int index, string edited, int subtype = 0)
        {
            return GetServiceOfNode(nodeData).ResolveCommandOfEditingNode(nodeData, propertyList
                , index, edited, subtype);
        }

        public override sealed PropertyViewContext GetEmptyContext(LocalParams localSettings)
        {
            return new PropertyViewContext(localSettings, ServiceSettings);
        }

        /// <summary>
        /// Obtain a list of <see cref="PropertyViewModel"/> according to data source with 
        /// same TypeUID for providing properties to edit.
        /// </summary>
        /// <param name="nodeData"> The data source with the same TypeUID. </param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        protected virtual IReadOnlyList<PropertyViewModel> ResolvePropertyViewModelOfNode(NodeData nodeData
            , int subtype = 0)
        {
            List<PropertyViewModel> result = new(nodeData.Properties.Count);
            foreach (var prop in nodeData.Properties)
            {
                result.Add(new PropertyViewModel(prop.Key, prop.Value));
            }
            return result;
        }

        /// <summary>
        /// Obtain a command which manipulate target <see cref="NodeData"/> with the same TypeUID by infomation from UI.
        /// </summary>
        /// <param name="nodeData"> The data source with the same TypeUID. </param>
        /// <param name="propertyList"> 
        /// The <see cref="PropertyViewModel"/> generated by <see cref="GetPropertyViewModelOfNode"/>. 
        /// </param>
        /// <param name="index"> Index of item in <see cref="PropertyViewModel"/>s. </param>
        /// <param name="edited"> The <see cref="string"/> as edit result. </param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        protected virtual CommandBase ResolveCommandOfEditingNode(NodeData nodeData
            , IReadOnlyList<PropertyViewModel> propertyList, int index, string edited, int subtype = 0)
        {
            return EditPropertyCommand.CreateEditCommandOnDemand(nodeData, propertyList[index].Name, edited);
        }
    }
}
