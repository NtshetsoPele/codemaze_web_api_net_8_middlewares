var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Use(middleware: async (HttpContext context, Func<Task> next) =>
{
    Console.WriteLine("Logic before executing the next delegate in the Use method"); 
    await next.Invoke(); 
    Console.WriteLine("Logic after executing the next delegate in the Use method");
});

app.Map(pathMatch: "/using-map-branch", configuration: (IApplicationBuilder builder) =>
{
    builder.Use(async (HttpContext context, Func<Task> next) =>
    {
        Console.WriteLine("Map branch logic in the Use method before the next delegate"); 
        await next.Invoke(); 
        Console.WriteLine("Map branch logic in the Use method after the next delegate");
    }); 
    
    builder.Run(async (HttpContext context) =>
    {
        Console.WriteLine("Map branch response to the client in the Run method"); 
        await context.Response.WriteAsync("Hello from the map branch.");
    });
});

app.MapWhen(
    (HttpContext context) => context.Request.Query.ContainsKey("test-query-string"), 
    (IApplicationBuilder builder) => 
    {
        builder.Run(async (HttpContext context) => 
        {
            await context.Response.WriteAsync("Hello from the MapWhen branch.");
        });
});

app.Run(handler: async (HttpContext context) =>
{
    Console.WriteLine("Writing the response to the client in the Run method");
    await context.Response.WriteAsync("Hello from the middleware component.");
});

app.Run();