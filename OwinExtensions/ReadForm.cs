﻿using System;
using System.Collections.Generic;
using System.IO;
using OwinExtensions;

namespace qed
{
    public static partial class Functions
    {
        internal static IDictionary<string, List<string>> ParseForm(string formText)
        {
            var form = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            var appendPair = new Action<string, string>((name, value) =>
            {
                List<string> existing;
                if (!form.TryGetValue(name, out existing))
                    form.Add(name, new List<string>(1) { value });
                else
                    existing.Add(value);
            });

            var length = formText.Length;
            var cursor = 0;

            var equalSignIndex = formText.IndexOf('=');
            if (equalSignIndex == -1)
                equalSignIndex = length;

            while (cursor < length)
            {
                var delimiterIndex = formText.IndexOf('&', cursor);
                if (delimiterIndex == -1)
                    delimiterIndex = length;

                if (equalSignIndex < delimiterIndex)
                {
                    while (cursor != equalSignIndex && char.IsWhiteSpace(formText[cursor]))
                        ++cursor;

                    var name = formText.Substring(cursor, equalSignIndex - cursor);
                    var value = formText.Substring(equalSignIndex + 1, delimiterIndex - equalSignIndex - 1);

                    appendPair(
                        Uri.UnescapeDataString(name.Replace('+', ' ')),
                        Uri.UnescapeDataString(value.Replace('+', ' ')));

                    equalSignIndex = formText.IndexOf('=', delimiterIndex);
                    if (equalSignIndex == -1)
                        equalSignIndex = length;
                }

                cursor = delimiterIndex + 1;
            }

            return form;
        }

        public static IDictionary<string, List<string>> ReadFormAsync(this IDictionary<string, object> environment)
        {
            var form = environment.Get<IDictionary<string, List<string>>>(Constants.OwinExtensions.RequestFormKey);

            if (form != null)
                return form;

            string formText;
            var stream = environment.GetRequestBody();
            using (var streamReader = new StreamReader(stream))
                formText = streamReader.ReadToEnd();

            form = ParseForm(formText);

            environment[Constants.OwinExtensions.RequestFormKey] = form;

            return form;
        }
    }
}