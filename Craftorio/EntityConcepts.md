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

### SpriteBox:
Sprite box are entities that display a sprite or a texture.
- **Location**: the location in the world of the information entity
- **DisplayInfoState**: Information on the current state of the rendering of the information entity
- **Sprite**: to display the information entity