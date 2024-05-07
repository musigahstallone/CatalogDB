using CatalogDB.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace CatalogDB.Controllers;

public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/catalog");

        // Routes for querying catalog items.
        api.MapGet("/items", GetAllItems);
        api.MapGet("/items/byids", GetItemsByIds);
        api.MapGet("/items/{id:int}", GetItemById);
        api.MapGet("/items/byname/{name:minlength(1)}", GetItemsByName);
        return app;
    }

    public static async Task<Results<Ok<PaginatedItems<CatalogItem>>, BadRequest<string>>> GetAllItems(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = services.Context.Products.Count();

        var itemsOnPage = services.Context.Products
            .OrderBy(c => c.Name)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToList();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }

    public static async Task<Ok<List<CatalogItem>>> GetItemsByIds(
        [AsParameters] CatalogServices services,
        int[] ids)
    {
        var items = services.Context.Products.Where(item => ids.Contains(item.Id)).ToList();
        return TypedResults.Ok(items);
    }

    public static async Task<Results<Ok<CatalogItem>, NotFound, BadRequest<string>>> GetItemById(
        [AsParameters] CatalogServices services,
        int id)
    {
        if (id <= 0)
        {
            return TypedResults.BadRequest("Id is not valid.");
        }

        var item = await services.Context.Products.Where(ci => ci.Id == id).SingleOrDefaultAsync(ci => ci.Id == id);

        if (item == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(item);
    }
    public static async Task<Ok<PaginatedItems<CatalogItem>>> GetItemsByName(
        [AsParameters] PaginationRequest paginationRequest,
        [AsParameters] CatalogServices services,
        string name)
    {
        var pageSize = paginationRequest.PageSize;
        var pageIndex = paginationRequest.PageIndex;

        var totalItems = await services.Context.Products
            .Where(c => c.Name.Contains(name))
            .LongCountAsync();

        var itemsOnPage = await services.Context.Products
            .Where(c => c.Name.Contains(name))
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(new PaginatedItems<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage));
    }
}
