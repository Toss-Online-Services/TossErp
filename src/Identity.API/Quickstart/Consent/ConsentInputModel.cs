// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace IdentityServerHost.Quickstart.UI;

public class ConsentInputModel
{
    public required string Button { get; set; }
    public required IEnumerable<string> ScopesConsented { get; set; }
    public bool RememberConsent { get; set; }
    public required string ReturnUrl { get; set; }
    public required string Description { get; set; }
}
