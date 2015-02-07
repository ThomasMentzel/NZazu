﻿using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;
using Xceed.Wpf.Toolkit;

namespace NZazu.Xceed
{
    [TestFixture, RequiresSTA]
    // ReSharper disable InconsistentNaming
    class XceedFieldFactory_Should
    {
        [Test]
        public void Create_WatermarkTextbox()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "login",
                Type = "string",
                Prompt = "login",
                Hint = "Enter user name",
                Description = "Your User name"
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<XceedTextBoxField>();

            var textBox = (WatermarkTextBox) field.ValueControl;
            textBox.Watermark.Should().Be(fieldDefinition.Hint);
        }

        [Test]
        public void Create_DateTimePicker()
        {
            var sut = new XceedFieldFactory();
            var fieldDefinition = new FieldDefinition
            {
                Key = "birthday",
                Type = "date",
                Prompt = "Date of Birth",
                Hint = "Enter date of birth",
                Description = "Your birthday",
            };
            var field = sut.CreateField(fieldDefinition);
            field.Should().BeOfType<XceedDateTimeField>();

            var datePicker = (DateTimePicker) field.ValueControl;
            datePicker.Watermark.Should().Be(fieldDefinition.Hint);
            datePicker.FormatString.Should().BeNull();

            const string dateFormat = "yyyy_MM_dd";
            fieldDefinition.Settings = new Dictionary<string, string> { { "Format", dateFormat } };

            field = sut.CreateField(fieldDefinition);
            datePicker = (DateTimePicker)field.ValueControl;
            datePicker.FormatString.Should().Be(dateFormat);
        }

    }
}