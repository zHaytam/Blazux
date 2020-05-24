using PlainSample.Store;
using Blazux.Core;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlainSample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var store = new Store<State>(new State
            {
                CurrentWeather = new WeatherEntry()
            });

            store.AddReducer<IncrementCounterAction>(IncrementCounterAction.Reducer);
            store.AddReducer<IncrementWeatherAction>(IncrementWeatherAction.Reducer);
            builder.Services.AddSingleton<IStore<State>>(store);

            await builder.Build().RunAsync();
        }
    }
}
