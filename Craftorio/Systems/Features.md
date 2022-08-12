This file contains descriptions on features (sets of systems that for a clear purpose)

# Item Production and production chain (not logistics)
Meaning the production of new items into the world.
- **TimeConsumingSystem**: All production systems are time consuming.
- **AssemblerProductionSystem**: This system is responsible for producing items from recipes.
- **MiningSystem**: This system is responsible for producing items from the ecosystem. No input is required.

# Logistics
Related to the logistics of the game.
Currently only one feature is implemented: the logistics network.
## Logistics network
The logistics network is a network of nodes and carriers.
- **CarrierExecuteSystem**: This system is responsible for determining the behavior of the carriers.
- **MovingObjectSystem**: This system is responsible for moving the carriers.
- **TimeConsumingSystem**: Bases require time to let the carriers out.
- **BaseNodeCarrierCreationSystem**: This system is responsible for creating the carriers for the nodes.

# Information
The player must be able to see information about the game, this information should be processed before-hand. Information features does just this.
- **TextDrawing**: will draw text on the screen.
- **DisplayTextGenerator**: Responsible of creating tooltip entities so the player can see information about the game. In this case, this is done by detecting and measuring the time the mouse is over an entity with the ability to generate a tooltip.

# Drawing
Systems responsible to render the entities on the screen. These systems and their main components are located on the `Drawing` namespace.
- **SpriteDrawing**: Responsible of drawing the sprites.
- **TextDrawing**: Responsible of drawing the text labels.

# Construction
Construct new buildings. The following systems are responsible for this:
- **ConstructionGatherSystem**: Will check the state of any construction entity in the gathering state, to determine if the resources are available so the state can be changed to construction.
- **ConstructionSystem**: Will check the state of any construction entity in the construction state, to determine if the construction is finished so we create the actual construction entity.
- **TimeConsumingSystem**: The construction of a building requires time on the construction step.
- **ConstructorExecuteSystem**: Responsible for leading the constructors from the bases to the construction site.