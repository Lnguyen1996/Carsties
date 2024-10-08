﻿using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication application)
        {

            await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(application.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(i => i.Make, KeyType.Text)
                .Key(i => i.Model, KeyType.Text)
                .Key(i => i.Color, KeyType.Text)
                .CreateAsync();

            using var scope = application.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

            var items = await httpClient.GetItemsForSearchDb();

            Console.WriteLine(items.Count + " Return from the auction service");

            if (items.Count > 0)
            {
                await DB.SaveAsync(items);
            }
        }
    }
}
