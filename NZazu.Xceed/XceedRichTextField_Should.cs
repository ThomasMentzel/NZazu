﻿using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using FluentAssertions;
using NEdifis.Attributes;
using NSubstitute;
using NUnit.Framework;
using NZazu.Contracts;
using NZazu.Extensions;
using NZazu.Fields;
using Xceed.Wpf.Toolkit;
using RichTextBox = Xceed.Wpf.Toolkit.RichTextBox;

namespace NZazu.Xceed
{
    [TestFixtureFor(typeof(XceedRichTextField))]
    [Apartment(ApartmentState.STA)]
    // ReSharper disable InconsistentNaming
    internal class XceedRichTextField_Should
    {
        private object ServiceLocator(Type type)
        {
            if (type == typeof(IValueConverter)) return NoExceptionsConverter.Instance;
            if (type == typeof(IFormatProvider)) return CultureInfo.InvariantCulture;
            throw new NotSupportedException($"Cannot lookup {type.Name}");
        }

        [Test]
        public void Be_Creatable()
        {
            var sut = new XceedRichTextField(new FieldDefinition {Key = "key"}, ServiceLocator);

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuWpfField>();
        }

        [Test]
        [STAThread]
        public void Override_ContentProperty_to_RichTextBox()
        {
            var field = new XceedRichTextField(new FieldDefinition {Key = "key"}, ServiceLocator);
            field.ContentProperty.Should().Be(RichTextBox.TextProperty);

            var textBox = (RichTextBox) field.ValueControl;

            field.GetValue().Should().BeNullOrEmpty();
            textBox.Text.Should().BeNullOrEmpty();

            field.SetValue("foobar");
            textBox.Text.Should().Contain(field.GetValue());

            textBox.Text = "not foobar";
            textBox.Text.Should().Contain(field.GetValue());
        }

        [Test]
        [STAThread]
        public void Set_Vertical_Scrollbar()
        {
            var field = new XceedRichTextField(new FieldDefinition {Key = "key"}, ServiceLocator);
            var textBox = (RichTextBox) field.ValueControl;
            textBox.VerticalScrollBarVisibility.Should().Be(ScrollBarVisibility.Auto);
        }

        [Test]
        [STAThread]
        public void Respect_Height_Setting()
        {
            var expectedHeight = 2 * XceedRichTextField.DefaultHeight;
            var definition = new FieldDefinition {Key = "key"};
            definition.Settings.Add("Height", expectedHeight.ToString(CultureInfo.InvariantCulture));

            var field = new XceedRichTextField(definition, ServiceLocator);
            field.ApplySettings(definition);

            var textBox = (RichTextBox) field.ValueControl;
            textBox.MinHeight.Should().Be(expectedHeight);
            textBox.MaxHeight.Should().Be(expectedHeight);

            field = new XceedRichTextField(new FieldDefinition {Key = "key"}, ServiceLocator);
            expectedHeight = XceedRichTextField.DefaultHeight;
            field.Definition.Settings.Add("Height", "not a number");

            textBox = (RichTextBox) field.ValueControl;
            textBox.MinHeight.Should().Be(expectedHeight);
        }

        [Test]
        [TestCase("rtf", typeof(RtfFormatter))]
        [TestCase("plain", typeof(PlainTextFormatter))]
        [TestCase("xaml", typeof(XamlFormatter))]
        [TestCase(null, typeof(RtfFormatter))]
        [TestCase("foobar", typeof(RtfFormatter))]
        [STAThread]
        public void Respect_Format_Setting(string format, Type formatterType)
        {
            var field = new XceedRichTextField(new FieldDefinition {Key = "key", Settings = {["Format"] = format}},
                ServiceLocator);

            var textBox = (RichTextBox) field.ValueControl;
            textBox.TextFormatter.Should().BeOfType(formatterType);
        }

        [Test]
        [STAThread]
        public void Add_optional_RichTextFormatBar()
        {
            var field = new XceedRichTextField(new FieldDefinition {Key = "key"}, ServiceLocator);
            var textBox = (RichTextBox) field.ValueControl;
            var formatBar = RichTextBoxFormatBarManager.GetFormatBar(textBox);
            formatBar.Should().BeNull();

            field = new XceedRichTextField(
                new FieldDefinition {Key = "key", Settings = {["ShowFormatBar"] = true.ToString()}}, ServiceLocator);

            textBox = (RichTextBox) field.ValueControl;
            formatBar = RichTextBoxFormatBarManager.GetFormatBar(textBox);
            formatBar.Should().NotBeNull();
        }

