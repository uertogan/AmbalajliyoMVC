using AmbalajliyoMVC.Service;
using AmbalajliyoMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

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

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<HttpClient>();

            // HttpClient servisleri
            builder.Services.AddHttpClient<RoleAPIService>();
            builder.Services.AddHttpClient<UserAPIService>();
            builder.Services.AddHttpClient<CustomerAPIService>();
            builder.Services.AddHttpClient<PostApiService>();
            builder.Services.AddHttpClient<CategoryAPIService>();
            builder.Services.AddHttpClient<ProductAPIService>();

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

            app.UseRouting();

            // Kimlik doðrulama middleware'i ekleniyor
            app.UseAuthentication();  // Bu satýr kimlik doðrulama iþlemlerini aktif eder
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // SEO dostu URL'ler
            app.MapControllerRoute(
                name: "product-details",
                pattern: "urunler/{categoryName}/{productName}-{id}",
                defaults: new { controller = "Product", action = "GetByIdProduct" });
            app.MapControllerRoute(
                name: "category-details",
                pattern: "kategoriler/{categoryName}/{id}",
                defaults: new { controller = "Category", action = "GetByIdCategory" });
            app.MapControllerRoute(
                name: "post-details",
                pattern: "postlar/{postTitle}/{id}",
                defaults: new { controller = "Post", action = "GetByIdPost" });
            app.MapControllerRoute(
                name: "hakkimizda",
                pattern: "ambalajliyo-hakkinda",
                defaults: new { controller = "Home", action = "Hakkimizda" });
            app.MapControllerRoute(
                name: "misyon-vizyon-degerlerimiz",
                pattern: "ambalajliyo-misyon-vizyon-degerleri",
                defaults: new { controller = "Home", action = "MisyonVizyonDegerlerimiz" });

            app.Run();
        }
    }
}
