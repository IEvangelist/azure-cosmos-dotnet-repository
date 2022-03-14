# Sample Applications

Our repository has a lot of sample applications that you can look at for inspiration. These cover a few different domains which help demonstrate the application of this library in an event sourcing architecture. 

## :ship: Ship Tracking

This application covers the example of an application that tracks a ships movements. The aggregate root in this case is the [`Ship`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/samples/Microsoft.Azure.CosmosEventSourcing/BasicEventSourcingSample/Core/Ship.Definition.cs) this allows a new ship to be created, it can also be docked into ports, loaded and then depart, each of the events is stored and used to build a [`ShipInformation`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/samples/Microsoft.Azure.CosmosEventSourcing/BasicEventSourcingSample/Projections/Models/ShipInformation.cs) projection.

See the [full shipping example here.](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/samples/Microsoft.Azure.CosmosEventSourcing/BasicEventSourcingSample)

## :pencil: Job Tracking

This application is a very simple one allows a user to create a [`JobsList`](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/blob/main/samples/Microsoft.Azure.CosmosEventSourcing/EventSourcingJobsTracker/Core/Aggregates/JobList.cs), this allows for a user to add jobs to the list and complete the jobs on that list. It is very much in line with the common Todo app samples you may find elsewhere. This also makes use of projections to build read models for jobs and jobs lists.

See the [full job tracking example here.](https://github.com/IEvangelist/azure-cosmos-dotnet-repository/tree/main/samples/Microsoft.Azure.CosmosEventSourcing/EventSourcingJobsTracker)

## :truck: Delivery Tracker

This application is a fully fledged example, it follows a clean architecture layout while also applying event sourcing using this library. It defines a [`Schedule`](https://github.com/mumby0168/cosmos-event-sourcing-delivery-tracker/blob/main/src/DeliveryTracker.Domain/Aggregates/Schedule.Apply.cs) that is created for a driver on a given day they then get stops assigned to the schedule and they complete stops as they deliver parcels. They also have the opportunity to fail deliveries. They can also abandon a delivery at the end of a day.

See the [full delivery tracking example here.](https://github.com/mumby0168/cosmos-event-sourcing-delivery-tracker)