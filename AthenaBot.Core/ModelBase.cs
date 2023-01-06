using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AthenaBot
{
    /// <summary>
    /// A base class for INotifyPropertyChanged using Expression trees to allow shorthanding PropertyChanged events using lambdas.
    /// Designed to speed up class definitions that implement the MVVM and/or observer patterns.
    /// </summary>
    public abstract class ModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Uses lambda expressions to select a field and modify the field using Reflection with a new value. 
        /// If the value has changed, the PropertyChanged event will be raised using the CallerMemberName (ie, from inside a property 'set').
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldSelector">An expression that selects the field being modified.</param>
        /// <param name="newValue">The new value of the field.</param>
        /// <param name="propertyName">The name of the property being changed. Compiler attribute CallerMemberName will be auto-filled if not supplied.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected void OnPropertyChanged<T>(Expression<Func<T>> fieldSelector, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (fieldSelector.Body is not MemberExpression body)
                throw new ArgumentException("Field selector must be a member access expression.", nameof(fieldSelector));

            var member = body.Member as FieldInfo;
            if (member == null) throw new InvalidOperationException("Field selector must return a field.");

            T oldValue = (T)member.GetValue(this);

            if (!Equals(oldValue, newValue))
            {
                member.SetValue(this, newValue);
                OnPropertyChanging(propertyName);
            }
        }

        /// <summary>
        /// Uses lambda expressions to select a field and modify the field using Reflection with a new value. 
        /// If the value has changed, the PropertyChanged event will be raised using the name of a property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propSelector">An expression that selects the property that changed.</param>
        /// <param name="fieldSelector">An expression that selects the field being modified.</param>
        /// <param name="newValue">The new value of the field.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propSelector, Expression<Func<T>> fieldSelector, T newValue)
        {
            if (fieldSelector.Body is not MemberExpression body)
                throw new ArgumentException("Field selector must be a member access expression.", nameof(fieldSelector));

            var member = body.Member as FieldInfo;
            if (member == null) throw new InvalidOperationException("Field selector must return a field.");

            T oldValue = (T)member.GetValue(this);

            if (!Equals(oldValue, newValue))
            {
                body = propSelector.Body as MemberExpression;
                if (body == null) throw new ArgumentException("Property selector must be a member access expression.", nameof(propSelector));

                var prop = body.Member as PropertyInfo;
                if (prop == null) throw new InvalidOperationException("Property selector must return a property.");

                member.SetValue(this, newValue);
                OnPropertyChanging(prop.Name);
            }
        }
        /// <summary>
        /// Uses a lambda expression to raise the PropertyChanged event using the name of the property selected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propSelector">The property that changed.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propSelector)
        {
            if (propSelector.Body is not MemberExpression body)
                throw new ArgumentException("Property selector must be a member access expression.", nameof(propSelector));

            var prop = body.Member as PropertyInfo;
            if (prop == null) throw new InvalidOperationException("Property selector must return a property.");

            OnPropertyChanging(prop.Name);
        }

        /// <summary>
        /// Raises the PropertyChanged event using the CallerMemberName
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (!string.IsNullOrEmpty(propertyName))
                OnPropertyChanging(propertyName);
        }
        /// <summary>
        /// Provides a mechanism for overriding, or preventing a PropertyChanged event on some extra condition.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanging(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }
        /// <summary>
        /// Raises the Propertycnanged event
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
