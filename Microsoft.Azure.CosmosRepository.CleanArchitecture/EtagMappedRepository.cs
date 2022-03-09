// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using System.Collections;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Paging;
using Microsoft.Azure.CosmosRepository.Specification;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

internal class EtagMappedRepository<TItem, TMapped>
    : IEtagMappedRepository<TItem, TMapped>
    where TItem : IItemWithEtag
{
    private readonly IRepository<TItem> _itemRepository;
    private readonly IMapper<TItem, TMapped> _mapper;
    private readonly IEtagCache _etagCache;
    private IDisposable? _optionsMonitor;

    public EtagMappedRepository(
        IRepository<TItem> itemRepository,
        IMapper<TItem, TMapped> mapper,
        IEtagCache etagCache,
        IOptionsMonitor<RepositoryOptions> repositoryOptions)
    {
        _itemRepository = itemRepository;
        _mapper = mapper;
        _etagCache = etagCache;

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

    public void Dispose()
    {
        _optionsMonitor?.Dispose();
        _optionsMonitor = null;
    }

    public async ValueTask<TMapped?> TryGetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
    {
        TItem? item = await _itemRepository.TryGetAsync(id, partitionKeyValue, cancellationToken);

        if (item is null)
        {
            return default;
        }

        _etagCache.StoreEtag(item);

        return await _mapper.MapAsync(item);
    }

    public async ValueTask<TMapped> GetAsync(string id, string? partitionKeyValue, CancellationToken cancellationToken)
    {
        TItem item = await _itemRepository.GetAsync(id, partitionKeyValue, cancellationToken);

        _etagCache.StoreEtag(item);

        return await _mapper.MapAsync(item);
    }

    public async ValueTask<TMapped> GetAsync(string id, PartitionKey partitionKey, CancellationToken cancellationToken)
    {
        TItem item = await _itemRepository.GetAsync(id, partitionKey, cancellationToken);

        _etagCache.StoreEtag(item);

        return await _mapper.MapAsync(item);
    }

    public async ValueTask<IEnumerable<TMapped>> GetAsync(Expression<Func<TItem, bool>> predicate, CancellationToken cancellationToken = default)
    {
        IEnumerable<TItem> items = await _itemRepository.GetAsync(predicate, cancellationToken);

        IEnumerable<Task<TMapped>> mappedItemsTasks = items.Select(item =>
        {
            _etagCache.StoreEtag(item);
            return _mapper.MapAsync(item).AsTask();
        });

        return await Task.WhenAll(mappedItemsTasks);
    }
}