using JasperFx.CodeGeneration;
using JasperFx.CodeGeneration.Frames;
using JasperFx.CodeGeneration.Model;

namespace Wolverine.Http.Resources;

internal class StringResourceWriterPolicy : IResourceWriterPolicy
{
    public bool TryApply(HttpChain chain)
    {
        if (chain.ResourceType == typeof(string))
        {
            chain.Postprocessors.Add(new WriteStringFrame(chain.Method.ReturnVariable));

            return true;
        }

        return false;
    }

    internal class WriteStringFrame : AsyncFrame
    {
        private readonly Variable _result;

        public WriteStringFrame(Variable result)
        {
            _result = result;
            uses.Add(_result);
        }

        public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
        {
            var prefix = method.AsyncMode == AsyncMode.ReturnCompletedTask ? "return" : "await";  
            
            writer.Write($"{prefix} {nameof(HttpHandler.WriteString)}(httpContext, {_result.Usage});");
            
            Next?.GenerateCode(method, writer);
        }
    }
}