# MMogri

MMogri is a current long-term project of mine.
The goal is to create a fully fleshed out MMO framework from scratch with a big focus on moddability and customizability.

Currently the renderer is a .Net console app, drawing objects as simple ASCII representations, similar to how old school Roguelikes used to do it. I quite like the ASCII look but I'm considering adding an alternative sprite based renderer using OpenTK later on. 

Everything in very early stages of development and probably will be for quite a while. Don't expect anything to be finished, polished or working properly.

###### Roadmap

- **[DONE]** basic server -> client, client -> server communication
- **[DONE]** basic account management
- **[IN PROGRESS]** interpreting incomming client calls on server via runTime lua and sending response back
- **[TODO]** basic client input prediction
- **[TODO]** client map editor
- **[IN PROGRESS]** define player format properly
- **[IN PROGRESS]** define map format properly
- **[IN PROGRESS]** define tileset format properly
- **[IN PROGRESS]** smart server tick sending required information to client
- **[RESEARCH]** figure out a more dynamic way to handle action calls for specific situations
