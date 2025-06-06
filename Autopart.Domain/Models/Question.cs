﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Models;

public partial class Question : IAggregateRoot
{
    public int Id { get; set; }

    public string Question1 { get; set; }

    public string Answer { get; set; }

    public int? UserId { get; set; }

    public int? ShopId { get; set; }

    public int? ProductId { get; set; }

    public int? PositiveFeedbacksCount { get; set; }

    public int? NegativeFeedbacksCount { get; set; }

    public string MyFeedback { get; set; }

    public int? AbusiveReportsCount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product Product { get; set; }

    public virtual Shop Shop { get; set; }

    public virtual AspNetUser User { get; set; }
}