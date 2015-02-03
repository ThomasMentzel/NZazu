using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuField_Should
    {
        [Test]
        public void Validate_ctor_parameters()
        {
            new Action(() => new NZazuField("")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new NZazuField(null)).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
            new Action(() => new NZazuField("\t\r\n ")).Invoking(a => a.Invoke()).ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuField("test");
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuField("test")
            {
                Prompt = "superhero"
            };

            var label = (Label)sut.LabelControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Prompt);
        }

        [Test]
        public void Create_ValueControl_Matching_Description()
        {
            var sut = new NZazuField("test")
            {
                Description = "superhero is alive"
            };

            var label = (Label)sut.ValueControl;
            label.Should().NotBeNull();
            label.Content.Should().Be(sut.Description);
        }

        [Test]
        public void Not_Create_ValueControl_On_Empty_Description()
        {
            var sut = new NZazuField("test");

            var label = (Label)sut.ValueControl;
            label.Should().BeNull();
        }

        [Test]
        public void Get_Set_Value_should_do_nothing()
        {
            var sut = new NZazuField("test");
            sut.Value.Should().BeEmpty();

            sut.Value = "test";

            sut.Value.Should().BeEmpty();
        }
    }
}