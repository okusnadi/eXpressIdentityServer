﻿/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using Owin;
using Serilog;
using IdentityManager.Logging;
using IdentityManager.Core.Logging;
using eXpressIdentityServer.IdSvr;
using IdentityManager.Configuration;
using IdentityServer3.Core.Configuration;
using eXpressIdentityServer.IdMgr;
using Microsoft.Owin.Diagnostics;

namespace eXpressIdentityServer
{
    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            LogProvider.SetCurrentLogProvider(new DiagnosticsTraceLogProvider());
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Trace()
               .CreateLogger();

            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService("AspId");
                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    Factory = factory,                      
                    SecurityConfiguration =
                    {
                        RequireSsl = false
                    }
                });
            });

            app.Map("/core", core =>
            {
                var idSvrFactory = Factory.Configure("AspId");

                var options = new IdentityServerOptions
                {
                    SiteName = "IdentityServer3 - AspNetIdentity 2FA",
                    SigningCertificate = Certificate.Get(),
                    Factory = idSvrFactory,
                    RequireSsl = false                    
                };

                core.UseIdentityServer(options);
            });

            app.UseWelcomePage("/welcome");
            
        }
    }
}