---
title: "TimeStampedItem"
weight: 3
chapter: true
pre: "<b>2. </b>"
---

## Time Stamped Item

The last updated value is retrieved from the \_ts property that Cosmos DB sets; as documented [here](https://docs.microsoft.com/en-us/rest/api/cosmos-db/documents#:~:text=the%20document%20resource.-,_ts,updated%20timestamp%20of%20the%20resource.%20The%20value%20is%20a%20timestamp.,-_self). This property is deserialised and is available in the raw seconds (`LastUpdatedTimeRaw`) since epoch and a human readable format (`LastUpdatedTimeUtc`). Both the base classes `FullItem` and `TimeStampedItem` contain these properties.

The `CreatedTimeUtc` time property available in both the base classes `FullItem` and `TimeStampedItem` is set when `CreateAsync` is called on the repository. However, this property can be set prior to calling `CreateAsync` in which case it wont be overwritten; allowing you to set your own `CreatedTimeUtc` value. This does mean that when using existing date the `CreatedTimeUtc` property will be null.