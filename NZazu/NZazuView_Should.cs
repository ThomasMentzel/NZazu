using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Fields;

namespace NZazu
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuView_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuView();

            sut.Should().NotBeNull();
        }

        [Test]
        [TestCase("fallback", null, "Heading", null, "fallback text", typeof(Label))]
        [TestCase("heading", "label", "Settings", null, "You can manage your account here. Use TAB ...", typeof(Label))]
        [TestCase("userName", "string", "User", "Enter your name", "Your name", typeof(TextBox))]
        [TestCase("gender", "bool", "Admin", "Is Admin", "Check if you are an admin", typeof(CheckBox))]
        //[TestCase("birthday", "dateTime", "Birthday", "Choose birthday", "Your birthday", typeof (DatePicker))]
        //[TestCase("weight", "double", "Weight", "Enter body weight (kg)", "Your weight", typeof(TextBox))]
        // ReSharper disable once TooManyArguments
        public void Support_field(string key, string type, string prompt, string hint, string description, Type controlType)
        {
            var sut = new NZazuView();

            var formDefinition = new FormDefinition
            {
                Fields = new[]
                {
                    new FieldDefinition
                    {
                        Key = key, 
                        Type = type,
                        Prompt = prompt,
                        Hint = hint,
                        Description = description
                    }
                }
            };

            sut.FormDefinition = formDefinition;

            var field = (NZazuField)sut.GetField(key);

            field.Should().NotBeNull();
            field.Key.Should().Be(key);
            field.Type.Should().Be(type ?? "label"); // to make sure fallback is used
            field.Prompt.Should().Be(prompt);
            field.Hint.Should().Be(hint);
            field.Description.Should().Be(description);

            var control = field.ValueControl;
            control.Should().BeOfType(controlType);

            VerifyControl(sut, field);
        }

        [Test]
        public void Update_when_FormDefinition_changed()
        {
            var sut = new NZazuView();

            var layout = Substitute.For<INZazuLayoutStrategy>();
            sut.LayoutStrategy = layout;
            layout.ClearReceivedCalls();

            sut.FormDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };

            layout.Received().DoLayout(sut.Layout, Arg.Any<IEnumerable<INZazuField>>());
        }

        [Test]
        public void Update_when_FieldFactory_changed()
        {
            var layout = Substitute.For<INZazuLayoutStrategy>();
            var sut = new NZazuView();
            sut.LayoutStrategy = layout;
            sut.FieldFactory = Substitute.For<INZazuFieldFactory>();
            sut.FormDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };


            layout.ClearReceivedCalls();
            var fieldFactory = Substitute.For<INZazuFieldFactory>();
            sut.FieldFactory = fieldFactory;

            layout.Received().DoLayout(sut.Layout, Arg.Any<IEnumerable<INZazuField>>());
        }


        [Test]
        public void Update_when_LayoutStrategy_changed()
        {
            var sut = new NZazuView();
            sut.FieldFactory = Substitute.For<INZazuFieldFactory>();
            sut.FormDefinition = new FormDefinition { Fields = new FieldDefinition[] { } };

            var layoutStrategy = Substitute.For<INZazuLayoutStrategy>();

            // change strategy
            sut.LayoutStrategy = layoutStrategy;

            layoutStrategy.Received().DoLayout(sut.Layout, Arg.Any<IEnumerable<INZazuField>>());
        }

        [Test]
        public void Disallow_null_FieldFactory()
        {
            var sut = new NZazuView();
            sut.FieldFactory.Should().NotBeNull();

            sut.FieldFactory = null;

            sut.FieldFactory.Should().NotBeNull();
        }

        [Test]
        public void Disallow_null_LayoutStrategy()
        {
            var sut = new NZazuView();
            sut.LayoutStrategy.Should().NotBeNull();

            sut.LayoutStrategy = null;

            sut.LayoutStrategy.Should().NotBeNull();
        }

        private static void VerifyControl(NZazuView sut, NZazuField field)
        {
            var control = field.ValueControl;
            var child = FindChild(sut, ctrl => Equals(ctrl, control));
            child.Should().NotBeNull();
        }

        private static DependencyObject FindChild(DependencyObject parent, Predicate<DependencyObject> matches)
        {
            if (parent == null) return null;

            var children = LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>();
            foreach (var child in children)
            {
                if (matches(child))
                    return child;
                var foundChild = FindChild(child, matches);
                if (matches(foundChild))
                    return foundChild;
            }
            return null;
        }

    }
}
