﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Models;

public partial class Value : IAggregateRoot
{
    public int Id { get; set; }

    public string Value1 { get; set; }

    public int? AttributeId { get; set; }

    public string Slug { get; set; }

    public string Meta { get; set; }

    public string Language { get; set; }

    public virtual Attribute Attribute { get; set; }
}