using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Moq;
using Ninject;
using GreenSol.Domain.Abstract;
using GreenSol.Domain.Entities;
using GreenSol.Domain.Concrete;
using System.Configuration;

namespace GreenSol.UI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            Mock<IAlbumRepository> mock = new Mock<IAlbumRepository>();
            
            //mock.Setup(m => m.Albums).Returns(new List<Album> {
            //    new Album { 
            //        Artist = new Artist {Name = "Rush"},
            //        Genre = new Genre {Name = "Rock"},
            //        Price = 9.99m,
            //        Name = "Caravan"
            //        }              
            //    });

            //kernel.Bind<IAlbumRepository>().ToConstant(mock.Object);

            kernel.Bind<IAlbumRepository>().To<EFAlbumRepository>();

            EmailSettings emailSettings = new EmailSettings
            {
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            };

            kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);
        }
    }
}