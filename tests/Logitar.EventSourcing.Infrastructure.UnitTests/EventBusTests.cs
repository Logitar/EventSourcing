using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Logitar.EventSourcing.Infrastructure;

[Trait(Traits.Category, Categories.Unit)]
public class EventBusTests
{
  private readonly Mock<IEventHandler<UserGenderChanged>> _userGenderChangedHandler = new();
  private readonly Mock<IEventHandler<UserLocaleChanged>> _userLocaleChangedHandler1 = new();
  private readonly Mock<IEventHandler<UserLocaleChanged>> _userLocaleChangedHandler2 = new();

  public EventBusTests()
  {
  }

  [Fact(DisplayName = "PublishAsync: it should call all found event handlers.")]
  public async Task Given_Handlers_When_PublishAsync_Then_AllCalled()
  {
    IServiceProvider serviceProvider = new ServiceCollection()
      .AddSingleton(_userGenderChangedHandler.Object)
      .AddSingleton(_userLocaleChangedHandler1.Object)
      .AddSingleton(_userLocaleChangedHandler2.Object)
      .BuildServiceProvider();
    EventBus eventBus = new(serviceProvider);

    UserLocaleChanged changed = new(CultureInfo.GetCultureInfo("fr-CA"));
    CancellationToken cancellationToken = default;

    await eventBus.PublishAsync(changed, cancellationToken);

    _userLocaleChangedHandler1.Verify(x => x.HandleAsync(changed, cancellationToken), Times.Once);
    _userLocaleChangedHandler2.Verify(x => x.HandleAsync(changed, cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "PublishAsync: it should not call any handler when there are none.")]
  public async Task Given_NoHandler_When_PublishAsync_Then_NoneCalled()
  {
    IServiceProvider serviceProvider = new ServiceCollection()
      .AddSingleton(_userGenderChangedHandler.Object)
      .BuildServiceProvider();
    EventBus eventBus = new(serviceProvider);

    UserLocaleChanged changed = new(CultureInfo.GetCultureInfo("fr-CA"));
    CancellationToken cancellationToken = default;

    await eventBus.PublishAsync(changed, cancellationToken);

    _userGenderChangedHandler.Verify(x => x.HandleAsync(It.IsAny<UserGenderChanged>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}
