namespace Microsoft.Extensions.DependencyInjection;

internal class RavenSmsUIConfigureOptions : IPostConfigureOptions<StaticFileOptions>
{
    private readonly IWebHostEnvironment _environment;

    public RavenSmsUIConfigureOptions(IWebHostEnvironment environment) 
        => _environment = environment;

    public void PostConfigure(string name, StaticFileOptions options)
    {
        // Basic initialization in case the options weren't initialized by any other component
        options.ContentTypeProvider ??= new FileExtensionContentTypeProvider();

        // make sure that at least we have an instance of the file provider
        if (options.FileProvider == null && _environment.WebRootFileProvider == null)
            throw new InvalidOperationException("Missing FileProvider.");

        // init the file provider
        options.FileProvider ??= _environment.WebRootFileProvider;

        // add the file provider to load the package assets
        options.FileProvider = new CompositeFileProvider(
            options.FileProvider, 
            new ManifestEmbeddedFileProvider(GetType().Assembly, "Assets"));
    }
}