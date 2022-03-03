---
title: "Major Release Notes"
weight: 6
chapter: true
pre: "<b>6. </b>"
---

# Release 3.0.0

## Nullable Reference Types

The most significant change to the library is the support for nullable reference types. The API surface has been updated to fully support null reference types. This may introduce warnings to code that previously may not have had to be null aware. This will introduce compile errors in projects that have `<TreatWarningsAsErrors>True</TreatWarningsAsErrors>` in a projects `.csproj` file.

## GetAsync method. 

The `_repository.GetAsync(id, partitionKey)` method was used to previously have a scenario where the API would return `null`.

This case was when the library read an item from cosmos successfully but the `type` field did not match that of the name of the class used. i.e. take the example below.

```json
{
    "partitionKey": "BookCategory",
    "_etag": "\"3100b48c-0000-0700-0000-6219f58b0000\"",
    "timeToLive": null,
    "createdTimeUtc": "2022-02-26T09:40:27.000061Z",
    "id": "Engineering",
    "type": "BookCategory", // <--------- TYPE Field
    "_rid": "vG1RAMQtoX4LAAAAAAAAAA==",
    "_self": "dbs/vG1RAA==/colls/vG1RAMQtoX4=/docs/vG1RAMQtoX4LAAAAAAAAAA==/",
    "_attachments": "attachments/",
    "_ts": 1645868427
}
```

Notice the type field's value is `BookCategory` if I was to use a repository let's say with a class called `Category` the repository that you'd use via DI would look like `IRepository<Category>` when you made a read for this item the value of the `type` field from the above JSON does not match the name of the class. This would not previously result in an exception but return null to the caller.

📣  The library now in this case throws an exception instead of returning `null`. The exception that will be thrown in the `MissMatchedTypeDiscriminatorException` code the previously checked for `null` will now have to catch this exception. 

## Opt-in to receive total item counts when paging.

Previously when using either of the paging operations provided by this library it would run a count query for every page request. This, however, can incur a large RU charge for large data sets. For this reason, we have chosen to make this now an opt-in feature. When calling either of the paging methods you will need to pass a `bool` to indicate you would like the count query to be run. See an example below.

```csharp
_dogRepository.PageAsync(
    d => d.Breed == "cocker spaniel", 
    pageNumber, 
    pageSize,
    returnTotal: true);
```

> this `returnTotal` field defaults to `false`.

## ETagItem is now abstract

This is a small change that makes the `ETagItem` class abstract.
