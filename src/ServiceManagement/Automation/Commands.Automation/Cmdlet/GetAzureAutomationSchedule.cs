﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Permissions;
using Microsoft.Azure.Commands.Automation.Common;
using Microsoft.Azure.Commands.Automation.Model;

namespace Microsoft.Azure.Commands.Automation.Cmdlet
{
    /// <summary>
    /// Gets azure automation schedules for a given account.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureAutomationSchedule", DefaultParameterSetName = AutomationCmdletParameterSets.ByAll)]
    [OutputType(typeof(Schedule))]
    public class GetAzureAutomationSchedule : AzureAutomationBaseCmdlet
    {
        /// <summary>
        /// Gets or sets the schedule name.
        /// </summary>
        [Parameter(ParameterSetName = AutomationCmdletParameterSets.ByName, Position = 1, Mandatory = true, ValueFromPipelineByPropertyName = true,
            HelpMessage = "The schedule name.")]
        [Alias("ScheduleName")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// Execute this cmdlet.
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void AutomationExecuteCmdlet()
        {
            IEnumerable<Schedule> schedules = null;
            if (this.ParameterSetName == AutomationCmdletParameterSets.ByName)
            {
                schedules = new List<Schedule>
                                {
                                    this.AutomationClient.GetSchedule(
                                        this.AutomationAccountName, this.Name)
                                };
            }
            else if (this.ParameterSetName == AutomationCmdletParameterSets.ByAll)
            {
                schedules = this.AutomationClient.ListSchedules(this.AutomationAccountName);
            }

            this.GenerateCmdletOutput(schedules);
        }
    }
}
