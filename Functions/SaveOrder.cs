using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Cosmos;

public class SaveOrder
{
    private readonly Container _container;
    public SaveOrder(Container container) => _container = container;

    public record ShippingAddress(string name, string country, string city, string zip, string line1);
    public record Item(string sku, int productId, string name, int qty, decimal unitPrice);
    public record OrderDoc(string id, string orderId, DateTime createdUtc, string customerId,
                           ShippingAddress shippingAddress, List<Item> items, decimal finalPrice);

    [Function("SaveOrder")] // <— обязательно
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req)
    {
        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var doc = JsonSerializer.Deserialize<OrderDoc>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (doc is null) return req.CreateResponse(HttpStatusCode.BadRequest);

        await _container.UpsertItemAsync(doc, new PartitionKey(doc.orderId));
        return req.CreateResponse(HttpStatusCode.Created);
    }
}
