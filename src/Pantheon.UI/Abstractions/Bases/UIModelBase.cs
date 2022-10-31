using System.Reflection;
using System.Text.Json;

namespace Pantheon.Abstractions.Bases
{
    /// <summary>
    /// <para>Base model for a .NET frontend model with a string for ID</para>
    /// <para>Implements the INotifyPropertyChanged interface through the <see cref="NotifyBase"/> class</para>
    /// </summary>
	public abstract class UIModelBase : UIModelBase<string>
	{
		public UIModelBase()
		{
            ID = Guid.NewGuid().ToString();
		}
	}

    /// <summary>
    /// <para>Base model for a .NET frontend model with an equatable ID</para>
    /// <para>Implements the INotifyPropertyChanged interface through the <see cref="NotifyBase"/> class</para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
	public abstract class UIModelBase<TKey> : NotifyBase
		where TKey : IEquatable<TKey>
	{
        private string? _original;

        public TKey ID { get; set; } = default!;

        /// <summary>
        /// <para>Clears the original property</para>
        /// <para>If the original property is null, the <see cref="HasChanges(JsonSerializerOptions?)"/> method will always return true</para>
        /// </summary>
        public void ClearOriginal() => _original = null;

        /// <summary>
        /// Sets the original property
        /// </summary>
        /// <param name="jsonSerializerOptions"></param>
        public void SetOriginal(JsonSerializerOptions? jsonSerializerOptions = null) => _original = JsonSerializer.Serialize(this, jsonSerializerOptions);
        
        /// <summary>
        /// <para>Compares the current model with the original model</para>
        /// <para>If the original is not set or was cleared, this method will always return true</para>
        /// </summary>
        /// <param name="jsonSerializerOptions"></param>
        /// <returns><see cref="true"/> or <see cref="false"/> based on the comparison between the current model and the original</returns>
        public bool HasChanges(JsonSerializerOptions? jsonSerializerOptions = null) => _original != JsonSerializer.Serialize(this, jsonSerializerOptions);

        /// <summary>
		/// Reset the current object to the original values.
		/// If there is no original value, there will be no reset
		/// </summary>
		public virtual void Reset(JsonSerializerOptions? jsonSerializerOptions = null)
        {
            if (_original == null)
            {
                return;
            }

            object originalClass = JsonSerializer.Deserialize(_original, GetType(), jsonSerializerOptions)!;

            foreach (PropertyInfo property in originalClass.GetType().GetProperties())
            {
                if (property.CanWrite && property.GetSetMethod(true)?.IsPublic == true)
                {
                    property.SetValue(this, property.GetValue(originalClass));
                }
            }
        }
    }
}
