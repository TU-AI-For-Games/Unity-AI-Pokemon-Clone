# AI Powered Pokémon Clone

[![](https://markdown-videos.deta.dev/youtube/Zkf9cAFNilE)](https://youtu.be/Zkf9cAFNilE)

# Project Details
The artefact created is a Pokémon battle simulator. The player navigates the world with a randomly selected team of 6 monsters from the original 151 monsters present in the 1996 video game. 

In the game world, there are several “wild pokémon”, who wander around the area using various pathfinding algorithms such as *A\* Pathfinding* and *Breadth-First Search*. In the sky, there are **Pidgey** who flock together. In another area, there are **Rattata** who use flocking with some bounds constraints to show off the *boids* algorithm in more depth. 

<img src="https://user-images.githubusercontent.com/48756858/235752937-d2fbf489-b3f3-43da-99e0-2ca612989b1f.png" width="80%">

When the player encounters a wild Pokemon, a battle begins. During the wild Pokemon battles, the player’s chosen actions are recorded, which is used as training data for the Neural Network implementation. After each battle, the player’s Pokemon are reset to full health, making them ready to battle again.

Elsewhere in the world, there are NPC characters - or *trainers* - who, when interacted with, challenge the player to a battle. The NPC decision-making uses Behaviour Trees informed by an *Artificial Neural Network*. The game is completed by winning every battle with the trainers. 

## Pathfinding Implementation - [Emil Walseth](https://github.com/emilwalseth)

Our pathfinding solution uses a grid of navigation points covering our walkable surface. Agents generate a path from their closest grid point, to their target's closest grid point.

<img src = "https://user-images.githubusercontent.com/48756858/235754250-cf86433e-752c-4802-8fb7-fd8785a4e4be.png" width=80%>

When generating, each grid point traces towards the surface to get surface information. This information allows for the calculation of surface angles. If the angle exceeds a certain threshold, the node is marked as blocked.

# Pathfinding Methods
To generate a path, algorithms consider the cost of movement, which may include both the "hCost" (distance from current to target position) and the "gCost" (distance from current to next node). Our implementation allows for different pathfinding algorithms.

## Breadth First Search (BFS)
Adds neighbouring nodes to a Queue, when selecting the next node, it chooses the next node in the queue.

## Depth First Search (DFS)
Adds each of the neighbouring nodes to a Stack, when selecting the next node, it chooses the top node of the Stack.

<img src = "https://user-images.githubusercontent.com/48756858/235754456-c902fc4f-ab53-4e8b-8cd5-2d9ff1ce6f43.png" width="50%">

## Best First Search
Checks the hCost of all our neighbours. Lowest hCost is chosen as the next node.

<img src = "https://user-images.githubusercontent.com/48756858/235754625-cf318147-01b9-4a6f-9212-8832fcf00c92.png" width="30%">

## Dijkstra
Checks the gCost of all our neighbours. Lowest gCost is chosen as the next node.

<img src = "https://user-images.githubusercontent.com/48756858/235754773-37ed3d3c-8eb0-40f2-a15f-95f4ac3ee756.png" width="50%"> 

## A Star
Checks both the gCost and the hCost of all our neighbours. The neighbour with the lowest combination of these is chosen as the next node.

<img src = "https://user-images.githubusercontent.com/48756858/235754928-077aba22-23d8-406b-953f-234495572c9f.png" width="50%">

Displaying all searched nodes helps identify the most efficient pathfinding algorithm. The A star algorithm is typically the fastest and most efficient, and is therefore our default implementation. 

### A Star Searched Nodes

<img src = "https://user-images.githubusercontent.com/48756858/235755089-2c3d79ad-a5b9-4a4e-888e-cc329e653b7a.png" width="50%">

### Dijkstra Searched Nodes
<img src = "https://user-images.githubusercontent.com/48756858/235755136-c673a9c8-5a76-431b-8367-215ad3e7c66d.png" width="50%">

# Modularity

Our implementation is highly customizable. All parameters are adjustable through the details panel. For debugging, users can choose to display grid points, paths, and which nodes are searched by our pathfinding algorithm.

<img src = "https://user-images.githubusercontent.com/48756858/235755282-009aa00c-3b36-4b86-923e-143186afa858.png" width="50%">

# Weight Zones
Weight zones can help the user define areas that require extra effort to navigate. By adding these  throughout the world, the user can specify how much additional cost should be added to the grid points.

<img src = "https://user-images.githubusercontent.com/48756858/235755385-2f2722c9-7014-4b3f-866c-565e305dd681.png" width="50%">

# Obstacles

If one of our nodes overlaps an object on a layer marked as unwalkable, the grid will mark the corresponding node as blocked.

<img src = "https://user-images.githubusercontent.com/48756858/235755735-ae8e04d3-af85-493b-af57-9da1d5ab5f60.png" width="50%">

# In the map

Here is a picture of the nav-grid in our scene. River is marked as un-walkable, weight zones are added in the forests.

<img src = "https://user-images.githubusercontent.com/48756858/235755893-e4738d72-f774-4bb6-917d-547053e84989.png" width="50%">

## Pathfinding Critical Evaluation:
The navigation system has demonstrated impressive performance. By adjusting the number of grid points and experimenting with pathfinding algorithms, we can test various levels of performance. 

Its modularity is a significant advantage, offering flexibility in parameters and weight zones to suit specific project requirements. Debugging is simplified, as the grid points, paths, and searched nodes can be easily displayed.

However, this implementation's reliance on a grid system may make it less suitable for complex or uneven terrain. Furthermore, the algorithm's efficiency decreases as the number of nodes on the grid increases. There are several potential solutions to these problems, including implementing more advanced pathfinding algorithms, such as hierarchical pathfinding. Another potential improvement could be to multithread the path generation process, particularly for larger grids, which could enhance the system's efficiency and reduce path generation time.

# Artificial Neural Network Implementation - [Tom Scott](http://www.github.com/tomdotscott)

Responsible for the Learning side of a project, in addition to the majority of the gameplay programming, I developed systems to collect data and implemented an Artificial Neural Network (ANN).

<img src = "https://user-images.githubusercontent.com/48756858/235756881-e0bf6c4b-1b2d-4808-b5ed-3ee20a33b5e4.png" width=40%>

The algorithm used to implement the neural network was a Feed-Forward Neural Network, using backpropagation to learn. This algorithm is well-suited to learning from datasets and has been shown to be effective in a variety of applications. Using multiple hidden layers, it calculates the stochastic gradient of descent with respect to the weights of the neurons. This includes Sigmoid, ReLU, and TanH activation functions for the neurons and He, Xavier, and Random neuron initialization methods.

<img src = "https://user-images.githubusercontent.com/48756858/235757461-c1160086-8cd6-41f4-97fc-585b800a0e08.png" width="50%">

<img src = "https://user-images.githubusercontent.com/48756858/235757511-18eac834-f7d6-4883-872d-3294826ab2bd.png" width="50%">

The ANNs can be saved and loaded in .csv format, allowing players to save time by not having to train the network every time they play the game. The shape and size of the hidden layers are described in the saved network, which includes the number of inputs, outputs, and the size of each of the hidden layers. After this, the layers get serialised line by line. Arrays are represented by numbers separated by commas within curly brackets, and 2D arrays, such as for the neuron weights and the delta weights, are just arrays of arrays represented by curly brackets within curly brackets.

<img src = "https://user-images.githubusercontent.com/48756858/235757580-d46235f1-ee78-4da3-a711-031c0d6dc2df.png" width="70%">

Training the network needed data, which was collected through a combination of player analytics and cleansing pre-existing data. Custom Python scripts were used to collate and normalise the collected data and also to cleanse the data. For example, to train the neural network on the type advantages in the game, the typeAdvantages.csv file (which the BattleManager uses to calculate damage) was split into several files to train the ANN.

<img src = "https://user-images.githubusercontent.com/48756858/235757666-4c44e405-c453-417d-b5b6-5a38749f7b0c.png" width="50%">
 
<img src = "https://user-images.githubusercontent.com/48756858/235757756-6fb2936b-a6da-4440-843c-2cc0abbf4d00.png" width="50%">

<img src = "https://user-images.githubusercontent.com/48756858/235757910-c0ae6911-5db0-4a9f-9fa0-44c2aca80e18.png" width="50%">

<img src = "https://user-images.githubusercontent.com/48756858/235757937-59d763d0-8ca6-4655-a764-2c9f8cf30328.png" width="50%">

A system was also created to record the player's chosen actions against wild Pokemon within the game. This data was normalized and collated, then used to train the decision-making ANN.

<img src = "https://user-images.githubusercontent.com/48756858/235758546-525e0ea5-3498-4f86-943e-6c286a0b8998.png" width="50%">

<img src = "https://user-images.githubusercontent.com/48756858/235759072-7b878b16-49ac-46db-8c5d-b9f693f623f5.png" width="50%">

<img src = "https://user-images.githubusercontent.com/48756858/235759114-8739c8cf-fd84-4573-8e16-9b528b745bb0.png" width="50%">

<img src = "https://user-images.githubusercontent.com/48756858/235759230-4710e1a1-9152-4591-936f-724ad385b45b.png" width="30%">

## Machine Learning Critical Evaluation:
In terms of performance, the **TypeLearner** was found to be 100% accurate in comparing the learned data to the dataset. This is likely due to the fact that there are only 289 entries in the dataset, which all have unique inputs and outputs. Contrastingly, the **MoveDecisionLearner** was found to be slightly less efficient, achieving about 77.3% accuracy. Tweaking the parameters of the ANN could potentially improve this accuracy. The current dataset, however, is ~600 entries. Collecting more data, including more Pokemon combinations, would improve accuracy. Despite this, the decision-making of the ANN is quite good in-game, and it has even been able to win a few times against me!

Using *softmax* calculations such as *binary cross-entropy loss* (BCEL) could also increase accuracy. BCEL works out the difference between the true and the predicted probability values for categorical tasks. The efficiency of the ANNs could be improved by offloading the workload to the GPU or using prewritten neural networks and libraries like *SciSharp* and Unity's *MLA*. 

Overall, the mistakes add to the game's realism, as human Pokemon players are also not likely to make the optimal choice in Pokemon battles.

# Flocking Implementation - [Kieron Killingsworth](https://github.com/AzureSun7)

This section allowed for the setting of the model to be used, the amount of them that would be spawned in, and the size of the boundary between each model. 

![image37](https://user-images.githubusercontent.com/48756858/235760412-06a5cfec-d674-4ae2-987a-d4c25c65ba1e.png)

Underneath, the speed at which the models would move around can be set, where the speed would be randomised between the minimum and maximum.

![image15](https://user-images.githubusercontent.com/48756858/235760461-6bcd0569-657c-460c-a0ab-3e2f2745ccf3.png)

In the first image above is the main code that will allow the flock to spawn in, taking into account their boundaries, positions and a randomised rotation, while underneath it, is where the movement vector is calculated using each of the behaviour vectors.

![image42](https://user-images.githubusercontent.com/48756858/235760822-dbdd34ce-72af-48bf-8267-dffe92af4282.png)

![image7](https://user-images.githubusercontent.com/48756858/235760872-ab06022b-9b90-4788-ba66-0a29422986db.png)

The distances and weights are set, including a *limit*, which is for managing how high aerial creatures can go. In Unity, each value can be adjusted to how much you want the AI to take them into consideration, for example, if *align* on both is maximised, then the models will stay identical in terms of their rotation.

![image20](https://user-images.githubusercontent.com/48756858/235761370-d8ed0191-8d8c-466e-94b6-2f5abd7a3e62.png)

Smooth damp is used to make sure that the models gradually move towards being at the set angle over time as smoothly as possible. The “is bird” is a simple check to see whether the selected model is an aerial creature, and if so, will allow it to fly, otherwise the model will be locked to the ground.

![image10](https://user-images.githubusercontent.com/48756858/235761710-972aa0d6-7b6b-4fb7-81ce-66ba0a1c7bb7.png)

![image35](https://user-images.githubusercontent.com/48756858/235761743-4b469eaf-a428-435b-83d6-7238e9389110.png)

## Flocking Implementation Critical Evaluation: 

Depending on how many members of the flock you have, the performance is quite stable with little to no issues to the game itself, however, if there are too many, then the frame rate will drop significantly. An alternative that I could have used to the algorithm I used was one that incorporated boids instead of model prefabs, which may have made the overall code a bit simpler.

I feel as though the method I used was more beneficial to the game we decided to go with as boids may not have worked well with the set of models we were using, whereas using set prefabs made it quite simple to implement. 

My flocking algorithm worked fairly well for what I set out to accomplish with it, allowing for both air and ground Pokémon to have flocks that work slightly differently, however, when seeing the models in motion, they do rotate fairly erratically when they aren’t spread out, so adjusting the rotation calculation may have made that less of an issue. The only real issue I had during the development of this algorithm itself was trying to understand how to control how the flock acted when the weights and distances of the behaviours were altered. For example, making sure that each member of the flock would not collide into one another.

The randomness of the movement, however, does work well when they’re separated more, and looks much more natural, instead of being hard-coded movements.

# Decision Making - [Jay Bunch](https://github.com/orgs/TU-AI-For-Games/people/goodeveningjay)

I was responsible for the Decision Making AI component of our project. The intended purpose of the decision making AI component was to drive the trainer AI combat and to work in conjunction with the neural network. Unfortunately I fell behind and Tom had to proceed without my contribution being implemented.

## Implementation

I briefly considered using a Finite State Machine as in a Pokemon battle there are only four actions that can be taken at any given time. However because these are actions that should be the result of decisions made, rather than states that are transitioned between, it quickly became obvious it shouldn’t be used here.

After I dismissed the idea of a FSM I approached Tom and we discussed all the components of a Pokemon battle that we wanted our AI to consider. For this exercise we considered how competitive players think when competing in tournaments. The outcome of these discussions were a set of 24 parameters we wanted our AI to consider, which Tom eventually whittled down to the 9 most important. This exercise informed how Tom designed the learning component and how I should design the decision making component.

![image28](https://user-images.githubusercontent.com/48756858/235762506-cc0aa837-79ee-4133-b8d2-0d8bc149ef5b.jpg)

Initially I attempted drafting a Decision Tree as they are fast to implement and relatively simple and efficient. As the complexity of the tree grew it became clear that it was not ideal for the sort of battle simulation we wanted (above image). This isn’t to say that a decision tree would not be appropriate, in fact the implementation in the main franchise games is likely done with decision trees. We wanted our AI to behave similarly to a human player though and so I was going to need to make use of the task driven structure of a behaviour tree.

I used [this](https://opensource.adobe.com/behavior_tree_editor/#/dash/home) visual editor to create my draft Behaviour Tree diagram:

![image25](https://user-images.githubusercontent.com/48756858/235763166-77eeaa2f-ec44-4365-991a-738a0660bcb9.jpg)

Unfortunately around the middle of March my progress stalled. The behaviour tree diagram I made was a useful brainstorming tool but was several iterations away from a final draft. I ran out of time and needed to begin programming an implementation so I only kept this first draft. I programmed the framework I was planning to use to create my behaviour tree. These scripts outlined the architecture for generating a behaviour tree. BTNode.cs, Selector.cs, Sequence.cs, and Tree.cs.

![image40](https://user-images.githubusercontent.com/48756858/235763661-127d3769-5e56-4de0-9bf5-ad0462437435.jpg)

Tree.cs contains a reference to a root node that recursively contains the entire rest of the tree. Upon start this script would then create a behaviour tree according to a SetupTree() function that I was going to define, and then in update if it has a tree it would evaluate continuously.

Sequence.cs and Selector.cs are composite nodes that behave like the AND logic gate and the OR logic gate respectively.

## Decision-Making Critical Evaluation:
The next step should have been implementing individual task scripts and building the tree but I did not achieve this. I think had I managed to complete an implementation it ultimately still would have been replaced by Tom’s neural net, but I should have been more proactive in catching up.


