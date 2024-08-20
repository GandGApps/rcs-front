using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Kassa.Shared.ServiceLocator.Analyzer;
public sealed record ServiceDescriptor(
    INamedTypeSymbol ServiceType,
    INamedTypeSymbol ImplementationType,
    IMethodSymbol Constructor,
    Diagnostic[] Diagnostics,
    SimpleNameSyntax ServiceTypeSyntax,
    SimpleNameSyntax ImplementationTypeSyntax,
    GenericNameSyntax FactorySyntax)
{

    public bool IsTransient => FactorySyntax.Identifier.Text is RcsLocatorBuilderGeneratorStrings.AddTransientMethodName;

    public bool IsScoped => FactorySyntax.Identifier.Text is RcsLocatorBuilderGeneratorStrings.AddScopedMethodName;

    public bool IsSingleton => FactorySyntax.Identifier.Text is RcsLocatorBuilderGeneratorStrings.AddSingletonMethodName;

    /// <summary>
    /// Need only for generating
    /// </summary>
    public string MethodSuffix
    {
        get; set;
    } = null!;
}