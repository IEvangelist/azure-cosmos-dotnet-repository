// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;
using InMemoryWebTier.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CosmosRepository;

namespace InMemoryWebTier.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParcelController : ControllerBase
{
    private readonly IRepository<Parcel> _repository;

    public ParcelController(IRepository<Parcel> repository)
    {
        _repository = repository;
    }

    [HttpGet(Name = nameof(GetParcels))]
    public ValueTask<IEnumerable<Parcel>> GetParcels() =>
        _repository.GetAsync(p => p.Id != null);

    [HttpGet("{id}", Name = nameof(GetParcel))]
    public ValueTask<Parcel> GetParcel(string id) =>
        _repository.GetAsync(id);

    [HttpPost(Name = nameof(CreateParcels))]
    public ValueTask<IEnumerable<Parcel>> CreateParcels([FromBody] params Parcel[] parcels) =>
        _repository.CreateAsync(parcels);

    [HttpPut(Name = nameof(UpdateParcel))]
    public ValueTask<Parcel> UpdateParcel([FromBody] Parcel parcel) =>
        _repository.UpdateAsync(parcel);

    [HttpDelete("{id}", Name = nameof(DeleteParcel))]
    public ValueTask DeleteParcel(string id) =>
        _repository.DeleteAsync(id);
}