using System;
using System.Reflection;
using Test.Interfaces;

namespace Test.Models
{
    public class TabResolver : ITabResolver
    {
        private readonly IServiceProvider serviceProvider;

        public TabResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ITab Resolve(string name)
        {
            var type = Assembly.GetAssembly(typeof(TabResolver)).GetType($"Test.Models.{name}Tab");

            return serviceProvider.GetService(type) as ITab;
        }
    }
}
