using AmbalajliyoMVC.Middlewares;
using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Smidge;
using System.Text;

namespace AmbalajliyoMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // HttpClient için merkezi BaseAddress ayarý
            builder.Services.AddHttpClient<UserAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });

            builder.Services.AddHttpClient<CategoryAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            builder.Services.AddHttpClient<CustomerAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            builder.Services.AddHttpClient<ProductAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            builder.Services.AddHttpClient<RoleAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            builder.Services.AddHttpClient<PostApiService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            builder.Services.AddHttpClient<FaqAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            builder.Services.AddHttpClient<CatalogAPIService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseAddress"]);
            });
            // Session yönetimi
            builder.Services.AddSession();

            // Cookie tabanlý kimlik doðrulama ekleniyor
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";  // Giriþ yapma yolu
                    options.LogoutPath = "/User/Logout"; // Çýkýþ yapma yolu
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Cookie süresi
                });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder => builder.WithOrigins("https://ambalajliyomvc20240918174909.azurewebsites.net")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            builder.Services.AddControllers();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<HttpClient>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // token'ýn ayarlarýný bildirelim. Yani Issuer, Audience, Lifetime, Key gibi özelliklerinin DOÐRULANACAÐINI belirtelim.
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true, // JWT token'ý veren kiþiyi doðrula
                    ValidateAudience = true, // JWT token'ý kullanan kiþiyi doðrula
                    ValidateLifetime = true, // JWT token'ýn süresini doðrula
                    ValidateIssuerSigningKey = true, // JWT token'ýn imza key'ini doðrula

                    ValidIssuer = builder.Configuration["JwtTokenSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtTokenSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokenSettings:Key"]))
                };
            });

            // HttpClient servisleri
            builder.Services.AddHttpClient<RoleAPIService>();
            builder.Services.AddHttpClient<UserAPIService>();
            builder.Services.AddHttpClient<CustomerAPIService>();
            builder.Services.AddHttpClient<PostApiService>();
            builder.Services.AddHttpClient<CategoryAPIService>();
            builder.Services.AddHttpClient<ProductAPIService>();
            builder.Services.AddHttpClient<FaqAPIService>();
            builder.Services.AddHttpClient<CatalogAPIService>();


            // Smidge yapýlandýrmasý
            builder.Services.AddSmidge(builder.Configuration.GetSection("smidge"));
            //builder.Services.AddSmidge(builder.Configuration.GetSection("adminSmidge"));
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("AllowSpecificOrigins");

            app.UseRouting();

            // JWT Middleware'i ekle
            app.UseMiddleware<JwtMiddleware>();

            // Kimlik doðrulama middleware'i ekleniyor
            app.UseAuthentication();  // Bu satýr kimlik doðrulama iþlemlerini aktif eder
            app.UseAuthorization();
           
            // JavaScript ve css dosyalarýný minify etme iþlemi
            app.UseSmidge(bundle =>
            {
                // JavaScript bundle    
                bundle.CreateJs("js-bundle", "~/js/site.js", "~/lib/bootstrap/dist/js/bootstrap.js" ); 
                // CSS bundle
                bundle.CreateCss("css-bundle", "~/css/site.css", "~/lib/bootstrap/dist/css/bootstrap.css", "~/lib/bootstrap/dist/css/bootstrap-grid.css", "~/lib/bootstrap/dist/css/bootstrap-reboot.css", "~/lib/bootstrap/dist/css/bootstrap-utilities.css");
            });

            app.UseSmidge(bundle =>
            {
                // JavaScript bundle    
                bundle.CreateJs("admin-js-bundle", "~/kaiadmin-lite-1.2.0/assets/js/core/coostrap.min.js",
            "~/kaiadmin-lite-1.2.0/assets/js/core/jquery-3.7.1.min.js",
                    "~/kaiadmin-lite-1.2.0/assets/js/core/pooper.js",
                    //"~/kaiadmin-lite-1.2.0/assets/js/plugin/", // umarým içindekileri hep alýr
                    "~/kaiadmin-lite-1.2.0/assets/js/demo.js",
                    "~/kaiadmin-lite-1.2.0/assets/js/kaiadmin.js",
                    "~/kaiadmin-lite-1.2.0/assets/js/kaiadmin.js/kaiadmin.min.js",
                    "~/kaiadmin-lite-1.2.0/assets/js/setting-demo.js",
                    "~/kaiadmin-lite-1.2.0/assets/js/setting-demo2.js"

                );

                // CSS bundle
                bundle.CreateCss("admin-css-bundle", "~/kaiadmin-lite-1.2.0/assets/css/bootstrap.min.css",
            "~/kaiadmin-lite-1.2.0/assets/css/plugins.min.css",
                    "~/kaiadmin-lite-1.2.0/assets/css/kaiadmin.min.css",
                    "~/kaiadmin-lite-1.2.0/assets/css/demo.css",
                    "~/kaiadmin-lite-1.2.0/assets/css/fonts.css"
                );
            });

            // area route tanýmý
            app.MapControllerRoute(
               name: "default",
               pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
            // default route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // SEO dostu URL'ler
            app.MapControllerRoute(
                name: "product-details",
                pattern: "urunler/{categoryName}/{productName}-{id}",
                defaults: new { controller = "Product", action = "GetByIdProduct" });
            app.MapControllerRoute(
                name: "post-details",
                pattern: "postlar/{postTitle}/{id}",
                defaults: new { controller = "Post", action = "GetByIdPost" });
            app.MapControllerRoute(
                name: "Hakkimizda",
                pattern: "ambalajliyo-hakkinda",
                defaults: new { controller = "Home", action = "Hakkimizda" });
            app.MapControllerRoute(
                name: "misyon-vizyon-degerlerimiz",
                pattern: "ambalajliyo-misyon-vizyon-degerleri",
                defaults: new { controller = "Home", action = "MisyonVizyonDegerlerimiz" });
            app.MapControllerRoute(
                name: "category-products",
                pattern: "kategori/{categoryName}-ambalaj-ürünleri-listesi/{id}",
                defaults: new { controller = "Product", action = "FilterProductsByCategory" });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.Run();
        }
    }
}
