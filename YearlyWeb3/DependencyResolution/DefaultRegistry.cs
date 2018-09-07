// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace YearlyWeb3.DependencyResolution {
    using MediatR;
    using StructureMap;
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;
	
    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());

                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });
            //For<IExample>().Use<Example>();
            For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
            For<IMediator>().Use<Mediator>();

            //var container = new Container(cfg =>
            //{
            //    cfg.Scan(scanner =>
            //    {
            //        //TODO FB  scanner.AssemblyContainingType<Ping>(); // Our assembly with requests & handlers
            //        scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            //        scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            //    });
            //    cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
            //    cfg.For<IMediator>().Use<Mediator>();
            //});

        }

        #endregion
    }
}