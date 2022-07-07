namespace Craftorio.Logistic;

/// <summary>
/// The brain of the logistic network meta-system.
/// </summary>
public class LogisticNetwork
{
    private readonly LogisticOrdersManager logisticOrders;
    private readonly DefaultEcs.EntitySet nodes;
    private readonly EntitySet provideNodes;
    private readonly EntitySet requestNodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogisticNetwork"/> class.
    /// </summary>
    public LogisticNetwork(World world)
    {
        World = world;
        nodes = world.GetEntities()
            .With<Location>()
            .With<NodeBase>()
            .AsSet();
        provideNodes = world.GetEntities()
            .With<ProvideData>()
            .With<Location>()
            .With<ITakeableBox>()
            .AsSet();
        requestNodes = world.GetEntities()
            .With<RequestData>()
            .With<Location>()
            .With<IStoreBox>()
            .AsSet();
        logisticOrders = new LogisticOrdersManager();
    }

    /// <summary>
    /// The amount of carriers in the network.
    /// </summary>
    public int AvailableCarrierCount { get; set; } = 100;

    /// <summary>
    /// Speed of produced carriers.
    /// </summary>
    public float CarriersSpeed { get; set; } = 1f;

    /// <summary>
    /// The ECS world where the network is.
    /// </summary>
    public World World { get; }

    /// <summary>
    /// Fills the orders buffers
    /// </summary>
    public void Rebuff()
    {
        // Cancel all orders and clear the orders queue
        logisticOrders.CancelAndClear();
        var ordersSpan = logisticOrders.AsSpan();
        var currentOrderIndex = 0;

        // Iterate for each requester, to find a provider to form an order
        foreach (var requester in requestNodes.GetEntities())
        {
            var requestData = requester.Get<RequestData>();
            var reqBox = requester.Get<IStoreBox>();

            foreach (var request in requestData.AsDictionary())
            {
                // Iterate for each provider, to find a provider to form an order.
                // For now, just pick the first provider that has the requested item.
                foreach (var provider in provideNodes.GetEntities())
                {
                    // How many items we need to pick up?
                    var actualRequest = request.Value - requestData.OrdersOf(request.Key);
                    if (actualRequest <= 0)
                        break; // Ignore requests that are being satisfied.

                    var provideBox = provider.Get<ITakeableBox>();
                    // How many items are available in the provider?
                    var providingCount = provideBox[request.Key] - requestData.OrdersOf(request.Key);
                    if (providingCount > 0)
                    {
                        var orderCount = Math.Min(actualRequest, providingCount);
                        // Create an order
                        var order = new LogisticOrder
                        {
                            DestinationNode = requester,
                            SourceNode = provider,
                            ItemId = request.Key,
                            Amount = orderCount
                        };

                        // Update the provide and request counts
                        provider.Get<ProvideData>().ChangeCurrentOrders(request.Key, order.Amount);
                        requester.Get<RequestData>().ChangeCurrentOrders(request.Key, order.Amount);

                        // Add the order to the queue
                        ordersSpan[currentOrderIndex++] = order;

                        // If the buffer is full, terminate this method
                        if (currentOrderIndex == ordersSpan.Length)
                        {
                            logisticOrders.SetCount(currentOrderIndex);
                            return;
                        }
                    }
                }
            }
        }

        // Set the count of the orders queue
        logisticOrders.SetCount(currentOrderIndex);
    }

    /// <summary>
    /// Gets the closest base node to the given location.
    /// </summary>
    public bool TryGetClosestBase(Vector2 location, out Entity node)
    {
        var nodes = this.nodes.GetEntities();
        // Return the first node, for now.
        node = nodes[0];
        return true;
    }

    /// <summary>
    /// Updates the network:
    /// - Send out a carrier if there are orders to fulfill.
    /// - Update the orders queue if the buffer is empty.
    /// </summary>
    public void Update()
    {
        // Check if there are any orders to process. If so, process the first one.
        if (logisticOrders.TryDequeue(1, out var order))
        {
            // TODO: create a carrier for the order.
            CreateCarrier(order);
            return;
        }

        // If there are no orders, check if there are any requests.
        Rebuff();
    }

    private Entity CreateCarrier(LogisticOrder order)
    {
        var carrier = World.CreateEntity();
        // Put it somewhere, at the origin for now
        carrier.Set<Location>();
        carrier.Set(new CarrierData
        {
            Order = order,
            State = CarrierState.Requesting,
            Content = new ItemStack { Count = 0, ItemId = order.ItemId },
            Network = this
        });
        carrier.Set(new MovingObject
        {
            MoveSpeed = CarriersSpeed,
            TargetEntity = order.SourceNode
        });
        World.Publish(new CarrierCreated { Carrier = carrier });
        return carrier;
    }
}