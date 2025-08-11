# Large testing area for custom systems and frameworks
This project showcases systems and protype of game developed by me.

### Gameplay
![Unity_VJMNBa3nJx](https://github.com/user-attachments/assets/a5153892-e534-47aa-a770-45865a7508c2)

### Pause menu
![IxyaCZjTuO](https://github.com/user-attachments/assets/3dd5ab55-436c-4dc3-bebd-43f44b2c874c)

### Main menu
![Unity_IX0vCQ9W2b](https://github.com/user-attachments/assets/4600175e-4a99-4268-a563-f08093150834)

> Knight model made by Ayna Khallanova

---

## Architecture
The main system and entry point of the project is [App](https://github.com/LOK1fx/FeatGame/blob/main/Assets/_Game/GameSystem/App/Scripts/App.cs), which initializes all other systems and components for the game to function by [ProjectContext](https://github.com/LOK1fx/FeatGame/blob/main/Assets/_Game/GameSystem/App/Scripts/ProjectContext.cs).

The architecture is built on the principle of game modes, which turn scenes into a just game space, not specifically tied to a player object or their interface. This allows any level to be set with any set of game rules. The basic game rules include the player object, its interface, camera, and controller, but the initialization/deinitialization order, as well as the flow of the game mode, can be changed by creating a separate class with the IGameMode interface ([BaseGameMode](https://github.com/LOK1fx/FeatGame/blob/main/Assets/_Game/GameSystem/GameMode/Scripts/BaseGameMode.cs) can be used for a typical set of fields).

One of the main tasks of architecture was also to separate character control from a specific object. This is how controllers capable of changing the contolled object at runtime came to be. This can be used, for example, when the player character gets into a car and you need to control the car itself instead of the character, or when control in the game switches to a completely different character or even a group of characters.

There is a game [level system](https://github.com/LOK1fx/FeatGame/blob/main/Assets/_Game/GameSystem/Scripts/LevelManager.cs) (one or more scenes can be considered a game level) that controls the correct order of scene loading and the setting of the corresponding game mode. The game mode can change at any moment at the level (in this project, it can be changed thru the developer console by typing "gm_set <LEVEL_ID>").

## Side systems

The project also features a [developer console](https://github.com/LOK1fx/FeatGame/blob/main/Assets/_Game/Utility/Scripts/Console.cs) that works thru method reflection. To open the console in the game, you need to press "/".

In a branch [speech-synth-test](https://github.com/LOK1fx/FeatGame/tree/speech-synth-test), a speech synthesizer is being developed for rapid prototyping of dialogs or cutscenes, in-game videos.

There is also a simple localization system.

| Custom scene manager | Character spawn points and gizmos |
|---|---|
| <img alt="Unity_3qY1CPvAW9" src="https://github.com/user-attachments/assets/ccb8b8da-db76-4616-ba7f-5c712cbf72db" /> | <img alt="image" src="https://github.com/user-attachments/assets/9ace9e11-3edc-4f3a-8892-e4cc62d7246a" /> |


