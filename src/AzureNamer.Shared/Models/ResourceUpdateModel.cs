using System;
using System.Collections.Generic;

using AzureNamer.Shared.Definitions;

using Generator.Equals;

namespace AzureNamer.Shared.Models;

[Equatable]
public partial class ResourceUpdateModel
    : EntityUpdateModel, IHaveOrganization
{
    #region Generated Properties
    public Guid Identifier { get; set; }

    public string? Name { get; set; }

    public string Namespace { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public string? ValidationDescription { get; set; }

    public int? MinimumCharacters { get; set; }

    public int? MaximumCharacters { get; set; }

    public bool AllowDelimter { get; set; }

    public string? ValidationExpression { get; set; }

    public int OrganizationId { get; set; }

    #endregion

}
