// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository;

/// <summary>
/// This is the repository interface for any implementation of
/// <typeparamref name="TItem"/>, exposing asynchronous C.R.U.D. functionality.
/// It exposes both <see cref="IReadOnlyRepository{TItem}"/> and
/// <see cref="IWriteOnlyRepository{TItem}"/>, providing a fully functioning repository.
/// </summary>
/// <typeparam name="TItem">The <see cref="IItem"/> implementation class type.</typeparam>
/// <example>
/// With DI, use .ctor injection to require any implementation of <see cref="IItem"/>:
/// <code language="c#">
/// <![CDATA[
/// public class ConsumingService
/// {
///     readonly IRepository<SomePoco> _pocoRepository;
///
///     public ConsumingService(
///         IRepository<SomePoco> pocoRepository) =>
///         _pocoRepository = pocoRepository;
/// }
/// ]]>
/// </code>
/// </example>
public interface IRepository<TItem> :
    IReadOnlyRepository<TItem>,
    IWriteOnlyRepository<TItem>,
    IBatchRepository<TItem>
    where TItem : IItem
{
}