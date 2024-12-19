﻿using Microsoft.AspNetCore.Identity;
using RestaurantCatalogService.Contracts;
using RestaurantCatalogService.Core.Abstractions;
using RestaurantCatalogService.Core.Models;
using RestaurantCatalogService.DataAccess.Entities;

namespace RestaurantCatalogService.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository restaurantRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository)
        {
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<IResult> GetRestaurants(GetRestaurantRequest request)
        {
            var restaurants = await restaurantRepository.Get(request.SearchCity, request.SearchPricecategory);

            var response = restaurants
                .Select(r => new RestaurantResponse(
                    Name: r.Name,
                    Description: r.Description,
                    PriceCategory: r.PriceCategory,
                    City: r.City,
                    Address: r.Address,
                    Rating: r.Rating))
                .ToList();

            return response == null ? Results.BadRequest() : Results.Ok(response);
        }

        public async Task<IResult> CreateRestaurant(CreateRestaurantRequest request)
        {
            var reviews = request.Reviews
                .Select(r => Review.Create(
                    Guid.NewGuid(),
                    r.Discription,
                    r.Score)
                .Review).ToList();

            var restaurant = Restaurant.Create(
                Guid.NewGuid(),
                request.Name,
                request.Description,
                request.PriceCategory,
                request.City,
                request.Address,
                reviews).Restaurant;

            var id = await restaurantRepository.Create(restaurant);

            return Results.Ok(id);
        }
    }
}
