using Aquiis.Core.Entities;
using Aquiis.UI.Shared.Components.Entities.OrganizationUsers;

namespace Aquiis.UI.Shared.Components.Entities.Organizations;

public class OrganizationViewModel
{
    public Organization? Organization { get; set; }
    public OrganizationUserViewModel? OrganizationOwner { get; set; }
    public List<OrganizationUserViewModel> OrganizationUsers { get; set; } = new();
}