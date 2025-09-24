# FSRDigitalTwin3D
FSRDigitalTwin3D is a process-oriented simulation tool to simulate processes of social human-robot interaction/collaboration. The design philosophy is to provide a "playground" for virtual robotics simulations abiding to the standards of modern Industry 4.0 applications.

**Note:** This framework is developed as a sub-project for the research association FORSocialRobots, namely "Subproject 4: Simulation and validation of socially cognitive robots in the digital twin"

## Requirements
This project has OS support for Linux and Windows.

- .NET 8.*
- Unity 2022.3.8f1

## Installation
### Unity Simulation Client (Virtualization Layer)
The client is located at ```/FSR/DigitalTwin/Client/Unity/FSR.DigitalTwin.Client/```:

1. Navigate to folder ```/FSR/Tools/```
2. Run script ```install-client-plugins```
3. Load Git submodules ```git submodule update --init --recursive```
4. Import the Unity project into Unity Hub and open

### Semantic Data Server (Digitization Layer)
The server solution is located at ```/FSR/DigitalTwin/Server/FSR.DigitalTwin.sln```

1. Navigate to folder ```/FSR/Tools/```
2. Run script ```install-tools```
3. Load Git submodules ```git submodule update --init --recursive```

## Required Infrastructure

### Semantic Data Repository (Apache Jena)
The semantic data repository is realized through [Apache Jena](https://github.com/apache/jena) querying a knowledge graph through a Fuseki Server. If you want to include or implement services for semantic reasoning, you need to either install Apache Jena Fuseki, or build it from source.

### ROS2 Workspace
If you want to use robot control via ROS2 Humble, set up our [Colcon Workspace](https://github.com/Neroware/FSRDigitalTwin3D).

The repo is linked as a submodule at path ```FSR/DigitalTwin/Server/modules/Ros2Ws```.

## More coming soon!
We are currently working on integrating a discrete-event process simulator into the simulation client. This allows us to simulate a first selection of basic test cases of human-robot interaction...

**Note**: This project is maintained by the [Chair of Digital Manufacturing](https://www.uni-augsburg.de/de/fakultaet/fai/informatik/prof/pi/) of the University of Augsburg.

**Note**: If you have questions or suggestions that are not suitable for discussion within the issues section, feel free to send an e-mail to raoul.zebisch@uni-a.de.

**Licence**: MIT