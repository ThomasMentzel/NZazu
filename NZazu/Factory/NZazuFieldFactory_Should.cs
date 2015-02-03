using FluentAssertions;
using NUnit.Framework;
using NZazu.Contracts;

namespace NZazu
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    class NZazuFieldFactory_Should
    {
        [Test]
        public void Be_Creatable()
        {
            var sut = new NZazuFieldFactory();

            sut.Should().NotBeNull();
            sut.Should().BeAssignableTo<INZazuFieldFactory>();
        }

        [Test]
        [TestCase(null)]
        [TestCase("label")]
        [TestCase("string")]
        [TestCase("bool")]
        public void Support(string fieldType)
        {
            var sut = new NZazuFieldFactory();

            var field = sut.CreateField(new FieldDefinition { Key = "test", Type = fieldType });

            field.Should().NotBeNull();
            field.Type.Should().Be(fieldType ?? "label"); // because of the fallback in case of null
        }
    }
}