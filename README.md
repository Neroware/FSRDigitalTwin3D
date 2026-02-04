# AROS2026-Reproduce
![image](./VirtualWorkspace.png)

This repository contains the knowledge server with our deployed ontological model [Sobots](https://github.com/Neroware/Sobots) presented for [IEEE ARSO 2026](https://ieee-arso.org/).

The system performs an interactive collaborative assembly using skill-based processing combined BDI agent modelling.

## Requirements
This project has OS support for Linux and Windows.

- .NET 8.*
- Unity 2022.3.8f1
- Jena Fuseki

## Get the simulation running!

1. Set-up knowledge server
    - Navigate to folder ```/FSR/DigitalTwin/Server/tools/```
    - Run script ```install-tools```
    - Load Git submodules ```git submodule update --init --recursive```
2. Set-up Unity simulation
    - Navigate to folder ```/FSR/DigitalTwin/Client/Unity/```
    - Load Git submodule ```git submodule update --init --recursive .```
    - Navigate into Unity project ```cd FSR.DigitalTwin.Client.Unity/```
    - Run script ```install-client-plugins```
    - Import the Unity project into Unity Hub and open
3. Launch Jena Fuseki
    - Run Jena Fuseki wit the the configuration at ```/FSR/DigitalTwin/Server/tools/LaunchJenaFuseki/run/configuration/fsrtriples.ttl```
    - Launch the knowledge server with DotNet ```/FSR/DigitalTwin/Server/src/FSR.DigitalTwin/FSR.DigitalTwin.csproj```
4. Run the simulation
    - Go to scene ```FSR/DigitalTwin/Client/Unity/FSR.DigitalTwin.Client.Unity/Assets/FSR/Scenes/UseCases/```
    - Enter play mode
    - Press *Connect*
    - Press *Run Simulation*
    - Place a screw at hole 1

## Controls
    - Directions: [W] [A] [S] [D]
    - Camera: Mouse
    - Change view: [V]
    - Free mouse: [E]

**Note**: If you have questions or suggestions that are not suitable for discussion within the issues section, feel free to send an e-mail to raoul.zebisch@uni-a.de.

**Licence**: MIT