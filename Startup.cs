using Amazon.DynamoDBv2;
using TaparSolution.Models;
using TaparSolution.Operations;
using TaparSolution.Operations.QueueHandler;
using Microsoft.Extensions.Options;
using TaparSolution.Helpers;

namespace TaparSolution;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
       services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));

        ActivatorUtilities.CreateInstance<DynamoDbClient>(services.BuildServiceProvider());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }



        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints( endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapPost("/queryrun", async context =>
            {
                // await TelegramMessageComposerHelper.SendInfoToAdmin("gelir");
                OperationResult result = new OperationResult();
                RequestSenderQueueHandler op = new();
                result = op.ExecuteAsync(new()).Result;

            });

            //endpoints.MapGet("/*", async context =>
            //{
            //    ComposeMessage responsemessage = new ComposeMessage()
            //    {
            //        text = "getwithsimplestar",
            //        chat_id = "1762884854"

            //    };
            //    await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage");
            //});

            //endpoints.MapGet("/", async context =>
            //{
            //    ComposeMessage responsemessage = new ComposeMessage()
            //    {
            //        text = "getwithoutany",
            //        chat_id = "1762884854"

            //    };
            //    await WebClient.SendMessagePostAsync<SendMessageResponse>(responsemessage, "sendMessage");
            //});
        });
    }
}