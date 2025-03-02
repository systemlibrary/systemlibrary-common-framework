using System;
using System.Collections.Generic;

namespace SystemLibrary.Common.Framework.App.Tests;

public interface IVehicle
{
    string Name { get; set; }
}
public class Car : IVehicle
{
    public string Name { get; set; }
    public string[] Names { get; set; }
    public List<string> LastNames { get; set; }
    public int Age { get; set; }
    public int[] Ages { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime Born { get; set; }
    public DateTimeOffset Death { get; set; }
    public TimeSpan Expiration { get; set; }

    public int? NullableAge { get; set; }
    public DateTime? DateTimeNull { get; set; }
    public DateTimeOffset? DateTimeOffsetNull { get; set; }
    public TimeSpan? TimeSpanNull { get; set; }
    public bool? IsEnabledNull { get; set; }

    public IVehicle Vehicle { get; set; }
    public List<IVehicle> Interfaces { get; set; }

    public CarOwner Owner { get; set; }
}

public class CarOwner
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
