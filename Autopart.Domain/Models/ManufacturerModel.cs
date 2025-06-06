﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Models;

public partial class ManufacturerModel : IAggregateRoot
{
    public int Id { get; set; }

    public int? ManufacturerId { get; set; }

    public string Model { get; set; }

    public string Slug { get; set; }

    public virtual ICollection<Engine> Engines { get; set; } = new List<Engine>();

    public virtual Manufacture Manufacturer { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}