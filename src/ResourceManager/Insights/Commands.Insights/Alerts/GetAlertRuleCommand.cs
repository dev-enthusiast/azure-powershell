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
using System.Linq;
using System.Management.Automation;
using Microsoft.Azure.Commands.Insights.OutputClasses;
using Microsoft.Azure.Management.Insights;
using Microsoft.Azure.Management.Insights.Models;

namespace Microsoft.Azure.Commands.Insights.Alerts
{
    /// <summary>
    /// Get an Alert rule
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AlertRule"), OutputType(typeof(List<PSManagementItemDescriptor>))]
    public class GetAlertRuleCommand : ManagementCmdletBase
    {
        internal const string GetAlertParamGroup = "Parameters for GetAlert cmdlet";
        internal const string GetAlertWithNameParamGroup = "Parameters for GetAlert cmdlet using name";
        internal const string GetAlertWithUriParamGroup = "Parameters for GetAlert cmdlet using target resource uri";

        #region Cmdlet parameters

        /// <summary>
        /// Gets or sets the ResourceGroupName parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAlertParamGroup, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The ResourceGroup name")]
        [Parameter(ParameterSetName = GetAlertWithNameParamGroup, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The ResourceGroup name")]
        [Parameter(ParameterSetName = GetAlertWithUriParamGroup, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The ResourceGroup name")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Gets or sets the rule name parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAlertWithNameParamGroup, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The alert rule name")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the TargetResourceUri parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAlertWithUriParamGroup, Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The alert rule target resource uri")]
        [ValidateNotNullOrEmpty]
        public string TargetResourceUri { get; set; }

        /// <summary>
        /// Gets or sets the detailedoutput parameter of the cmdlet
        /// </summary>
        [Parameter(ParameterSetName = GetAlertParamGroup, ValueFromPipelineByPropertyName = true, HelpMessage = "Return object with all the details of the records (the default is to return only some attributes, i.e. no detail)")]
        [Parameter(ParameterSetName = GetAlertWithNameParamGroup, ValueFromPipelineByPropertyName = true, HelpMessage = "Return object with all the details of the records (the default is to return only some attributes, i.e. no detail)")]
        [Parameter(ParameterSetName = GetAlertWithUriParamGroup, ValueFromPipelineByPropertyName = true, HelpMessage = "Return object with all the details of the records (the default is to return only some attributes, i.e. no detail)")]
        public SwitchParameter DetailedOutput { get; set; }

        #endregion

        /// <summary>
        /// Execute the cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.Name))
                {
                    // Retrieve all the AlertRules for a ResourceGroup
                    RuleListResponse result = this.InsightsManagementClient.AlertOperations.ListRulesAsync(resourceGroupName: this.ResourceGroup, targetResourceUri: this.TargetResourceUri).Result;

                    var records = result.RuleResourceCollection.Value.Select(e => this.DetailedOutput.IsPresent ? (PSManagementItemDescriptor)new PSAlertRule(e) : new PSAlertRuleNoDetails(e));
                    WriteObject(sendToPipeline: records, enumerateCollection: true);
                }
                else
                {
                    // Retrieve a single AlertRule determined by the ResourceGroup and the rule name
                    RuleGetResponse result = this.InsightsManagementClient.AlertOperations.GetRuleAsync(resourceGroupName: this.ResourceGroup, ruleName: this.Name).Result;
                    WriteObject(sendToPipeline: this.DetailedOutput.IsPresent ? (PSManagementItemDescriptor)new PSAlertRule(result) : new PSAlertRuleNoDetails(result));
                }
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }
    }
}
