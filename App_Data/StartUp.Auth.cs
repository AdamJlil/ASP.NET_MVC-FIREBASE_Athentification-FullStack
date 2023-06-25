using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Full_projeject.App_Data
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new Microsoft.Owin.PathString("/User/Login"),
                LogoutPath = new Microsoft.Owin.PathString("/User/Logout"),
                ExpireTimeSpan = System.TimeSpan.FromMinutes(25.0)
            });
        }
    }
}