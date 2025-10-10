using Chat.ModelBinders;
using Chat.Plugins;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using ModelContextProtocol.Client;


namespace Chat;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        //Add Model Binder for SK AuthorRole
        builder.Services.AddControllersWithViews(options =>
        {
            options.ModelBinderProviders.Insert(0, new AuthorRoleBinderProvider());
        }).AddRazorRuntimeCompilation();

        //Add Semantic Kernel
        var kernelBuilder = builder.Services.AddKernel();

        //Add MCP Servers
        await AddFileSystemMcpServer(kernelBuilder);

        kernelBuilder.Plugins.AddFromType<GetDateTime>();
        kernelBuilder.Plugins.AddFromType<GetWeather>();
        kernelBuilder.Plugins.AddFromType<GetGeoCoordinates>();
        kernelBuilder.Plugins.AddFromType<PersonalInfo>();

        var kernel = builder.Services.BuildServiceProvider().GetRequiredService<Kernel>();

        var kernelPlugin = await kernel.ImportPluginFromOpenApiAsync(
         pluginName: "customers",
         uri: new Uri("https://localhost:7049/swagger/v1/swagger.json")
         );
        builder.Services.AddSingleton(kernelPlugin);

        //Add Azure OpenAI Service

        builder.Services.AddAzureOpenAIChatCompletion(
            deploymentName: builder.Configuration.GetValue<string>("AZURE_OPENAI_CHAT_DEPLOYMENT")!,
            endpoint: builder.Configuration.GetValue<string>("AZURE_OPENAI_ENDPOINT")!,
            apiKey: builder.Configuration.GetValue<string>("AZURE_OPENAI_KEY")!);


        // Enable concurrent invocation of functions to get the latest news and the current time.
        FunctionChoiceBehaviorOptions options = new() { AllowConcurrentInvocation = true };


        builder.Services.AddTransient<PromptExecutionSettings>(_ => new OpenAIPromptExecutionSettings
        {
            Temperature = 0.75,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: options)
        });

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}");

        app.Run();
    }

    private static async Task AddFileSystemMcpServer(IKernelBuilder kernelBuilder)
    {
        //connect to MCP FileSystem server
        IMcpClient mcpClient = await McpClientFactory.CreateAsync(new StdioClientTransport(new()
        {
            Name = "FileSystem",
            Command = "npx",
            Arguments = ["-y", "@modelcontextprotocol/server-filesystem", "C:\\Users\\sunka\\source\\repos\\explore-semantic-kernel\\src\\SK-DevChat\\Chat\\data\\"]
        }));

        // Get the list of tools from the MCP server and add them to the kernel
        IList<McpClientTool> tools = await mcpClient.ListToolsAsync();
        kernelBuilder.Plugins.AddFromFunctions("FS", tools.Select(skFunction => skFunction.AsKernelFunction()));
    }
}
