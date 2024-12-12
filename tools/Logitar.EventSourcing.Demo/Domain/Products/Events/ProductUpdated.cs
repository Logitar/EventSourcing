using MediatR;
using System.Text.Json.Serialization;

namespace Logitar.EventSourcing.Demo.Domain.Products.Events;

public record ProductUpdated : DomainEvent, INotification
{
  public Sku? Sku { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<Price>? Price { get; set; }
  public Change<Url>? PictureUrl { get; set; }

  [JsonIgnore]
  public bool HasChanges => Sku != null || DisplayName != null || Description != null || Price != null || PictureUrl != null;
}
