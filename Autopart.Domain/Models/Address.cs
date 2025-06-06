﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Models;

public partial class Address:IAggregateRoot
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Type { get; set; }

    public short? IsDefault { get; set; }

    public int? ShopId { get; set; }

    public string Zip { get; set; }

    public string City { get; set; }

    public string State { get; set; }

    public string Country { get; set; }

    public string StreetAddress { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ShippingId { get; set; }

    public virtual Shipping Shipping { get; set; }

    public virtual Shop Shop { get; set; }

    public virtual AspNetUser User { get; set; }
}