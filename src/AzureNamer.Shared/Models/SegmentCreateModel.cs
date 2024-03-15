using System;
using System.Collections.Generic;

using AzureNamer.Shared.Definitions;

using Generator.Equals;

namespace AzureNamer.Shared.Models;

[Equatable]
public partial class SegmentCreateModel
    : EntityCreateModel, IHaveOrganization
{
    #region Generated Properties
    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public int MinimumCharacters { get; set; }

    public int MaximumCharacters { get; set; }

    public bool HasDelimterBefore { get; set; }

    public bool HasDelimterAfter { get; set; }

    public bool IsOptional { get; set; }

    public bool IsLocked { get; set; }

    public bool IsEnabled { get; set; }

    public int SegmentTypeId { get; set; }

    public int OrganizationId { get; set; }

    #endregion

}
