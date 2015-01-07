// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verifier.cs" company="Appccelerate">
//   Copyright (c) 2008-2015
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Appccelerate.CheckNoBindingRedirectsTask
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    public class Verifier
    {
        public IReadOnlyCollection<Violation> Verify(XDocument config, IReadOnlyCollection<string> excludePatterns)
        {
            XNamespace ns = "urn:schemas-microsoft-com:asm.v1";
            var bindingRedirects = config.Descendants(ns + "bindingRedirect").ToList();

            var violations = from bindingRedirect in bindingRedirects
                             let assemblyName = bindingRedirect.Parent.Element(ns + "assemblyIdentity").Attribute("name").Value
                             where
                    !excludePatterns
                        .Select(pattern => new Regex(pattern))
                        .Any(regex => regex.IsMatch(assemblyName))
                             select new Violation(assemblyName);

            return violations.ToList();
        }
    }
}