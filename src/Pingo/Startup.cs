using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pingo.CustomAuthR;
using Pingo.Hubs;

namespace Pingo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddSingleton<RoomManager>();
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.AddAuthentication("my_scheme")
                .AddCookie("my_scheme", options =>
                {
                    options.Cookie.HttpOnly = false;
                    options.Cookie.Name = "AuthCookie";
                });

            services.AddCors(options =>
            {
                options.AddPolicy("frontend", policy =>
                {
                    policy.WithOrigins("http://localhost:8080").AllowAnyMethod().AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("frontend");

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHub<ChatHub>("/ChatHub");
            });
        }
    }

    public class Room
    {
        public int Id { get; set; }
        public List<string> Users { get; set; }

        public Room()
        {
            Users = new List<string>();
        }
    }
    public class RoomManager
    {
        public RoomManager()
        {
            Rooms = new List<Room>();
        }
        public List<Room> Rooms { get; set; }
    }
}
