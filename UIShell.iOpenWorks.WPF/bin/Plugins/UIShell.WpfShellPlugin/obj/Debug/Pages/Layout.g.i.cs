﻿#pragma checksum "..\..\..\Pages\Layout.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EE05A44D41FEF83483622EDF5884FB4E"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Converters;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using UIShell.WpfShellPlugin.Pages;


namespace UIShell.WpfShellPlugin.Pages {
    
    
    /// <summary>
    /// Layout
    /// </summary>
    public partial class Layout : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TimeNowTextBlock;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock CurrentUserTextBlock;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MessageTextBlock;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ColumnDefinition TreeViewColumn;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView NavigationTreeView;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock LoadingTextBlock;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutDockPanel;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel SideBarDockPanel;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock SideBarTitleTextBlock;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\Pages\Layout.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DockPanel SideBarDockPanelContent;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/UIShell.WpfShellPlugin;component/pages/layout.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\Layout.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TimeNowTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.CurrentUserTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.MessageTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.TreeViewColumn = ((System.Windows.Controls.ColumnDefinition)(target));
            return;
            case 5:
            this.NavigationTreeView = ((System.Windows.Controls.TreeView)(target));
            
            #line 29 "..\..\..\Pages\Layout.xaml"
            this.NavigationTreeView.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.NavigationTreeView_SelectedItemChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 30 "..\..\..\Pages\Layout.xaml"
            ((System.Windows.Controls.GridSplitter)(target)).DragCompleted += new System.Windows.Controls.Primitives.DragCompletedEventHandler(this.GridSplitter_DragCompleted);
            
            #line default
            #line hidden
            return;
            case 7:
            this.LoadingTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.LayoutDockPanel = ((System.Windows.Controls.Grid)(target));
            return;
            case 9:
            this.SideBarDockPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 10:
            this.SideBarTitleTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 11:
            this.SideBarDockPanelContent = ((System.Windows.Controls.DockPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
