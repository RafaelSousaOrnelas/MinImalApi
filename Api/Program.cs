using MinimalApi;
IHostBuilder CreatHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();

    });
}

CreatHostBuilder(args).Build().Run();