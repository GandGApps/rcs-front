﻿#pragma checksum "..\..\..\..\Views\ClientView.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "6A9A7AAF6EA4E1BA5ED3E32389088BC759811B55"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Kassa.RxUI;
using Kassa.Wpf.Controls;
using Kassa.Wpf.MarkupExntesions;
using Kassa.Wpf.Views;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace Kassa.Wpf.Views {
    
    
    /// <summary>
    /// ClientView
    /// </summary>
    public partial class ClientView : ReactiveUI.ReactiveUserControl<Kassa.RxUI.ClientViewModel>, System.Windows.Markup.IComponentConnector {
        
        
        #line 82 "..\..\..\..\Views\ClientView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Shapes.Path IsSelectedIcon;
        
        #line default
        #line hidden
        
        
        #line 92 "..\..\..\..\Views\ClientView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock FullName;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\..\Views\ClientView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Details;
        
        #line default
        #line hidden
        
        
        #line 111 "..\..\..\..\Views\ClientView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run Phone;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\..\..\Views\ClientView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run Address;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.7.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Kassa.Wpf;component/views/clientview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Views\ClientView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.7.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.IsSelectedIcon = ((System.Windows.Shapes.Path)(target));
            return;
            case 2:
            this.FullName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.Details = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.Phone = ((System.Windows.Documents.Run)(target));
            return;
            case 5:
            this.Address = ((System.Windows.Documents.Run)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
