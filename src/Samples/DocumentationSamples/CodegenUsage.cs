using JasperFx.CodeGeneration;
using Microsoft.Extensions.Hosting;
using Wolverine;

namespace DocumentationSamples;

public class CodegenUsage
{
    public async Task override_codegen()
    {
        #region sample_codegen_type_load_mode

        using var host = await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                // The default behavior. Dynamically generate the 
                // types on the first usage 
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Dynamic;

                // Never generate types at runtime, but instead try to locate
                // the generated types from the main application assembly
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Static;
                
                // Hybrid approach that first tries to locate the types
                // from the application assembly, but falls back to
                // generating the code and dynamic type. Also writes the 
                // generated source code file to disk
                opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;

            }).StartAsync();

        #endregion
    }

    public async Task use_optimized_workflow()
    {
        #region sample_use_optimized_workflow

        using var host = await Host.CreateDefaultBuilder()
            .UseWolverine(opts =>
            {
                // Use "Auto" type load mode at development time, but
                // "Static" any other time
                opts.OptimizeArtifactWorkflow();
            }).StartAsync();

        #endregion
    }
}