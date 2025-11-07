namespace AMI_Frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -------------------------------
            // 🔧 Configure Services
            // -------------------------------
            builder.Services.AddControllersWithViews();

            // ✅ Add HttpClient support for API calls
            builder.Services.AddHttpClient();

            // ✅ Add Session for storing JWTs or user info
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // -------------------------------
            // ⚙️ Configure Middleware
            // -------------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                // Show detailed errors while developing
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ✅ Enable session before auth
            app.UseSession();

            app.UseAuthorization();

            // -------------------------------
            // 🚀 Route Configuration
            // -------------------------------
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
