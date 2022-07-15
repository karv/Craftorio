# The network
The network is a set of nodes of two types:
- **Providers** are nodes that provides resources to the network.
- **Consumers** are nodes that request resources from the network.
- **Storage** is a node that stores resources.
- **Base** are nodes used to store carriers.

## Node modifiers
Some nodes can be modified to provide additional functionality, like:
- Provider nodes can be modified to be set as active, so these resources will be transported to any storage node. The rebuffing order reverses here.

## Request array
The network must contain a queue of requests, which should be completed by the carriers in the same order.
When this queue is empty, the network will create a new one by comparing the providers and consumers. This process will be called **Rebuffing**.
If after that the queue is still empty, the network will pause and wait for new requests messages.

### Rebuffing algorithm
The rebuffing algorithm is a simple one:
- Iterate over all the requesters, and find the provider closest to the requester which have enough resources to satisfy the request, or to fill a carrier.
- When a provider-requester pair is found, the requester will be added to the queue, and the requested-provided resource will be removed from both the provider and the requester. A pair like these will be called **logistic order**.

### Logistic orders
A logistic order is a pair of provider-requester nodes, together with a resource type and amount.
This is a struct used to store the logistic orders queue in the network.
This object should be cancellable from the API, so the providers and requester status can be updated.
An exception should be thrown when this is cancelled when the order is being carried.

# Carriers
Carriers are nodes that can transport resources from one node to another.
They are ECS entities, which are created when they are sent to the world.

## Behavior
A state machine must be designed to handle the following states:
- **Idle**: the carrier is not moving.
- **Requesting**: the carrier is moving to a node that requests resources.
- **Delivering**: the carrier is moving to a node that provides resources.
- **Returning**: the carrier is moving to a **base**.

# Ideas
## As entity
The logistic network should be implemented as an element of the ECS model, not outside it. This way the code would become cleaner by removng the Logistic Network singleton and replaing it with a system. The following entities and components should be considred:
- An entity for each logistic network (possibly inly one, but the code would not change substancially if it were generalized). Each network would contain a unique ID, and access the content by a multimap via the world.
- A component unique for logistic network, so all the data used in the **old** logistic network must be moved into that component (for example, the content of items of the network, and the buffered logistic orders.)
- All logistic entities such as providers, requesters, etc, must also have a component addressing the ID of the network where they belong. Consider merging all/some of these components?