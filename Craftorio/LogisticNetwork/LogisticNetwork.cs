namespace Craftorio.Logistic;

/// <summary>
/// The brain of the logistic network meta-system.
/// </summary>
public class LogisticNetwork
{
    // private readonly LogisticOrdersManager logisticOrders;
    /// <summary>
    /// Stores the logistic orders which are buffered, but not yet assigned to a base node..
    /// </summary>
    private readonly Queue<LogisticOrder> logisticOrdersQueue = new Queue<LogisticOrder>();
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
            .With<IBox>()
            .AsSet();
        requestNodes = world.GetEntities()
            .With<RequestData>()
            .With<Location>()
            .With<IBox>()
            .AsSet();
        //logisticOrders = new LogisticOrdersManager();
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
    /// For each order, assign it to a base node.
    /// Currently thr assignation is by closest base node.
    /// </summary>
    public void AssignOrders()
    {
        // Take elements from the order queue and assign them to a base node which is closest to the provider entity.
        while (logisticOrdersQueue.Count > 0)
        {
            var order = logisticOrdersQueue.Dequeue();
            var provider = order.SourceNode;
            var providerLocation = provider.Get<Location>().AsVector;
            Entity closestNode;
            if (!TryGetClosestBase(providerLocation, out closestNode, true)) return;
            closestNode.Get<NodeBase>().OrdersQueue.Enqueue(order);
        }

        // If iterated over the entire logistic queue, rebuild it.
        if (logisticOrdersQueue.Count == 0)
            Rebuff();
    }

    /// <summary>
    /// Fills the orders buffers
    /// </summary>
    public void Rebuff()
    {
        // Cancel all orders and clear the orders queue
        logisticOrdersQueue.Clear();
        //var ordersSpan = logisticOrders.AsSpan();
        //var currentOrderIndex = 0;

        // Iterate for each requester, to find a provider to form an order
        foreach (var requester in requestNodes.GetEntities())
        {
            var requestData = requester.Get<RequestData>();
            var reqBox = requester.Get<IBox>();

            foreach (var request in requestData.AsDictionary)
            {
                // Iterate for each provider, to find a provider to form an order.
                // For now, just pick the first provider that has the requested item.
                foreach (var provider in provideNodes.GetEntities())
                {
                    // How many items we need to pick up?
                    var actualRequest = request.Value - requestData.OrdersOf(request.Key);
                    if (actualRequest <= 0)
                        break; // Ignore requests that are being satisfied.

                    var provideBox = provider.Get<IBox>();
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

                        this.logisticOrdersQueue.Enqueue(order);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the closest base node to the given location.
    /// </summary>
    public bool TryGetClosestBase(Vector2 location, out Entity node, bool withPositiveCapacity = false)
    {
        var nodes = this.nodes.GetEntities();
        var closestDistanceSquared = float.MaxValue;
        node = default(Entity);
        foreach (var baseNode in nodes)
        {
            // If the positive flag is set, we only want to get bases with positive capacity.
            if (withPositiveCapacity && baseNode.Get<NodeBase>().Capacity <= 0)
                continue;

            var baseLocation = baseNode.Get<Location>().AsVector;
            var distanceSquared = Vector2.DistanceSquared(location, baseLocation);
            if (distanceSquared < closestDistanceSquared)
            {
                closestDistanceSquared = distanceSquared;
                node = baseNode;
            }
        }

        if (node == default(Entity))
            return false;

        return true;
    }
}