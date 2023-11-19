// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using Aspire.Microsoft.Azure.CosmosRepository.Items;

namespace AspireSample.ApiService.Infrastructure;

public class WeatherForecastItem : Item
{
    public DateOnly Date { get; set; }

    public int TemperatureInCelsius { get; set; }

    public string? Summary { get; set; }
}