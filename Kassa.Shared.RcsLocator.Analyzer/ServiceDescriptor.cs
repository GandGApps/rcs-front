using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kassa.Shared.RcsLocator.Analyzer;
public sealed record ServiceDescriptor(
    INamedTypeSymbol ServiceType,
    INamedTypeSymbol ImplementationType,
    IMethodSymbol Constructor,
    Diagnostic[] Diagnostics,
    IdentifierNameSyntax ServiceTypeSyntax,
    IdentifierNameSyntax ImplementationTypeSyntax,
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
    }
}
