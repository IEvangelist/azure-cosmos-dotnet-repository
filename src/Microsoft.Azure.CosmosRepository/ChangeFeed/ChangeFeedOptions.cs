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

    internal bool IsTheSameAs(ChangeFeedOptions? options) =>
        options?.InstanceName == InstanceName &&
        options?.PollInterval == PollInterval &&
        options?.ProcessorName == ProcessorName &&
        options?.StartTime == StartTime;
}
