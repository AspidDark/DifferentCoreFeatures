using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using MediatR;

namespace ClassLibrary1
{
    public static class Entry
    {
        public static void MediatRInstaller(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(GetAdapterAssemblies());

        }

        private static Assembly[] GetAdapterAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.IsDynamic == false)
                .Where(x => x.FullName?.StartsWith("ClassLibrar") == true)
                .ToArray();

            return assemblies;
        }

    }
}
