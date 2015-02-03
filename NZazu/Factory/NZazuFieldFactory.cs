using System;
using System.Diagnostics;
using NZazu.Contracts;
using NZazu.Fields;

namespace NZazu
{
    class NZazuFieldFactory : INZazuFieldFactory
    {
        public INZazuField CreateField(FieldDefinition fieldDefinition)
        {
            if (fieldDefinition == null) throw new ArgumentNullException("fieldDefinition");

            NZazuField field = null;
            
            switch (fieldDefinition.Type)
            {
                case "string": field = new NZazuTextField(fieldDefinition.Key);
                    break;
                case "bool": field = new NZazuBoolField(fieldDefinition.Key);
                    break;
                case "label":
                default:
                    field = new NZazuField(fieldDefinition.Key);
                    if (fieldDefinition.Type != "label")
                        //throw new NotSupportedException("The specified field type is not supported: " + fieldDefinition.Type);
                        Trace.TraceWarning("The specified field type is not supported: " + fieldDefinition.Type);
                    break;
            }

            field.Prompt = fieldDefinition.Prompt;
            field.Hint = fieldDefinition.Hint;
            field.Description = fieldDefinition.Description;

            return field;
        }
    }
}