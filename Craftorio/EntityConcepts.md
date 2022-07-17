# Entities
## Industry
### Miner:
- **IStoreBox**: to store the output of the miner
- **TimeConsumption**: to store the time consumption of the miner
- **ItemTarget**: to determine the item that the miner produces
- **Provider**: So this entity can be part of the logistic network
- **Sprite**: to display the miner

### Assembler:
- **IStoreBox**: to store the output of the assembler
- **ITakeableBox**: to store the input of the assembler
- **TimeConsumption**: to store the time consumption of the assembler
- **Recipe**: to determine the recipe that the assembler produces
- **Sprite**: to display the assembler
- **Provider?**: So this entity can be part of the logistic network
- **Requester?**: So this entity can be part of the logistic network


## LogisticNetwork
### Carrier:
- **Location**: the location in the world of the carrier
- **CarryData**: Store the state of the carrier: speed, content, order, travel state, etc
- **MovingObject**: The carrier can move in the world toward a entity
- **Sprite**: to display the carrier

### Nodes:
#### Base:
- **Location**: the location in the world of the node
- **NodeBase**: Information unique to the base, such as the carrier capacity.
- **Sprite**: to display the node

#### Storage:
- **Location**: the location in the world of the node
- **ITakeableBox**: to store the items in the storage
- **IStoreBox**: to store the items in the storage (input and output are the same)
- **Sprite**: to display the node

#### Providers: 
Providers are entities that can provide items to the network (like a miner), not entities on their own.
- **Location**: the location in the world of the provider
- **ITakeableBox**: to store the items in the provider
- **Provider**: So this entity can be part of the logistic network

#### Requesters:
Requesters are entities that can request items from the network (like a assembler), not entities on their own.
- **Location**: the location in the world of the requester
- **IStoreBox**: to store the items in the requester
- **Requester**: So this entity can be part of the logistic network

## Information
Information entities are entities that are not part of the game, but are used to display information to the player.

### Tooltips:
All tooltip entities are just `TextSprite` entities, with a `IsInfoBox` flag component. This is a mark to the engine saying that it is safe to remove the entity without corrupting the game.

#### Tooltip generators:
Some entities can generate tooltips when the mouse is over them. This is done by adding a `MouseOverDisplayText` component to that entity. The entity must have a location so the engine can determine if the mouse is over it.
- **Location**: the location in the world of the entity.
- **MouseOverDisplayText**: the text to display when the mouse is over the entity.
When the mouse is over the entity for a given amount of time, `DisplayTextGenerator` system will create a `TextSprite` entity with the text specified in the `MouseOverDisplayText` component; of course, this entity have the `IsInfoBox` flag.