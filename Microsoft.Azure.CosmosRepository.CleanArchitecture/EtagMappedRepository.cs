// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public class EtagMappedRepository<TItem, TMapped>
    : IEtagMappedRepository<TItem, TMapped>
    where TItem : IItemWithEtag
    where TMapped : class
{
    private readonly IRepository<TItem> _itemRepository;
    private readonly IItemMapper<TItem, TMapped> _itemMapper;
    private readonly IMappedItemMapper<TMapped, TItem> _mappedItemMapper;
    private string? _etag;
    private IDisposable? _optionsMonitor;

    public string? Etag => _etag;

    public void ForceEtag(string etag) => _etag = etag;

    public EtagMappedRepository(
        IRepository<TItem> itemRepository,
        IItemMapper<TItem, TMapped> itemMapper,
        IMappedItemMapper<TMapped, TItem> mappedItemMapper,
        IOptionsMonitor<RepositoryOptions> repositoryOptions)
    {
        _itemRepository = itemRepository;
        _itemMapper = itemMapper;
        _mappedItemMapper = mappedItemMapper;

        _optionsMonitor = repositoryOptions.OnChange(EnsureOptimizeBandwidthIsOff);
        EnsureOptimizeBandwidthIsOff(repositoryOptions.CurrentValue);
    }

    private void EnsureOptimizeBandwidthIsOff(RepositoryOptions options)
    {
        if (options.OptimizeBandwidth)
        {
            throw new InvalidOperationException(
                $"Cannot use {nameof(EtagMappedRepository<TItem, TMapped>)} when {nameof(options.OptimizeBandwidth)} is {options.OptimizeBandwidth}");
        }
    }

    public async ValueTask<TMapped> GetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
    {
        TItem item = await _itemRepository.GetAsync(id, partitionKeyValue, cancellationToken);

        _etag = item.Etag;

        return await _itemMapper.MapAsync(item);
    }

    public async ValueTask UpdateAsync(TMapped value, CancellationToken cancellationToken = default)
    {
        TItem item = await _mappedItemMapper.MapAsync(value);

        item.Etag = _etag;

        TItem updatedItem = await _itemRepository.UpdateAsync(item, cancellationToken, false);

        _etag = updatedItem.Etag;
    }

    public async ValueTask UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, string? partitionKeyValue = null,
        CancellationToken cancellationToken = default, string? etag = default)
    {
        await _itemRepository.UpdateAsync(id, builder, partitionKeyValue, cancellationToken, _etag);

        TItem updatedItem = await _itemRepository.GetAsync(id, partitionKeyValue, cancellationToken);

        _etag = updatedItem.Etag;
    }

    public void Dispose()
    {
        _optionsMonitor?.Dispose();
        _optionsMonitor = null;
        _etag = null;
    }
}