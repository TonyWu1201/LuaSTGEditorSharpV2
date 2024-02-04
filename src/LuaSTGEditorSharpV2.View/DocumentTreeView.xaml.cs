﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LuaSTGEditorSharpV2.WPF;

namespace LuaSTGEditorSharpV2.View
{
    /// <summary>
    /// DocumentTreeView.xaml 的交互逻辑
    /// </summary>
    public partial class DocumentTreeView : MultiSelectDragableTreeView
    {
        public DocumentTreeView()
        {
            InitializeComponent();
        }
    }
}
