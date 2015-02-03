using System.Windows.Controls;

namespace NZazu.Fields
{
    class NZazuTextField : NZazuField
    {
        public NZazuTextField(string key) : base(key)
        {
            Type = "string";
        }

        protected override Control GetValue()
        {
            return new TextBox
            {
                //Text = Hint, 
                ToolTip = Description
            };
        }

        public override string Value
        {
            get { return ((TextBox) ValueControl).Text; }
            set { ((TextBox) ValueControl).Text = value; }
        }
    }
}