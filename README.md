# Blazux

A performant Flux/Redux state management library that uses selectors to determine which components to re-render, just like Redux's `useSelector` hook.

|                    | Badges                                                                                                                                                                                                         |
| ------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| NuGet: Blazux.Core | [![NuGet](https://img.shields.io/nuget/v/Blazux.Core.svg)](https://www.nuget.org/packages/Blazux.Core) [![Nuget](https://img.shields.io/nuget/dt/Blazux.Core.svg)](https://www.nuget.org/packages/Blazux.Core) |
| NuGet: Blazux.Web  | [![NuGet](https://img.shields.io/nuget/v/Blazux.Web.svg)](https://www.nuget.org/packages/Blazux.Web) [![Nuget](https://img.shields.io/nuget/dt/Blazux.Web.svg)](https://www.nuget.org/packages/Blazux.Web)     |
| License            | [![GitHub](https://img.shields.io/github/license/zHaytam/DynamicExpressions.svg)](https://github.com/zHaytam/DynamicExpressions)                                                                               |

## Getting started

- [Documentation](https://github.com/zHaytam/Blazux): Tutorials, Features, etc...
- [Samples](https://github.com/zHaytam/Blazux/tree/master/samples):
  - [PlainSample](https://github.com/zHaytam/Blazux/tree/master/samples/PlainSample): This shows you how you can use the `Blazux.Core` package to handle everything yourself. This is useful when you want for example your components to not inherit `BlazuxComponent`.
  - [WebCounterSample](https://github.com/zHaytam/Blazux/tree/master/samples/WebCounterSample): This shows you how you can start using `Blazux` (using the `Blazux.Web` package) with low boilerplate. This is what most developers would want to start with.

## The Why

As of the time of writing (24/05/2020), there are a couple of state management libraries for Blazor. Some of them try to follow Flux/Redux, others do their own thing. They are nonetheless made by competent developers that want to help the Blazor community.

However, when an action is dispatched, all the components subscribed to the store get re-rendered. Here's why that's bad:

- Renders can be costly.
- The more subscribed components your current page has, the less performant it will be.
- If a subscribed component passes complex props (e.g. a class) to one or more children, they will also get re-rendered because Blazor doesn't handle this (details [here](https://docs.microsoft.com/en-us/aspnet/core/blazor/lifecycle?view=aspnetcore-3.1#after-parameters-are-set)).

## The Solution

If you're familiar with React & Redux, you probably know the solution by now.  
`react-redux` has the [useSelector](https://react-redux.js.org/api/hooks#useselector) hook, which lets you grab a "sub-state" from the store and subscribes your component in case it changes.

**Blazux** tries to do the same thing. While it only started out of my curiosity and experimenting, I think it is now more than usable.

## How it works

Here's how a Counter component would look like using Blazux:

```cs
public class CounterComponent : BlazuxComponent<State>
{
    protected int CurrentCount => UseSelector(s => s.CurrentCount);

    protected void Increment() => Dispatch(new IncrementCounterAction());
}
```

The `UseSelector(s => s.CurrentCount)` method tells the Store that this component is interested in the "sub-state" `CurrentCount` if this is the first time it's called, otherwise it'll simply return the value of `CurrentCount`.

When the `IncrementCounterAction` action is dispatched and the `CurrentCount` is changed, only the components subscribed to `CurrentCount` will get re-rendered, not all the components. This is the main goal of Blazux.

## Performance

- `UseSelector` takes less than 0.1 ms, regardless of the number of components in the page.
- `Dispatch` takes ~0.7 ms for 100 subscribed components in the page.

Basically, performance shouldn't be an issue regardless of how complex your application might be. You can look at the `PlainSample` project to see it in action.