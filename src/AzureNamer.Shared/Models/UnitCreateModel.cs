using System;
using System.Collections.Generic;

using AzureNamer.Shared.Definitions;

using Generator.Equals;

namespace AzureNamer.Shared.Models;

[Equatable]
public partial class UnitCreateModel
    : EntityCreateModel, IHaveOrganization
{
    #region Generated Properties
    public string Name { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public int OrganizationId { get; set; }

    #endregion

}
