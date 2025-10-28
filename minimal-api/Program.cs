using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

#region "Builder"
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key))
{
    throw new InvalidOperationException("Chave JWT não encontrada nas configurações.");
}

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer("Bearer", options =>
{
    options.Authority = "https://localhost:7282/";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
        ValidateAudience = false,
        ValidateIssuer = false,
    };
});

builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Formato Token JWT: {Seu Token}"

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração do DbContext com SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseSqlServer(connectionString));

// Injeção de dependência do serviço de administrador
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IAdministradorServico, AdministradorServico >();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

// Construção da aplicação
var app = builder.Build();

#endregion



#region "Home"
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous();
#endregion

#region "Administradores"
app.MapPost("Administradores/Login", ([FromBody ]LoginDTO loginDTO, IAdministradorServico administradorServico) => {

    var admin = administradorServico.Login(loginDTO);

    if (admin != null)
    {
        string token = GerarTokenJwt(admin);
        return Results.Ok( new AdministradorLogado
        {

            Email = admin.Email,
            Perfil = admin.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();

    };
  
}).AllowAnonymous().WithTags("Administradores");

app.MapPost("/AdicionarAdministrador", ([FromBody] AdminstradorDTO adminstradorDTO, IAdministradorServico administradorServico) => {
   
    var validacao = new ErroDeValidacao { Mensagem = new List<string>() };


    if (string.IsNullOrWhiteSpace(adminstradorDTO.Email))
    {
        validacao.Mensagem.Add("O campo 'Email' é obrigatório.");
    }   
    if (string.IsNullOrWhiteSpace(adminstradorDTO.Senha))
    {
        validacao.Mensagem.Add("O campo 'Senha' é obrigatório.");
    }   
    if (string.IsNullOrWhiteSpace(adminstradorDTO.Perfil.ToString()))
    {
        validacao.Mensagem.Add("O campo 'Perfil' é obrigatório.");
    }   
    if (validacao.Mensagem.Count > 0)
    {
        return Results.BadRequest(validacao);
    }   


    var administrador = new Administrador()
    {
        Email = adminstradorDTO.Email,
        Senha = adminstradorDTO.Senha,
        Perfil = adminstradorDTO.Perfil.ToString(),
    };
    administradorServico.AdicionarAdministrador(administrador);
    return Results.Created($"/Administrador/{administrador.Id}", administrador);

})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM" })
    .WithTags("Administradores");

app.MapGet("/ListarAdministradores", (int? pagina, IAdministradorServico administradorServico) =>
{

   var listaAdministradores = new List<AdministradorModelView>();
    var administradores = administradorServico.ObterTodosAdministradores(pagina);

    foreach (var admin in administradores)
    {
        listaAdministradores.Add(new AdministradorModelView
        {
            Id = admin.Id,
            Email = admin.Email,
            Perfil = admin.Perfil
        });
    }
    return Results.Ok(listaAdministradores);


})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM" })
    .WithTags("Administradores");

app.MapGet("/BuscarAdministrador/{id}", (int id, IAdministradorServico administradorServico) =>
{
    var listaAdministradores = new List<AdministradorModelView>();
    var administrador = administradorServico.BuscarAdministradorPorId(id);
    if (administrador == null)
    {
        return Results.NotFound();
    }

    listaAdministradores.Add(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });

    return Results.Ok(administrador);
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM" })
    .WithTags("Administradores");

string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) {

        return string.Empty;
    }
  
   
    var secucurity = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
    var credesntials = new SigningCredentials(secucurity, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)    
    };



    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credesntials

        );

    return new JwtSecurityTokenHandler().WriteToken(token);

}

#endregion

#region "Veículos"

ErroDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErroDeValidacao { Mensagem = new List<string>() };

    if (string.IsNullOrWhiteSpace(veiculoDTO.Nome))
    {
        validacao.Mensagem.Add("O campo 'Nome' é obrigatório.");
    }
    if (string.IsNullOrWhiteSpace(veiculoDTO.Marca))
    {
        validacao.Mensagem.Add("O campo 'Marca' é obrigatório.");
    }
    if (veiculoDTO.Ano == 0 || veiculoDTO.Ano == null)
    {
        validacao.Mensagem.Add("O campo 'Ano' é obrigatório.");
    }
    return validacao;
}

app.MapPost("/AdicionarVeiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) => {
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagem.Count > 0)
    {
        return Results.BadRequest(validacao);
    }


    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
    };
    veiculoServico.AdicionarVeiculo(veiculo);
    return Results.Created($"/Veiculos/{veiculo.Id}", veiculo);

})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM, EDITOR" })
    .WithTags("Veiculos");

app.MapGet("/ListarVeiculos", (int? pagina, IVeiculoServico veiculoServico) =>
{
    var listaVeiculos = veiculoServico.ObterTodosVeiculos(pagina);
    return Results.Ok(listaVeiculos);

})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM, EDITOR" })
    .WithTags("Veiculos");

app.MapGet("/BuscarVeiculo/{id}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarVeiculoPorId(id);
    if (veiculo == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(veiculo);
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM, EDITOR" })
    .WithTags("Veiculos");

app.MapPut("/AtualizarVeiculo/{id}", (int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculoExistente = veiculoServico.BuscarVeiculoPorId(id);
    if (veiculoExistente == null)
    {
        return Results.NotFound();
    }

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagem.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    veiculoExistente.Nome = veiculoDTO.Nome;
    veiculoExistente.Marca = veiculoDTO.Marca;
    veiculoExistente.Ano = veiculoDTO.Ano;
    veiculoServico.AtualizarVeiculo(veiculoExistente);
    return Results.Ok(veiculoExistente);
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM" })
    .WithTags("Veiculos");

app.MapDelete("/RemoverVeiculo/{id}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculoExistente = veiculoServico.BuscarVeiculoPorId(id);
    if (veiculoExistente == null)
    {
        return Results.NotFound();
    }
    veiculoServico.RemoverVeiculo(veiculoExistente);
    return Results.Ok($"Veículo com ID {id} removido com sucesso.");
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "ADM" })
    .WithTags("Veiculos");

#endregion

#region "App"
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// habilitar autenticação e autorização token JWT
app.UseAuthentication();
app.UseAuthorization(); 

//app.UseHttpsRedirection();
app.Run();
#endregion


