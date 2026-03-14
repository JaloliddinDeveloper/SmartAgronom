using AqlliAgronom.Domain.Common;
using AqlliAgronom.Domain.Enums;
using AqlliAgronom.Domain.Events.Orders;
using AqlliAgronom.Domain.ValueObjects;

namespace AqlliAgronom.Domain.Entities;

public class Order : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string FarmerName { get; private set; } = default!;
    public PhoneNumber FarmerPhone { get; private set; } = default!;
    public string? CropDescription { get; private set; }
    public string? ProblemDescription { get; private set; }
    public string Region { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = default!;
    public string? Notes { get; private set; }
    public Guid? AssignedAgronomiystId { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }

    public static Order Place(
        Guid userId, string farmerName, string phone, string region,
        string? cropDescription = null, string? problemDescription = null,
        string? notes = null)
    {
        var order = new Order
        {
            UserId = userId,
            FarmerName = farmerName,
            FarmerPhone = PhoneNumber.Create(phone),
            Region = region,
            CropDescription = cropDescription,
            ProblemDescription = problemDescription,
            Notes = notes,
            Status = OrderStatus.Pending,
            TotalAmount = Money.Zero()
        };

        order.AddDomainEvent(new OrderPlacedEvent(order.Id, userId, farmerName, phone));
        return order;
    }

    public void AddItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("ORDER_NOT_EDITABLE", "Can only add items to pending orders.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            throw new DomainException("DUPLICATE_ORDER_ITEM", $"Product {productId} is already in the order.");

        _items.Add(new OrderItem(Id, productId, productName, quantity, unitPrice));
        RecalculateTotal();
    }

    public void Confirm(Guid agronomiystId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("ORDER_INVALID_STATE", $"Cannot confirm order in {Status} state.");

        Status = OrderStatus.Confirmed;
        AssignedAgronomiystId = agronomiystId;
        SetUpdatedAt();
        AddDomainEvent(new OrderConfirmedEvent(Id, UserId, agronomiystId));
    }

    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Delivered or OrderStatus.Cancelled)
            throw new DomainException("ORDER_INVALID_STATE", $"Cannot cancel order in {Status} state.");

        Status = OrderStatus.Cancelled;
        Notes = reason;
        SetUpdatedAt();
        AddDomainEvent(new OrderCancelledEvent(Id, UserId, reason));
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Aggregate(
            Money.Zero(),
            (sum, item) => sum.Add(item.UnitPrice.Multiply(item.Quantity)));
    }
}

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = default!;

    private OrderItem() { }

    internal OrderItem(Guid orderId, Guid productId, string productName, int quantity, Money unitPrice)
    {
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
