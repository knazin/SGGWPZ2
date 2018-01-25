using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGGWPZ.Models;
using SGGWPZ.Repositories;

namespace SGGWPZ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            using (var client = new PlanContext())
            {
                client.Database.EnsureCreated();

                try
                {
                    Rodzaj_uzytkownika rodzaj_Uzytkownika = new Rodzaj_uzytkownika();
                    rodzaj_Uzytkownika.rodzajuzytkownikaId = 1;
                    rodzaj_Uzytkownika.rodzajuzytkownika = "Admin";
                    client.Rodzaj_uzytkownika.Add(rodzaj_Uzytkownika);
                    client.SaveChanges();

                    Uzytkownik uzytkownik = new Uzytkownik();
                    uzytkownik.rodzajuzytkownikaId = 1;
                    uzytkownik.login = "admin";
                    uzytkownik.haslo = "1234";
                    uzytkownik.rodzajuzytkownikaId = 1;
                    client.Uzytkownik.Add(uzytkownik);
                    client.SaveChanges();
                }
                catch (Exception) { }
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
              .AddSessionStateTempDataProvider();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddEntityFrameworkSqlite().AddDbContext<PlanContext>();

            services.AddScoped<IUniversalRepositoryTypeOf, UniversalRepositoryTypeOf>();
            //services.AddScoped(typeof(IUniversalRepositoryTypeOf<>), typeof(UniversalRepositoryTypeOf<>)); // BARDZO UNIWERSALNE

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSession();
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
