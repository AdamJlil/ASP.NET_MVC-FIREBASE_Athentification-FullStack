using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(Full_projeject.AppStart))]

namespace Full_projeject
{
    public partial class AppStart
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new Microsoft.Owin.PathString("/User/Login"),
                LogoutPath = new Microsoft.Owin.PathString("/User/Logout"),
                ExpireTimeSpan = System.TimeSpan.FromMinutes(50.0)
            });
        }
    }
}
