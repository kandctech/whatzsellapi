using AutoMapper;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest;
using XYZ.WShop.Application.Dtos.Customer;
using XYZ.WShop.Application.Dtos.Expense;
using XYZ.WShop.Application.Dtos.Followup;
using XYZ.WShop.Application.Dtos.Notification;
using XYZ.WShop.Application.Dtos.Orders;
using XYZ.WShop.Application.Dtos.Product;
using XYZ.WShop.Application.Dtos.Subscription;
using XYZ.WShop.Application.Dtos.Task;
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

            CreateMap<CreateCustomer, Customer>()
       .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
           src.Tags != null ? string.Join(",", src.Tags) : null));

            CreateMap<UpdateCustomer, Customer>()
         .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
             src.Tags != null ? string.Join(",", src.Tags) : null));

            CreateMap<Customer, CustomerResponse>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Tags)
                        ? Array.Empty<string>()
                        : src.Tags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)));



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

            CreateMap<Notification, NotificationRequest>().ReverseMap();
            CreateMap<Notification, NotificationResponse>().ReverseMap();

            CreateMap<SubscriptionPlan, SubscriptionPlanRequest>().ReverseMap();
            CreateMap<SubscriptionPlanUpdateRequest, SubscriptionPlan>().ReverseMap();
            CreateMap<SubscriptionPlan, SubscriptionPlanResponse>().ReverseMap();

            CreateMap<FollowUp, CreateFollowUpRequest>().ReverseMap();
            CreateMap<UpdateFollowUpRequest, FollowUp>().ReverseMap();
            CreateMap<FollowUp, FollowUpResponse>().ReverseMap();

            CreateMap<XYZ.WShop.Domain.TaskPlanner, CreateTaskRequest>().ReverseMap();
            CreateMap<UpdateTaskRequest, XYZ.WShop.Domain.TaskPlanner>().ReverseMap();
            CreateMap<XYZ.WShop.Domain.TaskPlanner, TaskRequestResponse>().ReverseMap();
        }
    }
}

