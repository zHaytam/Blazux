using System;
using System.Reflection;
using Xunit;

namespace Blazux.Core.UnitTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void BuildFieldSetter_ShouldThrow_WhenFieldIsReadonly()
        {
            var field = typeof(Entry).GetField("_otherId", BindingFlags.NonPublic | BindingFlags.Instance);

            var ex = Assert.Throws<Exception>(() => field.BuildFieldSetter<Entry>());
            Assert.Equal("Cannot build setter for a read only field.", ex.Message);
        }

        [Fact]
        public void BuildFieldSetter_ShouldReturnWorkingSetter()
        {
            var field = typeof(Entry).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);
            var setter = field.BuildFieldSetter<Entry>();

            var entry = new Entry();
            setter(entry, 1000);

            Assert.Equal(1000, entry.Id);
        }
    }

    internal class Entry
    {
        private int _id;
        private readonly int _otherId;

        public int Id => _id;
    }
}
