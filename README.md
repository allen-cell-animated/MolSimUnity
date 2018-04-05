# AgentSim
a simulation framework for molecular modeling

our goal is to create a system that:
- simulates molecules spatially with volume exclusion
- has the ability to form spatial complexes of molecules that each have a position and orientation within the complex
- takes common molecular model definitions as input, starting with BioNetGen and possibly also SMBL
- can define shapes of molecule containers based on 3D microscopy data
- calculates in real time so that users can change parameters on the fly
- eventually runs in a web browser, with an easy-to-use UI for defining models and changing parameters

currently prototyping in Unity WebGL, eventually will probably be translated to C++ or something for speed

chart of current architecture: https://www.lucidchart.com/documents/view/25a4e766-d7fe-4a87-b33f-157feaca604d/0
