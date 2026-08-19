using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutoLotManager.ViewModel
{
    /// <summary>
    /// Base class for every view model in the application. It supplies the
    /// <see cref="INotifyPropertyChanged"/> plumbing that WPF data binding
    /// relies on, so derived types only need to call
    /// <see cref="OnPropertyChanged"/> from their property setters.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        // TODO: implement common dependencies to be injected into all view models
        /// <summary>
        /// Initialises the base view model. Currently does no work; it exists
        /// as the future injection point for dependencies shared by all view
        /// models, such as logging or data services.
        /// </summary>
        public ViewModelBase()
        {

        }

        /// <summary>
        /// Raised when a bound property value has changed. WPF subscribes to
        /// this to refresh the UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the given property. Safe to
        /// call when nothing is bound, as the event is invoked only if it has
        /// subscribers.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed. Supplied automatically by the
        /// compiler from the calling member, so setters can call this with no
        /// arguments; pass a name explicitly only when raising the event on
        /// behalf of a different property.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
