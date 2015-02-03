using System;
using System.Windows.Controls;
using FluentAssertions;
using NUnit.Framework;

namespace NZazu.Fields
{
    [TestFixture]
    [RequiresSTA]
    // ReSharper disable InconsistentNaming
    class NZazuTextField_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuTextField("test");

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuField>();
        }

        [Test]
        public void Not_Create_Empty_Label()
        {
            var sut = new NZazuTextField("test");
            sut.LabelControl.Should().BeNull();
        }

        [Test]
        public void Create_Label_Matching_Prompt()
        {
            var sut = new NZazuTextField("test")
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
            var sut = new NZazuTextField("test")
            {
                Hint = "superhero",
                Description = "check this if you are a registered superhero"
            };

            var label = (TextBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Text.Should().BeEmpty();
            label.ToolTip.Should().Be(sut.Description);
        }

        [Test]
        public void Create_ValueControl_Even_If_Empty_Hint()
        {
            var sut = new NZazuTextField("test");

            var label = (TextBox)sut.ValueControl;
            label.Should().NotBeNull();
            label.Text.Should().BeEmpty();
        }

        [Test]
        public void Get_Set_Value_should_propagate_to_ValueControl()
        {
            var sut = new NZazuTextField("test");
            sut.Value.Should().BeEmpty();

            sut.Value = "test";
            sut.Value.Should().Be("test");
            ((TextBox)sut.ValueControl).Text.Should().Be("test");

            ((TextBox) sut.ValueControl).Text = String.Empty;
            sut.Value.Should().BeEmpty();
        }

    }
}