        [Test]
        [STAThread] // for NCrunch
        [Apartment(ApartmentState.STA)]
        public void Toggle_CallSigns_on_KeyGesture()
        {
            INZazuWpfField gistField = new XceedRichTextField(new FieldDefinition {Key = "myGist"}, ServiceLocator);
            gistField.GetValue().Should().BeNullOrWhiteSpace();

            var routedEvent = Keyboard.KeyUpEvent;
            var source = Substitute.For<PresentationSource>();
            var keyGesture = new KeyGesture(Key.Home);
            var key = keyGesture.Key;
            var device = Keyboard.PrimaryDevice;
            var eventArgs = new KeyEventArgs(device, source, 0, key) {RoutedEvent = routedEvent};

            var textBox = (RichTextBox) gistField.ValueControl;
            textBox.RaiseEvent(eventArgs);
            gistField.GetValue().Should().BeNullOrWhiteSpace("neither from or to has value that could be toggled");
        }
    }

    #region converter tests

    [TestFixtureFor(typeof(RtfSpecialCharactersConverter))]
    internal class RtfSpecialCharactersConverter_Should
    {
        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Be_Creatable()
        {
            var sut = new RtfSpecialCharactersConverter();
            sut.Should().NotBeNull();
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Not_Convert_When_Invalid_Parameters()
        {
            var sut = new RtfSpecialCharactersConverter();

            object result = null;

            Assert.DoesNotThrow(() => result = sut.Convert(null, typeof(string), null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);

            Assert.DoesNotThrow(() => result = sut.Convert(new object(), typeof(string), null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);

            Assert.DoesNotThrow(() => result = sut.Convert("some string", null, null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);

            Assert.DoesNotThrow(() => result = sut.Convert("some string", typeof(object), null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Not_Convert_Back_When_Invalid_Parameters()
        {
            var sut = new RtfSpecialCharactersConverter();

            object result = null;

            Assert.DoesNotThrow(() => result = sut.ConvertBack(null, typeof(string), null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);
            
            Assert.DoesNotThrow(() => result = sut.ConvertBack(new object(), typeof(string), null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);
            
            Assert.DoesNotThrow(() => result = sut.ConvertBack("some string", null, null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);
            
            Assert.DoesNotThrow(() => result = sut.ConvertBack("some string", typeof(object), null, CultureInfo.CurrentCulture));
            result.Should().Be(Binding.DoNothing);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Convert_When_No_Special_Character()
        {
            var sut = new RtfSpecialCharactersConverter();

            const string input = "some input string";
            const string expected = "some input string";

            var result = sut.Convert(input, typeof(string), null, CultureInfo.CurrentCulture) as string;

            result.Should().NotBeNull();
            result.Should().Contain(expected);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Convert_When_Special_Character()
        {
            var sut = new RtfSpecialCharactersConverter();

            const string input = "some weird input string: äöü ß";
            const string expected = "some weird input string: \\'e4\\'f6\\'fc \\'df";

            var result = sut.Convert(input, typeof(string), null, CultureInfo.CurrentCulture) as string;

            result.Should().NotBeNull();
            result.Should().Contain(expected);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Convert_Back_When_No_Special_Character()
        {
            var sut = new RtfSpecialCharactersConverter();

            const string input = "some input string";
            const string expected = "some input string";

            var result = sut.ConvertBack(input, typeof(string), null, CultureInfo.CurrentCulture) as string;

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Convert_Back_When_Special_Character()
        {
            var sut = new RtfSpecialCharactersConverter();

            const string input = "some weird input string: \\'e4\\'f6\\'fc \\'df";
            const string expected = "some weird input string: äöü ß";

            var result = sut.ConvertBack(input, typeof(string), null, CultureInfo.CurrentCulture) as string;

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }

        [Test]
        [RequiresThread(ApartmentState.STA)]
        public void Convert_Back_And_Forth()
        {
            var sut = new RtfSpecialCharactersConverter();

            // ReSharper disable once StringLiteralTypo
            const string expected = "äöüß一個mộtля";

            var converted = sut.Convert(expected, typeof(string), null, CultureInfo.CurrentCulture);
            var result = sut.ConvertBack(converted, typeof(string), null, CultureInfo.CurrentCulture);

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }
    }

    #endregion
}