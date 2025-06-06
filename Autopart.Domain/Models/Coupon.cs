﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Models;

public partial class Coupon : IAggregateRoot
{
    public int Id { get; set; }

    public string Code { get; set; }

    public string Language { get; set; }

    public decimal? Amount { get; set; }

    public decimal? MinimumCartAmount { get; set; }

    public List<string> TranslatedLanguages { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ShopId { get; set; }

    public bool IsActive { get; set; }

    public virtual Shop Shop { get; set; }
}