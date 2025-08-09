using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Domain;

namespace XZY.WShop.Infrastructure.MappingProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AddProduct, Product>()
          .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
              src.ImageUrls != null ? string.Join(",", src.ImageUrls) : null));

            CreateMap<UpdateProductRequest, Product>()
         .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
             src.ImageUrls != null ? string.Join(",", src.ImageUrls) : null));

            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ImageUrls)
                        ? Array.Empty<string>()
                        : src.ImageUrls.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));

            CreateMap<Product, UpdateProductRequest>().ReverseMap();

            CreateMap<Customer, CreateCustomer>().ReverseMap();
            CreateMap<UpdateCustomer, Customer>().ReverseMap();
            CreateMap<Customer, CustomerResponse>().ReverseMap();

            CreateMap<Order, CreateOrder>().ReverseMap();
            CreateMap<Order, OrderResponse>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Amount));

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Price));

            CreateMap<Expense, AddExpense>().ReverseMap();
            CreateMap<UpdateExpense, Expense>().ReverseMap();
            CreateMap<Expense, ExpenseResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CreatedBy))
                .ReverseMap();
        }
    }
}

