// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckNoBindingRedirectsTask.cs" company="Appccelerate">
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
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class CheckNoBindingRedirectsTask : Task
    {
        [Required]
        public bool TreatWarningsAsErrors { get; set; }

        [Required]
        public string ConfigFullPath { get; set; }

        public string ExcludePatterns { get; set; }

        public bool Verbose { get; set; }

        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.Low, "checking " + this.ConfigFullPath + " for binding redirects. Excluded are " + this.ExcludePatterns);

            XDocument config = XDocument.Load(this.ConfigFullPath);

            var excludePatterns =
                this.ExcludePatterns != null
                    ? this.ExcludePatterns.Split(',')
                    : Enumerable.Empty<string>();

            var verifier = new Verifier();

            IReadOnlyCollection<Violation> violations = verifier.Verify(
                config,
                excludePatterns.ToList());

            foreach (Violation violation in violations)
            {
                this.LogViolation(violation);
            }

            bool continueBuild = !(violations.Any() && this.TreatWarningsAsErrors);
            return continueBuild;
        }

        private void LogViolation(Violation violation)
        {
            string message = string.Concat(
                "Found unexpecte binding redirect for assembly `",
                violation.Assembly,
                "` in ",
                this.ConfigFullPath);

            if (this.TreatWarningsAsErrors)
            {
                this.Log.LogError(null, null, null, this.ConfigFullPath, 0, 0, 0, 0, message);
            }
            else
            {
                this.Log.LogWarning(null, null, null, this.ConfigFullPath, 0, 0, 0, 0, message);
            }
        }
    }
}