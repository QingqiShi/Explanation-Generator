using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ExplanationGenerator.UnitTests
{
    public class TestFrameworkTest
    {
        [TestCase(1, 2, 3)]
        public void TestAddition(int a, int b, int expected)
        {
            int actual = a + b;
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
