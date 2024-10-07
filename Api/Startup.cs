using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration.GetSection("Jwt").ToString() ?? "";
        }
        
        private string key;
        public IConfiguration Configuration { get; set; } = default!;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(option => {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(option => {
                    option.TokenValidationParameters = new TokenValidationParameters{
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            services.AddAuthorization();
            services.AddScoped<IAdministradorServicos, AdministradorServicos>();
            services.AddScoped<IVeiculosServicos, VeiculosServico>();
            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options => {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token Jwt aqui"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, 
                        new string[] {}
                    }
                });
            });
            
            services.AddDbContext<DbContexto>(options => 
            {
                options.UseMySql(Configuration.GetConnectionString("MySql"), ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql")));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endPoints => {
                #region Home
                endPoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
                #endregion

                #region Administradores
                string GerarTokenJwt(Administrador administrador)
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        return string.Empty;
                    }
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); //Criptografia
                    var claims = new List<Claim>()
                    {
                        new Claim("Email", administrador.Email),
                        new Claim("Perfil", administrador.Perfil),
                        new Claim(ClaimTypes.Role, administrador.Perfil)
            
                    };

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }

                endPoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) => { //Faz a validação com o MapPost
                    var adm = administradorServicos.Login(loginDTO);
                    if (adm != null)
                    {
                        string token = GerarTokenJwt(adm);
                        return Results.Ok(new AdmLogado
                        {
                            Email = adm.Email,
                            Perfil = adm.Perfil,
                            Token = token

                        });
                    }
                    else
                    {
                        return Results.Unauthorized(); //nao autoriazdo
                    }
                }).AllowAnonymous().WithTags("Administrador");

                endPoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServicos administradorServicos) => { //Faz a validação com o MapPost
                    var validacao = new ErrosDeValidacao{
                        Mensgem = new List<string>()
                    };
                    if (string.IsNullOrEmpty(administradorDTO.Email))
                    {
                        validacao.Mensgem.Add("Email não pode ser vazio");
                    }
                    if (string.IsNullOrEmpty(administradorDTO.Senha))
                    {
                        validacao.Mensgem.Add("Senha não pode ser vazio");
                    }

                    if (validacao.Mensgem.Count() > 0)
                    {
                        return Results.BadRequest(validacao);
                    }
    
                    var adm = new Administrador{
                        Email = administradorDTO.Email,
                        Senha = administradorDTO.Senha,
                        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                    };
                    administradorServicos.Incluir(adm);
                    return Results.Created($"/administrador/{adm.Id}", adm);

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administrador");

                endPoints.MapGet("/administradores", ([FromQuery] int pagina, IAdministradorServicos administradorServicos) => { //Faz a validação com o MapPost
                    var adms = new List<AdministradorModelViews>();
                    var administradores = administradorServicos.Todos(pagina);
                    foreach (var adm in administradores)
                    {
                        adms.Add(new AdministradorModelViews{
                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil
            
                        });
                    }

                    return Results.Ok(adms);
    
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administrador");

                endPoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServicos administradorServicos) => {
                    var adm = administradorServicos.BuscarPorId(id);

                    if (adm == null)
                    {
                        return Results.NotFound();
                    }
    
                    return Results.Ok(new AdministradorModelViews{
                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil
            
                        });

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Administrador");
                #endregion

                #region Veiculos
                ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
                {
                    var validacao = new ErrosDeValidacao{
                        Mensgem = new List<string>()
                    };

                    if (string.IsNullOrEmpty(veiculoDTO.Nome))
                    {
                        validacao.Mensgem.Add("O nome não pode ser vazio ou nullo!");
                    }

                    if (string.IsNullOrEmpty(veiculoDTO.Marca))
                    {
                        validacao.Mensgem.Add("O marca não pode ser vazio ou nullo!");
                    }

                    if (veiculoDTO.Ano < 1886)
                    {
                        validacao.Mensgem.Add("Veiculo muito antigo tente um depois de 1886!");
                    }

                    return validacao;
                }

                endPoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServicos veiculosServicos) => { //Faz a validação com o MapPost

    
                    var validacao = ValidaDTO(veiculoDTO);
                    if (validacao.Mensgem.Count() > 0)
                    {
                        return Results.BadRequest(validacao);
                    }

                    var veiculo = new Veiculos{
                        Nome = veiculoDTO.Nome,
                        Marca = veiculoDTO.Marca,
                        Ano = veiculoDTO.Ano
                    };
                    veiculosServicos.Incluir(veiculo);
                    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm,Editor"}).WithTags("Veiculo");

                endPoints.MapGet("/veiculos", ([FromQuery] int pagina, IVeiculosServicos veiculosServicos) => {
                    var veiculos = veiculosServicos.Todos(pagina);
    
                    return Results.Ok(veiculos);

                }).RequireAuthorization().WithTags("Veiculo");

                endPoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculosServicos veiculosServicos) => {
                    var veiculo = veiculosServicos.BuscaPorId(id);

                    if (veiculo == null)
                    {
                        return Results.NotFound();
                    }
    
                    return Results.Ok(veiculo);

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm,Editor"}).WithTags("Veiculo");

                endPoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculosServicos veiculosServicos) => {
                    var veiculo = veiculosServicos.BuscaPorId(id);
    
                    if (veiculo == null)
                    {
                        return Results.NotFound();
                    }

                    var validacao = ValidaDTO(veiculoDTO);
                    if (validacao.Mensgem.Count() > 0)
                    {
                        return Results.BadRequest(validacao);
                    }

                    veiculo.Nome = veiculoDTO.Nome;
                    veiculo.Marca = veiculoDTO.Marca;
                    veiculo.Ano = veiculoDTO.Ano;
                    veiculosServicos.Atualizar(veiculo);
                    return Results.Ok(veiculo);

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Veiculo");

                endPoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculosServicos veiculosServicos) => {
                    var veiculo = veiculosServicos.BuscaPorId(id);

                    if (veiculo == null)
                    {
                        return Results.NotFound();
                    }
    
                    veiculosServicos.Apagar(veiculo);
                    return Results.NoContent();

                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute {Roles = "Adm"}).WithTags("Veiculo");
                #endregion

            });
        }
    }
}