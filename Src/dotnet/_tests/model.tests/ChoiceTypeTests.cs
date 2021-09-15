using System.Collections.Generic;
using model.cql.hl7.org;
using Xunit;

namespace model.tests
{
    public class ChoiceTypeTests
    {
        [Fact]
        public void TestChoiceTypeIsCompatible()
        {
            var first = new ChoiceType(new List<DataType>{new SimpleType("Period"), new SimpleType("Interval"), new SimpleType("DateTime")});
            var second = new ChoiceType(new List<DataType> { new SimpleType("Period"), new SimpleType("DateTime") });
            Assert.True(first.IsCompatibleWith(second));
            Assert.True(second.IsCompatibleWith(first));
            Assert.True(first.IsSuperSetOf(second));
            Assert.False(first.IsSubSetOf(second));
            Assert.True(second.IsSubSetOf(first));
            Assert.False(second.IsSuperSetOf(first));
        }

        [Fact]
        public void TestChoiceTypeIsNotCompatible()
        {
            var first = new ChoiceType(new List<DataType> { new SimpleType("Period"), new SimpleType("Interval"), new SimpleType("DateTime") });
            var second = new ChoiceType(new List<DataType> { new SimpleType("Integer"), new SimpleType("String") });
            Assert.False(first.IsCompatibleWith(second));
            Assert.False(second.IsCompatibleWith(first));
            Assert.False(first.IsSubSetOf(second));
            Assert.False(first.IsSuperSetOf(second));
            
        }
    }
}
