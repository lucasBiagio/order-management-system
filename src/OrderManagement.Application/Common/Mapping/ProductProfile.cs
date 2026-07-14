using AutoMapper;
using OrderManagement.Application.DTOs.Products;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Common.Mapping;

public sealed class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductResponse>();
    }
}