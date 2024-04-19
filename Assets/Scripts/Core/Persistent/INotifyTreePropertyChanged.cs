using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UnityEngine;

using UDebug = UnityEngine.Debug;

namespace Shuile.Persistent
{
    public delegate void TreePropertyChangedEventHandler(object value, string path);

    public interface INotifyTreePropertyChanged
    {
        public event TreePropertyChangedEventHandler OnTreePropertyChanged;

        public void InvokeTreePropertyChanged(object value, string path);
    }

    public static class INotifyTreePropertyChangedExtension
    {
        public static void UpdateClassProperty<TField>(
            this INotifyTreePropertyChanged notifier,
            ref TField field,
            TField value,
            [CallerMemberName] string caller = null) where TField : class
        {
            if (caller == null)
            {
                var errorMessage = "Updating a field with null property name";

#if DEBUG
                errorMessage += '\n';
                errorMessage += new StackTrace();
#endif

                UDebug.LogError(errorMessage);
                return;
            }

            if (field == value)
                return;

            if (value is INotifyTreePropertyChanged newSubNotifier)
            {
                var oldSubNotifier = field as INotifyTreePropertyChanged;
                if (oldSubNotifier != null)
                    oldSubNotifier.OnTreePropertyChanged -= BuildEventLinker(notifier, caller);

                if (newSubNotifier != null)
                    newSubNotifier.OnTreePropertyChanged += BuildEventLinker(notifier, caller);
            }

            field = value;
            notifier.InvokeTreePropertyChanged(value, caller);
        }

        public static void UpdateStructProperty<TField>(
            this INotifyTreePropertyChanged notifier,
            ref TField field,
            TField value,
            [CallerMemberName] string caller = null) where TField : struct, IEquatable<TField>
        {
            if (caller == null)
            {
                var errorMessage = "Updating a field with null property name";

#if DEBUG
                errorMessage += '\n';
                errorMessage += new StackTrace();
#endif

                UDebug.LogError(errorMessage);
                return;
            }

            if (field.Equals(value))
                return;

            if (value is INotifyTreePropertyChanged newSubNotifier)
            {
                var oldSubNotifier = field as INotifyTreePropertyChanged;
                if (oldSubNotifier != null)
                    oldSubNotifier.OnTreePropertyChanged -= BuildEventLinker(notifier, caller);

                if (newSubNotifier != null)
                    newSubNotifier.OnTreePropertyChanged += BuildEventLinker(notifier, caller);
            }

            field = value;
            notifier.InvokeTreePropertyChanged(value, caller);
        }

        public static TreePropertyChangedEventHandler BuildEventLinker(INotifyTreePropertyChanged notifier, string callerName)
        {
            return (value, path) => notifier.InvokeTreePropertyChanged(value, callerName + '.' + path);
        }
    }
}