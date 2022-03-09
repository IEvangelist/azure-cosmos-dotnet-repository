// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Azure.CosmosRepository.Builders;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.CosmosRepository.CleanArchitecture;

public class EtagMappedRepository<TItem, TMapped>
    : IEtagMappedRepository<TItem, TMapped>
    where TItem : IItemWithEtag
{
    private readonly IRepository<TItem> _itemRepository;
    private readonly IMapper<TItem, TMapped> _mapper;
    private readonly IEtagCache _etagCache;
    private IDisposable? _optionsMonitor;

    public string? GetEtag(string id) => _etagCache.GetEtag(id);

    public void ForceEtag(string id, string etag) => _etagCache.SetEtag(id, etag);

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

    public async ValueTask<TMapped> GetAsync(string id, string? partitionKeyValue = null, CancellationToken cancellationToken = default)
    {
        TItem item = await _itemRepository.GetAsync(id, partitionKeyValue, cancellationToken);

        _etagCache.SetEtag(item.Id, item.Etag);

        return await _mapper.MapAsync(item);
    }

    public async ValueTask<TMapped> UpdateAsync(TMapped value, CancellationToken cancellationToken = default)
    {
        TItem item = await _mapper.MapAsync(value);

        item.Etag = _etagCache.GetEtag(item.Id);

        TItem updatedItem = await _itemRepository.UpdateAsync(item, cancellationToken, false);

        _etagCache.SetEtag(updatedItem.Id, updatedItem.Etag);

        return await _mapper.MapAsync(updatedItem);
    }

    public async ValueTask<IEnumerable<TMapped>> UpdateAsync(IEnumerable<TMapped> values, CancellationToken cancellationToken = default)
    {
        List<Task<TMapped>> updateTasks =
            values.Select(value =>
                UpdateAsync(value, cancellationToken)
                    .AsTask())
                    .ToList();

            await Task.WhenAll(updateTasks).ConfigureAwait(false);

            return updateTasks.Select(x => x.Result);
        }

    public async ValueTask<TMapped> UpdateAsync(string id, Action<IPatchOperationBuilder<TItem>> builder, string? partitionKeyValue = null,
        CancellationToken cancellationToken = default, string? etag = default)
    {
        await _itemRepository.UpdateAsync(id, builder, partitionKeyValue, cancellationToken, _etagCache.GetEtag(id));

        TItem updatedItem = await _itemRepository.GetAsync(id, partitionKeyValue, cancellationToken);

        _etagCache.SetEtag(updatedItem.Id, updatedItem.Etag);

        return await _mapper.MapAsync(updatedItem);
    }

    public void Dispose()
    {
        _optionsMonitor?.Dispose();
        _optionsMonitor = null;
    }
}