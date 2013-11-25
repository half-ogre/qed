using System;
using System.Linq;
using Xunit;
using fn = qed.Functions;

#if DEBUG
namespace qed.UnitTests
{
    public class the_ParseForm_function
    {
        [Fact]
        public void parses_a_single_key_and_value()
        {
            const string formText = "key=value";

            var form = fn.ParseForm(formText);

            Assert.Equal(1, form.Count);
            Assert.Equal("key", form.Keys.ElementAt(0));
            Assert.Equal("value", form.Values.ElementAt(0).ElementAt(0));
        }

        [Fact]
        public void parses_a_key_with_multiple_values()
        {
            const string formText = "key=value1&key=value2";

            var form = fn.ParseForm(formText);

            Assert.Equal(1, form.Count);
            Assert.Equal("key", form.Keys.ElementAt(0));
            Assert.Equal("value1", form.Values.ElementAt(0).ElementAt(0));
            Assert.Equal("value2", form.Values.ElementAt(0).ElementAt(1));
        }

        [Fact]
        public void parses_form_text_with_multuple_keys_and_values()
        {
            const string formText = "key1=value1a&key1=value1b&key2=value2";

            var form = fn.ParseForm(formText);

            Assert.Equal(2, form.Count);
            Assert.Equal("key1", form.Keys.ElementAt(0));
            Assert.Equal("key2", form.Keys.ElementAt(1));
            Assert.Equal("value1a", form.Values.ElementAt(0).ElementAt(0));
            Assert.Equal("value1b", form.Values.ElementAt(0).ElementAt(1));
            Assert.Equal("value2", form.Values.ElementAt(1).ElementAt(0));
        }

        [Fact]
        public void decodes_keys()
        {
            string formText = String.Format("{0}=value", Uri.EscapeDataString("&key"));

            var form = fn.ParseForm(formText);

            Assert.Equal(1, form.Count);
            Assert.Equal("&key", form.Keys.ElementAt(0));
        }

        [Fact]
        public void decodes_keys_with_spaces()
        {
            const string formText = "key+one=value";

            var form = fn.ParseForm(formText);

            Assert.Equal(1, form.Count);
            Assert.Equal("key one", form.Keys.ElementAt(0));
        }

        [Fact]
        public void decodes_values()
        {
            var formText = String.Format("key={0}", Uri.EscapeDataString("&value"));

            var form = fn.ParseForm(formText);

            Assert.Equal(1, form.Count);
            Assert.Equal("&value", form.Values.ElementAt(0).ElementAt(0));
        }

        [Fact]
        public void decodes_values_with_spaces()
        {
            const string formText = "key=value+one";

            var form = fn.ParseForm(formText);

            Assert.Equal(1, form.Count);
            Assert.Equal("value one", form.Values.ElementAt(0).ElementAt(0));
        }
    }
}
#endif
