using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuBoolField : NZazuField
    {
        public NZazuBoolField(string key) : base(key)
        {
            Type = "bool";
        }

        protected override Control GetValue()
        {
            return new CheckBox {Content = Hint, ToolTip = Description, IsChecked = null };
        }

        public override string Value
        {
            get { return ((CheckBox) ValueControl).IsChecked.ToString(); }
            set
            {
                var checkBox = ((CheckBox) ValueControl);
                bool isChecked;
                if (bool.TryParse(value, out isChecked))
                    checkBox.IsChecked = isChecked;
                else
                    checkBox.IsChecked = null;
            }
        }
    }
}