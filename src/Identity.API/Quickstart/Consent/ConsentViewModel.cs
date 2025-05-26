// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServerHost.Quickstart.UI;

public class ConsentViewModel : ConsentInputModel
{
    public required string ClientName { get; set; }
    public required string ClientUrl { get; set; }
    public required string ClientLogoUrl { get; set; }
    public bool AllowRememberConsent { get; set; }

    public required IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
    public required IEnumerable<ScopeViewModel> ApiScopes { get; set; }
}
