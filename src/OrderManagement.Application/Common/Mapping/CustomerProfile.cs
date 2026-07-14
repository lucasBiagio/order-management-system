using AutoMapper;
using OrderManagement.Application.DTOs.Customers;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Common.Mapping;

public sealed class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, CustomerResponse>();
    }
}