// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerifierFacts.cs" company="Appccelerate">
//   Copyright (c) 2008-2014
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
    using System.Xml.Linq;
    using FluentAssertions;
    using Xunit;
    using Xunit.Extensions;

    public class VerifierFacts
    {
        private static readonly List<string> EmptyExcludePatterns = new List<string>();

        private readonly Verifier testee;

        public VerifierFacts()
        {
            this.testee = new Verifier();
        }

        [Fact]
        public void ReturnsNoViolation_WhenConfigFileDoesNotContainAbindingRedirect()
        {
            IReadOnlyCollection<Violation> result = this.testee.Verify(
                ValidConfig,
                EmptyExcludePatterns);

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Castle.Core", "Castle.Core")]
        [InlineData("Castle.Core", ".*Core")]
        public void ReturnsNoViolation_WhenBindingRedirectIsExcluded(string assemblyName, string excludePattern)
        {
            IReadOnlyCollection<Violation> result = this.testee.Verify(
                ConfigWithBindingRedirect("Castle.Core"),
                new[] { "otherPattern", excludePattern, "yetAnotherPattern" });

            result.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsViolation_WhenConfigContainsBindingRedirect()
        {
            const string AssemblyName = "Castle.Core";

            IReadOnlyCollection<Violation> result = this.testee.Verify(
                ConfigWithBindingRedirect(AssemblyName),
                EmptyExcludePatterns);

            result.Should().BeEquivalentTo(new Violation(AssemblyName));
        }

        private static XDocument ValidConfig
        {
            get
            {
                return XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <configSections>
    <section name=""CleanUp"" type=""Appccelerate.Bootstrapper.Configuration.ExtensionConfigurationSection, Appccelerate.Bootstrapper"" />
  </configSections>
  
  <startup> 
    <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.5"" />
  </startup>

  <CleanUp>
    <Configuration>
      <add key=""CleanUpInterval"" value=""60"" />
      <add key=""CleanUpOlderThan"" value=""60"" />
    </Configuration>
  </CleanUp>
  
  <appSettings>
    <add key=""SomeKey"" value=""Somevalue"" />
  </appSettings>
</configuration>");
            }
        }
           
        private static XDocument ConfigWithBindingRedirect(string assemblyName)
        {
            return XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <configSections>
    <section name=""CleanUp"" type=""Appccelerate.Bootstrapper.Configuration.ExtensionConfigurationSection, Appccelerate.Bootstrapper"" />
  </configSections>
  
  <startup> 
    <supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.5"" />
  </startup>

  <CleanUp>
    <Configuration>
      <add key=""CleanUpInterval"" value=""60"" />
      <add key=""CleanUpOlderThan"" value=""60"" />
    </Configuration>
  </CleanUp>
  
  <appSettings>
    <add key=""SomeKey"" value=""Somevalue"" />
  </appSettings>
  <runtime>
    <assemblyBinding xmlns=""urn:schemas-microsoft-com:asm.v1"">
      <dependentAssembly>
        <assemblyIdentity name=""Castle.Core"" publicKeyToken=""407dd0808d44fbdc"" culture=""neutral"" />
        <bindingRedirect oldVersion=""0.0.0.0-3.1.0.0"" newVersion=""3.1.0.0"" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>");
        }
    }
}