using FashionEcommerce.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. ĐĂNG KÝ DỊCH VỤ (SERVICES) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình CORS - PHẢI ĐẶT TRƯỚC builder.Build()
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Cấu hình JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "Key_Bi_Mat_Cuc_Ky_Dai_Tren_32_Ky_Tu_123456";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key) // Sử dụng biến key đã tạo
    };
});

// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- 2. CẤU HÌNH PIPELINE (MIDDLEWARE) ---

// Swagger luôn bật để hỗ trợ Dev
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// THỨ TỰ QUAN TRỌNG: CORS phải nằm trước Authentication
app.UseCors("AllowAll"); 

app.UseAuthentication(); // Xác thực danh tính
app.UseAuthorization();  // Kiểm tra quyền hạn

app.MapControllers();

app.Run();