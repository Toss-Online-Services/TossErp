// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServerHost.Quickstart.UI;

public class GrantsViewModel
{
    public required IEnumerable<GrantViewModel> Grants { get; set; }
}

public class GrantViewModel
{
    public required string ClientId { get; set; }
    public required string ClientName { get; set; }
    public required string ClientUrl { get; set; }
    public required string ClientLogoUrl { get; set; }
    public required string Description { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Expires { get; set; }
    public required IEnumerable<string> IdentityGrantNames { get; set; }
    public required IEnumerable<string> ApiGrantNames { get; set; }
}
