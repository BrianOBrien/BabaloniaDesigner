using System;
using System.ComponentModel;
using System.Reflection;
using Avalonia.Media;

namespace BabaloniaDesigner
{
    public class PropertyItem : INotifyPropertyChanged
    {
        public string Name { get; }
        public Type PropertyType { get; }
        public PropertyInfo PropertyInfo { get; }
        public object Target { get; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                    ApplyValue();
                }
            }
        }

        public PropertyItem(object target, PropertyInfo propertyInfo)
        {
            Target = target;
            PropertyInfo = propertyInfo;
            Name = propertyInfo.Name;
            PropertyType = propertyInfo.PropertyType;

            try
            {
                var current = propertyInfo.GetValue(target);
                _value = current?.ToString() ?? string.Empty;
            }
            catch
            {
                // Some properties may throw or require parameters; just show empty
                _value = string.Empty;
            }
        }

        private void ApplyValue()
        {
            try
            {
                object? converted = ConvertTo(PropertyType, _value);
                PropertyInfo.SetValue(Target, converted);
            }
            catch
            {
                // ignore parse errors for now
            }
        }

        private object? ConvertTo(Type type, string text)
        {
            if (type == typeof(string)) return text;
            if (type == typeof(double)) return double.Parse(text);
            if (type == typeof(int)) return int.Parse(text);
            if (type == typeof(bool)) return bool.Parse(text);
            if (typeof(IBrush).IsAssignableFrom(type))
                return Brush.Parse(text);

            return TypeDescriptor
                .GetConverter(type)
                .ConvertFromInvariantString(text);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
