using Blazux.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazux.Web
{
    public static class Extensions
    {

        public static IServiceCollection AddBlazux<TState>(this IServiceCollection services,
            Action<StoreBuilder<TState>> configurator, TState initialState = default)
        {
            var builder = new StoreBuilder<TState>(initialState);
            configurator.Invoke(builder);

            services.AddSingleton<IStore<TState>>(builder.Store);

            return services;
        }

    }
}
