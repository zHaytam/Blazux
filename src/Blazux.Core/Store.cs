﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Blazux.Core
{
    public class Store<TState> : IStore<TState>
    {
        private readonly Dictionary<Type, List<Func<TState, IAction, TState>>> _reducersByAction;
        private readonly Dictionary<IBlazuxComponent, Dictionary<string, Subscription<TState>>> _subscriptionsByComponent;

        public Store(TState initialState = default)
        {
            _reducersByAction = new Dictionary<Type, List<Func<TState, IAction, TState>>>();
            _subscriptionsByComponent = new Dictionary<IBlazuxComponent, Dictionary<string, Subscription<TState>>>();

            State = initialState;
        }

        public TState State { get; private set; }

        public T UseSelector<T>(Expression<Func<TState, T>> selectorExpr, IBlazuxComponent component)
        {
            var selectorRepresentation = selectorExpr.ToString();

            if (!_subscriptionsByComponent.ContainsKey(component))
                _subscriptionsByComponent.Add(component, new Dictionary<string, Subscription<TState>>());

            if (!_subscriptionsByComponent[component].ContainsKey(selectorRepresentation))
            {
                var selector = selectorExpr.Compile();
                _subscriptionsByComponent[component].Add(selectorRepresentation, new Subscription<TState>
                {
                    Selector = s => selector(s),
                    LatestValue = selector(State)
                });
            }

            return (T)_subscriptionsByComponent[component][selectorRepresentation].LatestValue;
        }

        public T UseSelector<T, C>(Func<TState, T> selector, C component, string fieldName) where C : IBlazuxComponent
        {
            var field = typeof(C).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            var typedSetter = field.BuildFieldSetter<C>();
            Action<object, object> setter = (c, v) => typedSetter((C)c, v);

            if (!_subscriptionsByComponent.ContainsKey(component))
                _subscriptionsByComponent.Add(component, new Dictionary<string, Subscription<TState>>());

            _subscriptionsByComponent[component].Add(fieldName, new Subscription<TState>
            {
                Selector = s => selector(s),
                LatestValue = selector(State),
                FieldSetter = setter
            });

            return (T)_subscriptionsByComponent[component][fieldName].LatestValue;
        }

        public void AddReducer<TAction>(Func<TState, TAction, TState> reducer) where TAction : IAction
        {
            var actionType = typeof(TAction);

            if (!_reducersByAction.ContainsKey(actionType))
                _reducersByAction.Add(actionType, new List<Func<TState, IAction, TState>>());

            Func<TState, IAction, TState> func = (s, a) => reducer(s, (TAction)a);
            _reducersByAction[actionType].Add(func);
        }

        public void AddReducersFromAssembly(Assembly assembly, params Type[] typesToExclude)
        {
            foreach (var type in assembly.GetTypes().Where(t => !typesToExclude.Contains(t)))
            {
                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
                {
                    if (!method.IsReducer<TState>(out var actionType))
                        continue;

                    if (!_reducersByAction.ContainsKey(actionType))
                        _reducersByAction.Add(actionType, new List<Func<TState, IAction, TState>>());

                    var reducerFunc = method.GetReducerFunc<TState>(actionType);
                    _reducersByAction[actionType].Add(reducerFunc);
                }
            }
        }

        public void Dispatch(IAction action)
        {
            var actionType = action.GetType();

            if (!_reducersByAction.ContainsKey(actionType))
                return;

            TState newState = State;

            foreach (var reducer in _reducersByAction[actionType])
            {
                newState = reducer(newState, action);
            }

            if (!newState.Equals(State))
            {
                ProcessNewState(newState);
                State = newState;
            }
        }

        private void ProcessNewState(TState newState)
        {
            var componentsToReRender = new List<IBlazuxComponent>(_subscriptionsByComponent.Count);

            foreach ((var component, var subscriptions) in _subscriptionsByComponent)
            {
                var shouldReRender = false;

                foreach (var subscription in subscriptions.Values)
                {
                    var oldValue = subscription.LatestValue;
                    var newValue = subscription.Selector(newState);

                    if (!Equals(oldValue, newValue))
                    {
                        subscription.LatestValue = newValue;
                        subscription.FieldSetter?.Invoke(component, newValue);
                        shouldReRender = true;
                    }
                }

                if (shouldReRender)
                    componentsToReRender.Add(component);
            }

            foreach (var component in componentsToReRender)
                component.StateHasChanged();
        }

        public void Unsubscribe(IBlazuxComponent component) => _subscriptionsByComponent.Remove(component);
    }
}