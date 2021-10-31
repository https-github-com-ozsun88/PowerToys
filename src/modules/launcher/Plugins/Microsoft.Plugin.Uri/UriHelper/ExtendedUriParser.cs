// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.Plugin.Uri.Interfaces;

namespace Microsoft.Plugin.Uri.UriHelper
{
    public class ExtendedUriParser : IUriParser
    {
        public bool TryParse(string input, out System.Uri result)
        {
            if (string.IsNullOrEmpty(input))
            {
                result = default;
                return false;
            }

            // Handling URL with only scheme, typically mailto or application uri.
            // Do nothing, return the result without urlBuilder
            if ((input.EndsWith(":", StringComparison.OrdinalIgnoreCase) || input.EndsWith(":/", StringComparison.OrdinalIgnoreCase)) &&
                !input.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                !input.All(char.IsDigit))
            {
                result = new System.Uri(input);
                return true;
            }

            // Handle common cases UriBuilder does not handle
            // Using CurrentCulture since this is a user typed string
            if (input.EndsWith(":", StringComparison.CurrentCulture)
                || input.EndsWith(".", StringComparison.CurrentCulture)
                || input.EndsWith(":/", StringComparison.CurrentCulture)
                || input.All(char.IsDigit))
            {
                result = default;
                return false;
            }

            try
            {
                var urlBuilder = new UriBuilder(input);
                var hadDefaultPort = urlBuilder.Uri.IsDefaultPort;
                urlBuilder.Port = hadDefaultPort ? -1 : urlBuilder.Port;

                if (input.StartsWith("HTTP://", StringComparison.OrdinalIgnoreCase))
                {
                    urlBuilder.Scheme = System.Uri.UriSchemeHttp;
                }
                else if (input.Contains(":", StringComparison.OrdinalIgnoreCase) &&
                        !input.StartsWith("http", StringComparison.OrdinalIgnoreCase) &&
                        !input.Contains("[", StringComparison.OrdinalIgnoreCase))
                {
                    // Do nothing, leave unchanged
                }
                else
                {
                    urlBuilder.Scheme = System.Uri.UriSchemeHttps;
                }

                result = urlBuilder.Uri;
                return true;
            }
            catch (UriFormatException)
            {
                result = default;
                return false;
            }
        }
    }
}
