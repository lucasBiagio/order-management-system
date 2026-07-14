using AutoMapper;
using OrderManagement.Application.DTOs.Orders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Common.Mapping;

public sealed class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(
                destination => destination.Total,
                options => options.MapFrom(source => source.Total));

        CreateMap<Order, OrderResponse>();
    }
}