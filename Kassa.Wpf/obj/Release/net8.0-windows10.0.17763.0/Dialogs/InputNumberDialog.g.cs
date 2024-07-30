﻿#pragma checksum "..\..\..\..\Dialogs\InputNumberDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "8DA2D2EAD298DB16679FEC9996084109263337C8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Kassa.RxUI.Dialogs;
using Kassa.Wpf.Controls;
using Kassa.Wpf.Dialogs;
using Kassa.Wpf.MarkupExntesions;
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


namespace Kassa.Wpf.Dialogs {
    
    
    /// <summary>
    /// InputNumberDialog
    /// </summary>
    public partial class InputNumberDialog : Kassa.Wpf.Dialogs.ClosableDialog<Kassa.RxUI.Dialogs.InputNumberDialogViewModel>, System.Windows.Markup.IComponentConnector {
        
        
        #line 33 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock FieldName;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Kassa.Wpf.Controls.TextBoxWithoutVirtualKeyboard Input;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Placeholder;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Kassa.Wpf.Controls.Keyboard Numpad;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelButton;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button OkButton;
        
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
            System.Uri resourceLocater = new System.Uri("/Kassa.Wpf;component/dialogs/inputnumberdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Dialogs\InputNumberDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.7.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.FieldName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.Input = ((Kassa.Wpf.Controls.TextBoxWithoutVirtualKeyboard)(target));
            return;
            case 3:
            this.Placeholder = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.Numpad = ((Kassa.Wpf.Controls.Keyboard)(target));
            return;
            case 5:
            this.CancelButton = ((System.Windows.Controls.Button)(target));
            return;
            case 6:
            this.OkButton = ((System.Windows.Controls.Button)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
