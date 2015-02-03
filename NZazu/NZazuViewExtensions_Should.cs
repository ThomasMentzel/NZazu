using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuViewExtensions_Should
    {
        [Test]
        public void Return_field_values_on_GetFieldValues()
        {
            var view = Substitute.For<INZazuView>();

            const string key = "name";
            const string value = "John";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key } } };
            view.FormDefinition = formDefinition;

            var field = Substitute.For<INZazuField>();
            field.Value = value;
            view.GetField(key).Returns(field);

            var actual = view.GetFieldValues();
            actual.Keys.Single().Should().Be(key);
            actual[key].Should().Be(value);
        }

        [Test]
        public void Set_field_values_on_SetFieldValues()
        {
            var view = Substitute.For<INZazuView>();

            const string key = "name";
            const string value = "John";

            var formDefinition = new FormDefinition { Fields = new[] { new FieldDefinition { Key = key } } };
            view.FormDefinition = formDefinition;

            var field = Substitute.For<INZazuField>();
            view.GetField(key).Returns(field);

            var input = new Dictionary<string, string> {{key, value}};

            view.SetFieldValues(input);

            field.Value.Should().Be(value);
        }
    }
}