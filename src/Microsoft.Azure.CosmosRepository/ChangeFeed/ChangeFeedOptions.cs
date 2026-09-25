// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Azure.CosmosRepository.ChangeFeed;

/// <summary>
/// The options for monitoring the change feed.
/// </summary>
public class ChangeFeedOptions
{
    /// <summary>
    /// Gets the type for the item being monitored. However, this type will
    /// be self-assigned to the <see cref="ChangeFeedOptions"/> type when
    /// the default value options instance.
    /// </summary>
    internal Type ItemType { get; }

    internal ChangeFeedOptions(Type itemType)
    {
        ItemType = itemType;
    }

    /// <summary>
    /// The instance name of the processor.
    /// </summary>
    public string InstanceName { get; set; } = "default";

    /// <summary>
    /// The poll interval to query the change feed processor
    /// </summary>
    public TimeSpan? PollInterval { get; set; }

    /// <summary>
    /// The processor name provided to the change feed processor library.
    /// </summary>
    public string ProcessorName { get; set; } = "cosmos-repository-pattern-processor";

    private DateTime? _startTime;

    /// <summary>
    /// Sets the time (exclusive) to start looking for changes after.
    /// </summary>
    /// <remarks>
    /// This is only used when:
    /// (1) Lease store is not initialized and is ignored if a lease exists and has continuation token.
    /// (2) StartContinuation is not specified.
    /// If this is specified, StartFromBeginning is ignored.
    /// </remarks>
    public DateTime? StartTime
    {
        get => _startTime;
        set
        {
            if (value.HasValue && value.Value.Kind != DateTimeKind.Utc)
                throw new ArgumentOutOfRangeException(nameof(value),"StartTime must be Utc");
            _startTime = value;
        }
    }

    /// <summary>
    /// The interval on which instances scan the lease store for expired or unowned leases to
    /// acquire. When <see langword="null"/> the change feed processor SDK default is used.
    /// </summary>
    public TimeSpan? LeaseAcquireInterval { get; set; }

    /// <summary>
    /// The interval after which a lease is considered expired when it has not been renewed,
    /// allowing another instance to acquire it. When <see langword="null"/> the change feed
    /// processor SDK default is used.
    /// </summary>
    /// <remarks>
    /// Keep this comfortably larger than <see cref="LeaseRenewInterval"/>; if it is not, a live
    /// owner can lose its lease before renewing, causing changes to be processed more than once.
    /// A good rule of thumb is roughly three to four times the renew interval (for example a 30s
    /// renew with a 120s expiration), which mirrors the SDK defaults. The intervals are not
    /// validated against each other.
    /// </remarks>
    public TimeSpan? LeaseExpirationInterval { get; set; }

    /// <summary>
    /// The interval on which the current owner renews (heartbeats) its leases. A larger value
    /// results in fewer lease writes. When <see langword="null"/> the change feed processor SDK
    /// default is used.
    /// </summary>
    /// <remarks>
    /// If you raise this above the SDK default lease expiration, also raise
    /// <see cref="LeaseExpirationInterval"/> so leases do not expire before they are renewed.
    /// </remarks>
    public TimeSpan? LeaseRenewInterval { get; set; }

    internal bool IsTheSameAs(ChangeFeedOptions? options) =>
        options?.InstanceName == InstanceName &&
        options?.PollInterval == PollInterval &&
        options?.ProcessorName == ProcessorName &&
        options?.StartTime == StartTime &&
        options?.LeaseAcquireInterval == LeaseAcquireInterval &&
        options?.LeaseExpirationInterval == LeaseExpirationInterval &&
        options?.LeaseRenewInterval == LeaseRenewInterval;
}
