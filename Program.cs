using MinimalApi.Dominio.DTOs;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello!");

app.MapPost("/login", (LoginDTO loginDTO) => { //Faz a validação com o MapPost
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    {
        return Results.Ok("Login realizado com sucesso");
    }
    else
    {
        return Results.Unauthorized(); //nao autoriazdo
    }
});

app.Run();
