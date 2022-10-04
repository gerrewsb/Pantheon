using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pantheon.Abstractions.Bases
{
    /// <summary>
    /// Baseclass that is usable in .NET frontends to notify the view of changes in a (view)model
    /// </summary>
	public class NotifyBase : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Used in the setter of a property of an implementing model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore">The private property passed as reference</param>
        /// <param name="value">The new value to assign to the property</param>
        /// <param name="propertyName">The name of the public property</param>
        /// <param name="onChanged">Optional action to perform if the value of the backingstore and the new value are different</param>
        /// <returns></returns>
        protected virtual bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
            {
                return false;
            }

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
