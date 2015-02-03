using System;
using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuField : INZazuField
    {
        private readonly Lazy<Control> _label;
        private readonly Lazy<Control> _value;
        public string Type { get; protected set; }
        public NZazuField(string key)
        {
            if (String.IsNullOrWhiteSpace(key)) throw new ArgumentException("key");
            Key = key;
            Type = "label";

            _label = new Lazy<Control>(GetLabel);
            _value = new Lazy<Control>(GetValue);
        }

        public string Key { get; private set; }
        public string Prompt { get; protected internal set; }
        public string Hint { get; protected internal set; }
        public string Description { get; protected internal set; }

        public virtual string Value
        {
            get { return String.Empty; }
            set { }
        }

        public Control LabelControl { get { return _label.Value; } }
        public Control ValueControl { get { return _value.Value; } }

        protected virtual Control GetLabel() { return !String.IsNullOrWhiteSpace(Prompt) ? new Label { Content = Prompt } : null; }
        protected virtual Control GetValue() { return !String.IsNullOrWhiteSpace(Description) ? new Label { Content = Description } : null; }
    }
}