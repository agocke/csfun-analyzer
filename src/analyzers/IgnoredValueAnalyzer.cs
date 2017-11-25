
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CsFunAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IgnoredValueAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "FUN001";
        internal const string Title = "Return value implicitly ignored";
        internal const string MessageFormat = "Value of expression is being ignored. Assign result with '_ = ' if you mean to ignore the value";
        internal const string Description = "Expression values should be explicitly ignored";
        internal const string Category = "Semantics";

        public static readonly DiagnosticDescriptor IgnoredValueDiagnosticRule = new DiagnosticDescriptor(
                DiagnosticId,
                Title,
                MessageFormat,
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(IgnoredValueDiagnosticRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
        }

        private static void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var expr = ((ExpressionStatementSyntax)context.Node).Expression;
            if (expr is AssignmentExpressionSyntax)
            {
                // Ignore assignment expressions
                return;
            }

            var typeInfo = context.SemanticModel.GetTypeInfo(expr);
            if (typeInfo.Type.SpecialType != SpecialType.System_Void)
            {
                var diag = Diagnostic.Create(IgnoredValueDiagnosticRule, expr.GetLocation());
                context.ReportDiagnostic(diag);
            }
        }
    }
}