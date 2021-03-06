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

using Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets.Models;
using Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets.ProviderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.RecoveryServices.Backup.Cmdlets
{
    /// <summary>
    /// Get list of items
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "AzureRmRecoveryServicesBackupItem", 
        DefaultParameterSetName = GetItemsForContainerParamSet), OutputType(typeof(ItemBase),
            typeof(IList<ItemBase>))]
    public class GetAzureRmRecoveryServicesBackupItem : RecoveryServicesBackupCmdletBase
    {
        internal const string GetItemsForContainerParamSet = "GetItemsForContainer";
        internal const string GetItemsForVaultParamSet = "GetItemsForVault";

        [Parameter(Mandatory = true, Position = 1, HelpMessage = ParamHelpMsgs.Item.Container,
            ParameterSetName = GetItemsForContainerParamSet)]
        [ValidateNotNullOrEmpty]
        public ContainerBase Container { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = ParamHelpMsgs.Common.BackupManagementType,
            ParameterSetName = GetItemsForVaultParamSet)]
        [ValidateNotNullOrEmpty]
        public BackupManagementType BackupManagementType { get; set; }

        [Parameter(Mandatory = false, Position = 2, HelpMessage = ParamHelpMsgs.Item.AzureVMName)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = false, Position = 3, HelpMessage = ParamHelpMsgs.Item.ProtectionStatus)]
        [ValidateNotNullOrEmpty]
        public ItemProtectionStatus ProtectionStatus { get; set; }

        [Parameter(Mandatory = false, Position = 4, HelpMessage = ParamHelpMsgs.Item.Status)]
        [ValidateNotNullOrEmpty]
        public ItemProtectionState ProtectionState { get; set; }

        [Parameter(Mandatory = true, Position = 5, HelpMessage = ParamHelpMsgs.Common.WorkloadType)]
        [ValidateNotNullOrEmpty]
        public WorkloadType WorkloadType { get; set; }

        public override void ExecuteCmdlet()
        {
            ExecutionBlock(() =>
            {
                base.ExecuteCmdlet();

                PsBackupProviderManager providerManager = 
                    new PsBackupProviderManager(new Dictionary<System.Enum, object>()
                {
                    {ItemParams.Container, Container},
                    {ItemParams.BackupManagementType, BackupManagementType},
                    {ItemParams.AzureVMName, Name},
                    {ItemParams.ProtectionStatus, ProtectionStatus},
                    {ItemParams.ProtectionState, ProtectionState},
                    {ItemParams.WorkloadType, WorkloadType},
                }, ServiceClientAdapter);

                IPsBackupProvider psBackupProvider = null;

                if (this.ParameterSetName == GetItemsForVaultParamSet)
                {
                    psBackupProvider = 
                        providerManager.GetProviderInstance(WorkloadType, BackupManagementType);
                }
                else
                {
                    psBackupProvider = providerManager.GetProviderInstance(WorkloadType,
                    (Container as ManagementContext).BackupManagementType);
                }

                var itemModels = psBackupProvider.ListProtectedItems();

                WriteObject(itemModels, enumerateCollection: true);
            });
        }
    }
}
