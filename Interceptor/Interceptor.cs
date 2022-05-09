using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InterceptPipeline
{
    public static class Interceptor
    {

        public static void AddInteceptedSingleton<TInterface, TImplementation, TInterceptor>
            (this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
            where TInterceptor : class, IInterceptor
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddSingleton<TImplementation>();
            services.TryAddTransient<TInterceptor>();
            services.AddSingleton(provider => {
                var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                var implementation = provider.GetRequiredService<TImplementation>();
                var interceptor = provider.GetRequiredService<IInterceptor>();
                return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
            });
        
        }
    }
}
