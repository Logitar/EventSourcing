﻿namespace Logitar.Identity.Core.ApiKeys.Payloads;

public record CreateApiKeyPayload
{
  public string? TenantId { get; set; }

  public string Title { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}