using System;
using System.Collections.Generic;

using Generator.Equals;

namespace AzureNamer.Shared.Models;

[Equatable]
public partial class ProjectUpdateModel
    : EntityUpdateModel
{
    #region Generated Properties
    public string Name { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public int OrganizationId { get; set; }

    #endregion

}
