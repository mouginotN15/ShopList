using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace Tuto_System.Reactive
{
    public static class NotifyPropertyChangeReactiveExtensions
    {
        // Returns the values of property (an Expression) as they
        // change, starting with the current value
        public static IObservable<TValue>
            GetPropertyValues<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property)
                where TSource : INotifyPropertyChanged
        {
            MemberExpression memberExpression = property.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException(
                    "property must directly access " +
                    "a property of the source");
            }

            string propertyName = memberExpression.Member.Name;

            Func<TSource, TValue> accessor = property.Compile();

            return source.GetPropertyChangedEvents()
                .Where(x => x.EventArgs.PropertyName == propertyName)
                .Select(x => accessor(source))
                .StartWith(accessor(source));
        }

        // This is a wrapper around FromEvent(PropertyChanged)
        public static IObservable<EventPattern<PropertyChangedEventArgs>>
            GetPropertyChangedEvents(this INotifyPropertyChanged source)
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler,
                PropertyChangedEventArgs>(
                h => h.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h);
        }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer)
            where TSource : INotifyPropertyChanged
        {
            return source
                    .GetPropertyValues(property)
                    .Subscribe(observer);
        }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer,
                Action onCompleted)
            where TSource : INotifyPropertyChanged
        {
            return source
                    .GetPropertyValues(property)
                    .Subscribe(observer, onCompleted);
        }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer,
                Action<Exception> onException)
            where TSource : INotifyPropertyChanged
        {
            return source
                    .GetPropertyValues(property)
                    .Subscribe(observer, onException);
        }

        public static IDisposable
            Subscribe<TSource, TValue>(
                this TSource source,
                Expression<Func<TSource, TValue>> property,
                Action<TValue> observer,
                Action<Exception> onException,
                Action onCompleted)
            where TSource : INotifyPropertyChanged
        {
            return source
                    .GetPropertyValues(property)
                    .Subscribe(observer, onException, onCompleted);
        }
    }
}
