# OpenAPI .NET SDK

[![Nuget version (Tinkoff Trading OpenApi)](https://img.shields.io/nuget/v/Tinkoff.Trading.OpenApi.svg?style=flat-square)](https://www.nuget.org/packages/Tinkoff.Trading.OpenApi/)

Данный проект представляет собой инструментарий на языке C# для работы с OpenAPI Тинькофф Инвестиции, который можно использовать для создания торговых роботов.

## Начало работы

### Nuget

SDK [доступен](https://www.nuget.org/packages/Tinkoff.Trading.OpenApi/) на nuget.org, для подключения добавьте в проект зависимость Tinkoff.Trading.OpenApi.

### Сборка

Для сборки вам потребуется совместимая с .netstandard 2.0 реализация .NET.
Перейдите в директорию проекта и выполните следующую команду:
```bash
dotnet build -c Release
```
Или с помощью docker
```bash
docker run --rm  -v "$PWD":/home/dotnet/project -w  /home/dotnet/project mcr.microsoft.com/dotnet/core/sdk:3.0 dotnet build -c Release
```
После успешной сборки в поддиректрии `bin/Release/netstandard2.0` появится файл `Tinkoff.Trading.OpenApi.dll`, который можно подключить к любому другому .NET-проекту.

### Где взять токен аутентификации?

В разделе инвестиций вашего  [личного кабинета tinkoff](https://www.tinkoff.ru/invest/) . Далее:

* Перейдите в настройки
* Проверьте, что функция “Подтверждение сделок кодом” отключена
* Выпустите токен для торговли на бирже и режима “песочницы” (sandbox)
* Скопируйте токен и сохраните, токен отображается только один раз, просмотреть его позже не получится, тем не менее вы можете выпускать неограниченное количество токенов

## Документация

Единственная зависимость проекта — Json.NET для работы с JSON.
Документацию непосредственно по OpenAPI можно найти по [ссылке](https://api-invest.tinkoff.ru/openapi/docs/).

### Быстрый старт

Для непосредственного взаимодействия с OpenAPI нужно создать подключение.

```csharp
using Tinkoff.Trading.OpenApi.Network;
...
// токен аутентификации
var token = "my.token";
// для работы в песочнице используйте GetSandboxConnection
var connection = ConnectionFactory.GetConnection(token);
var context = connection.Context;

// вся работа происходит асинхронно через объект контекста
var portfolio = await context.PortfolioAsync();
```

### У меня есть вопрос

[Основной репозиторий с документацией](https://github.com/TinkoffCreditSystems/invest-openapi/) — в нем вы можете задать вопрос в Issues и получать информацию о релизах в Releases.
Если возникают вопросы по данному SDK, нашёлся баг или есть предложения по улучшению, то можно задать его в Issues, либо писать на почту:
* Мельникову Никите ( [n.v.melnikov@tinkoff.ru](mailto:n.v.melnikov@tinkoff.ru) )
* Каменскому Никите ( [n.m.kamenskiy@tinkoff.ru](mailto:n.m.kamenskiy@tinkoff.ru) )

