# Entities
## Industry
### Miner:
- **IOutputBox**: to store the output of the miner
- **TimeConsumption**: to store the time consumption of the miner
- **ItemTarget**: to determine the item that the miner produces
- **Provider**: So this entity can be part of the logistic network

### Assembler:
- **IOutputBox**: to store the output of the assembler
- **IInputBox**: to store the input of the assembler
- **TimeConsumption**: to store the time consumption of the assembler
- **Recipe**: to determine the recipe that the assembler produces
- **Provider?**: So this entity can be part of the logistic network
- **Requester?**: So this entity can be part of the logistic network


## LogisticNetwork
### Carrier:
- **Location**: the location in the world of the carrier
- **CarryData**: Store the state of the carrier: speed, content, order, travel state, etc
- **MovingObject**: The carrier can move in the world toward a entity

### Nodes:
#### Base:
- **Location**: the location in the world of the node
- **NodeBase**: Information unique to the base, such as the carrier capacity.

#### Storage:
- **Location**: the location in the world of the node
- **IInputBox**: to store the items in the storage
- **IOutputBox**: to store the items in the storage (input and output are the same)

#### Providers: 
Providers are entities that can provide items to the network (like a miner).
- **Location**: the location in the world of the provider
- **IOutputBox**: to store the items in the provider
- **Provider**: So this entity can be part of the logistic network 