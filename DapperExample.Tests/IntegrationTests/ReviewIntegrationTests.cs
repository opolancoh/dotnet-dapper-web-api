using System.Net;
using System.Text;
using System.Text.Json;
using DapperExample.Tests.Helpers;
using DapperExample.Tests.IntegrationTests.Fixtures;
using DapperExample.Web.DTOs;

namespace DapperExample.Tests.IntegrationTests;

[Collection("SharedContext")]
public class ReviewIntegrationTests
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly HttpClient _httpClient;
    private const string BasePath = "/api/reviews";

    public ReviewIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();

        _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region GetAll

    [Fact]
    public async Task GeAll_ShouldReturnAllItems()
    {
        var response = await _httpClient.GetAsync($"{BasePath}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<List<ReviewDto>>(payloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(payloadObject?.Count >= DbDataHelper.Reviews.Count);
    }

    #endregion

    #region GeById

    [Theory]
    [MemberData(nameof(ItemIds))]
    public async Task GeById_ShouldReturnOnlyOneItem(Guid itemId)
    {
        var existingItem = DbDataHelper.Reviews.SingleOrDefault(x => x.Id == itemId);

        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<ReviewDto>(payloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(existingItem?.Comment, payloadObject?.Comment);
        Assert.Equal(existingItem?.Rating, payloadObject?.Rating);
        Assert.Equal(existingItem?.BookId, payloadObject?.BookId);
    }

    [Fact]
    public async Task GeById_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();

        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GeById_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string itemId = "not-valid-id";

        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ShouldCreateAnItem()
    {
        var newItem = new
        {
            Comment = "This is a new item to test Create_ShouldCreateAnItem",
            Rating = 3,
            BookId = DbDataHelper.BookId1
        };
        var payload = JsonSerializer.Serialize(newItem, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var locationHeader = response.Headers.GetValues("Location").FirstOrDefault();
        var newItemId = locationHeader?.Split('/').Last();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, new Guid(newItemId!));
        Assert.Contains(BasePath, locationHeader?.ToLower() ?? string.Empty);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsBase))]
    [MemberData(nameof(MissingRequiredFieldsForCreating))]
    public async Task Create_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(InvalidFields))]
    public async Task Create_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_ShouldUpdateAnItem()
    {
        // Create a new item
        var newItem = new
        {
            Comment = "This is a new item to test Update_ShouldUpdateAnItem",
            Rating = 3,
            BookId = DbDataHelper.BookId1
        };
        var newItemPayload = JsonSerializer.Serialize(newItem, _serializerOptions);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{BasePath}", newItemHttpContent);

        var locationHeader = newItemResponse.Headers.GetValues("Location").FirstOrDefault();
        var newItemId = locationHeader?.Split('/').Last();

        // Update the created item
        var itemToUpdate = new
        {
            Id = newItemId,
            Comment = "This is a new item to test Update_ShouldUpdateAnItem [updated]",
            Rating = 5,
        };
        var payload = JsonSerializer.Serialize(itemToUpdate, _serializerOptions);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BasePath}/{newItemId}", httpContent);

        // Ensure the item has been changed getting the item from the DB
        var updatedItemResponse = await _httpClient.GetAsync($"{BasePath}/{newItemId}");
        var updatedItemPayloadString = await updatedItemResponse.Content.ReadAsStringAsync();
        var updatedItemPayloadObject =
            JsonSerializer.Deserialize<ReviewDto>(updatedItemPayloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(itemToUpdate.Comment, updatedItemPayloadObject?.Comment);
        Assert.Equal(itemToUpdate.Rating, updatedItemPayloadObject?.Rating);
        Assert.Equal(newItem.BookId, updatedItemPayloadObject?.BookId);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var itemToUpdate = new
        {
            Id = itemId,
            Comment = "Comment",
            Rating = 3
        };

        var payload = JsonSerializer.Serialize(itemToUpdate, _serializerOptions);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsBase))]
    [MemberData(nameof(MissingRequiredFieldsForUpdating))]
    public async Task Update_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var itemId = new Guid();
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(InvalidFields))]
    public async Task Update_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var itemId = new Guid();
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId.ToString()}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    #endregion

    #region Remove

    [Fact]
    public async Task Remove_ShouldRemoveOnlyOneItem()
    {
        // Create a new item
        var newItem = new
        {
            Comment = "This is a new item to test Remove_ShouldRemoveOnlyOneItem",
            Rating = 3,
            BookId = DbDataHelper.BookId1
        };
        var newItemPayload = JsonSerializer.Serialize(newItem, _serializerOptions);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{BasePath}", newItemHttpContent);
        
        var locationHeader = newItemResponse.Headers.GetValues("Location").FirstOrDefault();
        var newItemId = locationHeader?.Split('/').Last();

        // Remove the created item
        var response = await _httpClient.DeleteAsync($"{BasePath}/{newItemId}");

        // Ensure the item has been deleted trying to get the item from the DB
        var deletedItemResponse = await _httpClient.GetAsync($"{BasePath}/{newItemId}");

        Assert.Equal(HttpStatusCode.Created, newItemResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deletedItemResponse.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var response = await _httpClient.DeleteAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string itemId = "not-valid-id";
        var response = await _httpClient.DeleteAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    public static TheoryData<string[], object> MissingRequiredFieldsBase => new()
    {
        { new[] { "The Comment field is required." }, new { Rating = 3, BookId = new Guid() } },
        { new[] { "The Rating field is required." }, new { Comment = "Comment", BookId = new Guid() } },
        { new[] { "The Comment field is required." }, new { Comment = "", Rating = 3, BookId = new Guid() } }
    };

    public static TheoryData<string[], object> MissingRequiredFieldsForCreating => new()
    {
        {
            new[]
            {
                "The Comment field is required.",
                "The Rating field is required.",
                "The BookId field is required."
            },
            new { }
        },
        { new[] { "The BookId field is required." }, new { Comment = "Comment", Rating = 3 } },
    };

    public static TheoryData<string[], object> MissingRequiredFieldsForUpdating => new()
    {
        { new[] { "The Id field is required." }, new { Comment = "Comment", Rating = 3, BookId = new Guid() } },
    };

    public static TheoryData<string[], object> InvalidFields => new()
    {
        {
            new[] { "The Rating field must be between 1 and 5." },
            new { Id = new Guid(), Comment = "Comment", Rating = 0, BookId = new Guid() }
        },
        {
            new[] { "The Rating field must be between 1 and 5." },
            new { Id = new Guid(), Comment = "Comment", Rating = 6, BookId = new Guid() }
        },
    };
    
    public static TheoryData<Guid> ItemIds => new()
    {
        { DbDataHelper.ReviewId1 },
        { DbDataHelper.ReviewId2 },
        { DbDataHelper.ReviewId3 },
        { DbDataHelper.ReviewId4 },
        { DbDataHelper.ReviewId5 },
        { DbDataHelper.ReviewId6 },
        { DbDataHelper.ReviewId7 },
        { DbDataHelper.ReviewId8 },
    };
}