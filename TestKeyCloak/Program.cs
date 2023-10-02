using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using System.Security.Claims;
using TestKeyCloak;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var host = builder.Host;

//host.ConfigureLogger();
services
    .AddEndpointsApiExplorer()
    .AddSwagger();

services.AddKeycloakAuthentication(configuration);

var authorizationOptions = configuration
    .GetSection(KeycloakProtectionClientOptions.Section)
    .Get<KeycloakProtectionClientOptions>();

services
    .AddAuthorization(o => o.AddPolicy("IsAdmin", b =>
    {
        b.RequireRealmRoles("admin");
        b.RequireResourceRoles("r-admin");
        // TokenValidationParameters.RoleClaimType is overriden
        // by KeycloakRolesClaimsTransformation
        b.RequireRole("r-admin");
    }))
    .AddKeycloakAuthorization(authorizationOptions);

var app = builder.Build();

app.UseSwagger().UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", (ClaimsPrincipal user) =>
{
    app.Logger.LogInformation(user.Identity.Name);
}).RequireAuthorization();

// Add services to the container.
//builder.Services.AddRazorPages();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.MapRazorPages();

app.Run();
