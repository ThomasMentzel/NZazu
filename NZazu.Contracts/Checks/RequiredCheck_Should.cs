﻿using System;
using FluentAssertions;
using NEdifis.Attributes;
using NUnit.Framework;

namespace NZazu.Contracts.Checks
{
    [TestFixtureFor(typeof(RequiredCheck))]
    // ReSharper disable InconsistentNaming
    internal class RequiredCheck_Should
    {
        [Test]
        public void Throw_ValidationException_if_value_null_or_whitespace()
        {
            var check = new RequiredCheck();

            check.ShouldFailWith<ArgumentException>(null, null);
            check.ShouldFailWith<ArgumentException>(string.Empty, string.Empty);
            check.ShouldFailWith<ArgumentException>("\t\r\n", "\t\r\n");
            check.ShouldFailWith<ArgumentException>(" ", " ");

            check.Validate("a", "a").IsValid.Should().BeTrue();
        }
    }
}