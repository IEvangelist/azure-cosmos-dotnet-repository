// Copyright (c) IEvangelist. All rights reserved.
// Licensed under the MIT License.

using CleanArchitecture.Domain;

namespace CleanArchitecture;

public interface IPersonRepository : IDisposable
{
    ValueTask<PersonEntity> GetPerson(string id);
    ValueTask<PersonEntity> UpdatePerson(PersonEntity person);
}