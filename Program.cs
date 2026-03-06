<<<<<<< HEAD
using System.Text;
using FashionEcommerce.Data;
using FashionEcommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ DỊCH VỤ (SERVICES) ---
=======
using FashionEcommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
>>>>>>> origin/haihoang
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

<<<<<<< HEAD
// Cấu hình CORS - PHẢI ĐẶT TRƯỚC builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://127.0.0.1:5500",
                    "http://localhost:5500"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    );
});

// Cấu hình JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "Key_Bi_Mat_Cuc_Ky_Dai_Tren_32_Ky_Tu_123456";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
=======
// Minimal JWT authentication skeleton (replace with real settings)
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChangeThisToASecureKey";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
>>>>>>> origin/haihoang
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
<<<<<<< HEAD
            IssuerSigningKey = new SymmetricSecurityKey(key), // Sử dụng biến key đã tạo
        };
    });

// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// --- 3. ĐĂNG KÝ DỊCH VỤ TÙYCHỈNH ---

// Đăng ký Promotion Engine Service
builder.Services.AddScoped<IPromotionService, PromotionService>();

var app = builder.Build();

// --- 2. CẤU HÌNH PIPELINE (MIDDLEWARE) ---

// Swagger luôn bật để hỗ trợ Dev
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// THỨ TỰ QUAN TRỌNG: CORS phải nằm trước Authentication
app.UseCors("AllowAll");

app.UseAuthentication(); // Xác thực danh tính
app.UseAuthorization(); // Kiểm tra quyền hạn

app.MapControllers();

app.Run();
=======
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configure DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
>>>>>>> origin/haihoang
